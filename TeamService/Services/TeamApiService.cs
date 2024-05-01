using GreatDatabase.Data;
using GreatDatabase.Data.Model;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.EntityFrameworkCore;
using TeamService;

namespace TeamService.Services
{
    public class TeamApiService : TeamService.TeamServiceBase
    {

        private static MyDbContext db;



        private readonly ILogger<TeamApiService> _logger;
        public TeamApiService(ILogger<TeamApiService> logger)
        {
            _logger = logger;
            db = MyDbContext.GetInstance;
        }


        public async override Task<TeamModel> GetTeamByTag(GetTeamByTagRequest request, ServerCallContext context)
        {
            try
            {
                var team = await db.Teams.FirstAsync(t => t.TeamTag == request.TeamTag);
                return new TeamModel
                {
                    TeamId = team.ID,
                    TeamLeadId = team.TeamLeadId,
                    TeamName = team.TeamName,
                    TeamTag = team.TeamTag,
                };
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        public async override Task<TeamModel> GetTeam(GetTeamRequest request, ServerCallContext context)
        {
            try
            {
                var team = await db.Teams.FindAsync(request.TeamId);
                return new TeamModel
                {
                    TeamId = team.ID,
                    TeamLeadId = team.TeamLeadId,
                    TeamName = team.TeamName,
                    TeamTag = team.TeamTag,
                };
            }

            catch (Exception)
            {
                return null;
            }


        }

        public async override Task<VoidTeamReply> LeaveTeam(LeaveTeamRequest request, ServerCallContext context)
        {
            try
            {
                var user = await db.Teams_Users.FirstAsync(c => c.TeamId == request.TeamId && c.UserId == request.UserId);
                int teamId = user.TeamId;
                db.Teams_Users.Remove(user);
                await db.SaveChangesAsync();

                _logger.LogInformation($"User with id = {user.UserId} leave team with id = {teamId}");

                if (!db.Teams_Users.Any(cu => cu.TeamId == teamId))
                {
                    Team team = await db.Teams.FindAsync(teamId);
                    db.Teams.Remove(team);
                    await db.SaveChangesAsync();

                    _logger.LogInformation($"Team {teamId} is deleted");
                }



                return new VoidTeamReply();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception();
            }
        }

        public async override Task<VoidTeamReply> AddUserInTeam(AddUserTeamRequest request, ServerCallContext context)
        {
            try
            {
                var userId = db.Users.First(u => u.UserTag == request.UserTag).ID;

                await db.Teams_Users.AddAsync(new Team_User()
                {
                    TeamId = request.TeamId,
                    UserId = userId,
                    DateCreated = DateTime.Now
                });

                await db.SaveChangesAsync();

                _logger.LogInformation($"Add user with tad {request.UserTag} in team with id {request.TeamId}");

                return await Task.FromResult(new VoidTeamReply());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException.Message);
                throw new RpcException(new Status(StatusCode.Internal, "add db error"));
            }
        }

        public async override Task<CreateTeamReply> CreateTeam(CreateTeamRequest request, ServerCallContext context)
        {
            try
            {

                Team team = new Team()
                {
                    TeamLeadId = request.UserId,
                    TeamName = request.Name,
                    DateCreated = DateTime.Now,
                    LastModified = DateTime.Now,
                    TeamTag = GetUniqueTag()
                };


                await db.Teams.AddAsync(team);
                await db.SaveChangesAsync();

                _logger.LogInformation($"Created team: {team.TeamName}, {team.TeamTag}");


                await db.Teams_Users.AddAsync(new Team_User()
                {
                    TeamId = team.ID,
                    UserId = request.UserId,
                    DateCreated = DateTime.Now
                });

                await db.SaveChangesAsync();

                _logger.LogInformation($"User with id {request.UserId} add in {team.TeamName}");

                return await Task.FromResult(new CreateTeamReply()
                {
                    TeamId = team.ID,
                    TeamTag = team.TeamTag,
                    TeamName = team.TeamName,
                    TeamLeadId = team.TeamLeadId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.InnerException.Message);
                _logger.LogError(ex.Message);
                throw new RpcException(new Status(StatusCode.Internal, "add db error"));
            }
        }

        public async override Task<ListTeamsReply> GetListTeams(GetListTeamsRequest request, ServerCallContext context)
        {

            ListTeamsReply listTeams = new ListTeamsReply();

            List<TeamModel> replyList = await Task.Run(() => (from t in db.Teams
                                                              join tu in db.Teams_Users on t.ID equals tu.TeamId
                                                              where tu.UserId == request.UserId
                                                              select new TeamModel()
                                                              {
                                                                  TeamLeadId = t.TeamLeadId,
                                                                  TeamTag = t.TeamTag,
                                                                  TeamId = t.ID,
                                                                  TeamName = t.TeamName
                                                              }).ToList());

            listTeams.Teams.AddRange(replyList);

            _logger.LogInformation($"Return {replyList.Count} teams");

            return await Task.FromResult(listTeams);
        }

        public async override Task<VoidTeamReply> DeleteUserFromTeam(DeleteUserFromTeamRequst request, ServerCallContext context)
        {
            try
            {
                var userId = (await db.Users.FirstAsync(u => u.UserTag == request.UserTag)).ID;

                var team_User = await db.Teams_Users.FirstAsync(tu => tu.TeamId == request.TeamId && tu.UserId == userId);

                db.Teams_Users.Remove(team_User);
                await db.SaveChangesAsync();

                _logger.LogInformation($"User {request.UserTag} leave team with id {request.TeamId}");

                if (!db.Teams_Users.Any(tu => tu.TeamId == request.TeamId))
                {
                    Team team = db.Teams.First(t => t.ID == request.TeamId);
                    db.Teams.Remove(team);
                    await db.SaveChangesAsync();

                    _logger.LogInformation($"Team {team.TeamName} was deleted");
                }


                return await Task.FromResult(new VoidTeamReply());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new RpcException(new Status(StatusCode.Internal, "delete db error"));
            }
        }

        public async override Task<VoidTeamReply> UpdateTeam(UpdateTeamsRequest request, ServerCallContext context)
        {
            try
            {

                var team = await db.Teams.FirstAsync(t => t.ID == request.TeamId);



                team.TeamLeadId = String.IsNullOrEmpty(request.TeamLeadTag) ? team.TeamLeadId : (await db.Users.FirstOrDefaultAsync(u => u.UserTag == request.TeamLeadTag)).ID;
                team.TeamName = String.IsNullOrEmpty(request.Name) ? team.TeamName : request.Name;

                db.Teams.Update(team);
                await db.SaveChangesAsync();

                _logger.LogInformation($"Update team");

                return new VoidTeamReply();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException.Message);
                throw new RpcException(new Status(StatusCode.Internal, "update db error"));
            }
        }

        public async override Task<GetUsersReply> GetUsers(GetUsersRequest request, ServerCallContext context)
        {
            try
            {
                GetUsersReply listTeams = new GetUsersReply();
                var t = Task.Run(() => (
                                             from tu in db.Teams_Users
                                             join u in db.Users on tu.UserId equals u.ID
                                             where tu.TeamId == request.TeamId
                                             select new UserReply()
                                             {
                                                 Email = u.Email,
                                                 FirstName = u.FirstName,
                                                 LastName = u.LastName,
                                                 SecondName = u.SecondName,
                                                 PhoneNumber = u.PhoneNumber,
                                                 UserTag = u.UserTag,
                                                 UserId = u.ID

                                             }).ToList());


                List<UserReply> replyList = await t;

                Console.WriteLine($"users api amount = {replyList.Count}");

                listTeams.Users.AddRange(replyList);
                return await Task.FromResult(listTeams);

            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                await Console.Out.WriteLineAsync(ex.InnerException.Message);

                await Console.Out.WriteLineAsync("return empty list");
                var reply = new GetUsersReply();
                reply.Users.AddRange(new List<UserReply>());
                return reply;
            }

        }


        private string GetUniqueTag()
        {

            while (true)
            {
                string tag = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Replace("=", "").Replace("+", "").Replace("/", "").Substring(0, 15);
                if (!(from u in db.Teams
                      where u.TeamTag == tag
                      select u).Any())
                {
                    return tag;
                }

            }
        }

    }
}
