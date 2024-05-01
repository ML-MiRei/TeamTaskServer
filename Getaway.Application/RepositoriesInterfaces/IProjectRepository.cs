using Getaway.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.RepositoriesInterfaces
{
    public interface IProjectRepository
    {
        Task UpdateProjects(int projectId, string? creatorTag, string? name);
        Task DeleteProjects(int projectId);
        Task<ProjectEntity> CreateProjects(int userId, string name);
        Task<List<ProjectEntity>> GetListProjects(int userId);
        Task<ProjectEntity> GetProject(int projectId);
        Task<List<UserEntity>> GetUsersByProject(int projectId);
        Task AddTeamInProject(int projectId, string teamTag);
        Task AddUserInProject(int projectId, string userTag);
        Task DeleteUserFromProject(int projectId, string userTag);
        Task LeaveFromProject(int projectId, int userId);
    }
}
