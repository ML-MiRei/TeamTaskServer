using Getaway.Application.ServicesInterfaces;
using Getaway.Core.Entities;
using Getaway.Core.Exceptions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Infrustructure.RepositoryImplementation
{

    public class TeamRepository : ITeamRepository
    {
        public async void AddUserInTeam(int teamId, string userTag)
        {
            try
            {
                Console.WriteLine($"{teamId}, {userTag}");
                await Connections.TeamServiceClient.AddUserInTeamAsync(new AddUserTeamRequest() { TeamId = teamId, UserTag = userTag });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception();
            }
        }

        public async Task<TeamEntity> CreateTeam(int userId, string name)
        {
            try
            {
                var reply = await Connections.TeamServiceClient.CreateTeamAsync(new CreateTeamRequest() { Name = name, UserId = userId });
                return new TeamEntity()
                {
                    ID = reply.TeamId,
                    TeamLeadId = reply.TeamLeadId,
                    Name = reply.TeamName,
                    Tag = reply.TeamTag
                };
            }
            catch
            {
                throw new Exception();
            }
        }

        public async Task<List<TeamEntity>> GetListTeams(int userId)
        {
            try
            {
                try
                {
                    var teams = (await Connections.TeamServiceClient.GetListTeamsAsync(new GetListTeamsRequest() { UserId = userId })).Teams;

                    if (teams == null || teams.Count == 0)
                    {
                        return new List<TeamEntity>();
                    }

                    return teams.Select(t => new TeamEntity() { Tag = t.TeamTag, TeamLeadId = t.TeamLeadId, Name = t.TeamName, ID = t.TeamId }).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw new NotFoundException();
                }
            }
            catch
            {
                throw new Exception();
            }
        }

        public async Task<List<UserEntity>> GetUsers(int teamId)
        {
            try
            {
                var users = (await Connections.TeamServiceClient.GetUsersAsync(new GetUsersRequest() { TeamId = teamId })).Users;
                if (users == null || users.Count == 0)
                {
                    throw new NotFoundException();
                }
                return users.Select(u => new UserEntity()
                {
                    Email = u.Email,
                    FirstName = u.FirstName,
                    SecondName = u.SecondName,
                    LastName = u.LastName,
                    PhoneNumber = u.PhoneNumber,
                    Tag = u.UserTag,
                    ID = u.UserId.Value
                }).ToList();
            }
            catch
            {
                throw new Exception();
            }
        }

        public async void DeleteUserFromTeam(string userTag, int teamId)
        {
            try
            {
                await Connections.TeamServiceClient.DeleteUserFromTeamAsync(new DeleteUserFromTeamRequst() { TeamId = teamId, UserTag = userTag });
            }
            catch
            {
                throw new Exception();
            }
        }

        public async void UpdateTeam(TeamEntity teamEntity)
        {
            try
            {
                await Connections.TeamServiceClient.UpdateTeamAsync(
                    new UpdateTeamsRequest() { TeamId = teamEntity.ID.Value, TeamLeadTag = teamEntity.TeamLeadTag, Name = teamEntity.Name });
            }
            catch
            {
                throw new Exception();
            }
        }

        public async void LeaveTeam(int userId, int teamId)
        {
            try
            {
                await Connections.TeamServiceClient.LeaveTeamAsync(
                    new LeaveTeamRequest() { TeamId = teamId, UserId = userId });
            }
            catch
            {
                throw new Exception();
            }
        }

        public async Task<TeamEntity> GetTeam(int teamId)
        {
            try
            {
                var reply = await Connections.TeamServiceClient.GetTeamAsync(new GetTeamRequest { TeamId = teamId });
                return new TeamEntity
                {
                    ID = reply.TeamId,
                    Name = reply.TeamName,
                    Tag = reply.TeamTag,
                    TeamLeadId = reply.TeamLeadId
                };
            }
            catch
            {
                throw new Exception();
            }
        }
    }
}
