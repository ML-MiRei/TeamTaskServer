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
        void UpdateProjects(int projectId, string? creatorTag, string? name);
        void DeleteProjects(int projectId);
        Task<ProjectEntity> CreateProjects(int userId, string name);
        Task<List<ProjectEntity>> GetListProjects(int userId);
        Task<List<UserEntity>> GetUsersByProject(int projectId);
        void AddTeamInProject(int projectId, string teamTag);
        void AddUserInProject(int projectId, string userTag);
        void DeleteUserFromProject(int projectId, string userTag);
        void LeaveFromProject(int projectId, int userId);
    }
}
