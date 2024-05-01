using Getaway.Application.ReturnsModels;
using Getaway.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.ServicesInterfaces
{
    public interface ITeamRepository
    {
        Task UpdateTeam(TeamEntity teamEntity);
        Task DeleteUserFromTeam(string userTag, int teamId);
        Task<TeamEntity> CreateTeam(int userId, string name);
        Task<List<TeamEntity>> GetListTeams(int userId);
        Task AddUserInTeam(int teamId, string userTag);
        Task<List<UserEntity>> GetUsers(int userId);
        Task LeaveTeam(int userId, int teamId);
        Task<TeamEntity> GetTeam(int teamId);
        Task<TeamEntity> GetTeamByTag(string teamTag);
    }
}
