using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Server.Data;
using Server.Data.Model;
using Services;

namespace Server.Services
{
    public class TeamApiService : TeamService.TeamServiceBase
    {
        MyDbContext db;

        public TeamApiService()
        {
            db = Config.myDbContext;
        }


        public override Task<VoidTeamReply> AddUserTeam(AddUserTeamRequest request, ServerCallContext context)
        {
            try
            {
                Console.WriteLine(request.UserTag);
                foreach (var user in db.Users)
                {
                    Console.WriteLine(user.Tag);
                    Console.WriteLine(user.Tag == request.UserTag);
                }

                db.Teams_Users.Add(new Team_User()
                {
                    ID_Team = request.IdTeam,
                    ID_User = db.Users.First(u => u.Tag == request.UserTag).ID
                });

                db.SaveChanges();
                Console.WriteLine("add db complete");
                return Task.FromResult(new VoidTeamReply());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new RpcException(new Status(StatusCode.Internal, "add db error"));
            }
        }

        public override Task<TeamModel> CreateTeams(CreateTeamRequest request, ServerCallContext context)
        {
            try
            {
                Team team = new Team()
                {
                    ID_Lead = request.IdUser,
                    Name = request.Name
                };
                db.Teams.Add(team);
                db.SaveChanges();

                db.Teams_Users.Add(new Team_User()
                {
                    ID_Team = team.ID,
                    ID_User = request.IdUser
                });

                db.SaveChanges();

                return Task.FromResult(new TeamModel()
                {
                    IdTeam = team.ID,
                    Name = team.Name,
                    TagLead = db.Users.First(u=> u.ID == request.IdUser).Tag
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new RpcException(new Status(StatusCode.Internal, "add db error"));
            }
        }

        public override Task<ListTeamsReply> GetListTeams(GetListTeamsRequest request, ServerCallContext context)
        {
            ListTeamsReply listTeams = new ListTeamsReply();
            List<TeamModel> replyList = (from t in db.Teams
                                         join tu in db.Teams_Users on t.ID equals tu.ID_Team
                                         where tu.ID_User == request.IdUser
                                        select new TeamModel()
                                        {
                                            TagLead = db.Users.First(u=> u.ID == t.ID_Lead).Tag,
                                            IdTeam = t.ID,
                                            Name = t.Name
                                        }).ToList();

            listTeams.Teams.AddRange(replyList);
            return Task.FromResult(listTeams);
        }

        public override Task<LeaveTeamReply> LeaveTeam(LeaveTeamRequst request, ServerCallContext context)
        {
            try
            {
                Team_User team_User = db.Teams_Users.First(tu => tu.ID_Team == request.IdTeam 
                                                && tu.ID_User == db.Users.First(p=>p.Tag == request.TagUser).ID);
                db.Teams_Users.Remove(team_User);
                db.SaveChanges();

                if(!db.Teams_Users.Any(tu=> tu.ID_Team == request.IdTeam))
                {
                    Team team = db.Teams.First(t => t.ID == request.IdTeam);
                    db.Teams.Remove(team);
                    db.SaveChanges();
                }

                return Task.FromResult(new LeaveTeamReply() { IdTeam = request.IdTeam});
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new RpcException(new Status(StatusCode.Internal, "delete db error"));
            }
        }

        public override Task<VoidTeamReply> UpdateTeams(UpdateTeamsRequest request, ServerCallContext context)
        {
            try
            {
                Team team = db.Teams.First(t => t.ID == request.IdTeam);
                if(request.TagLead != null)
                    team.ID_Lead = db.Users.First(u => u.Tag == request.TagLead).ID;
                if(request.Name != null)
                    team.Name = request.Name;
                db.Teams.Update(team);
                db.SaveChanges(true);
                return Task.FromResult(new VoidTeamReply());

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new RpcException(new Status(StatusCode.Internal, "update db error"));
            }
        }
    }
}
