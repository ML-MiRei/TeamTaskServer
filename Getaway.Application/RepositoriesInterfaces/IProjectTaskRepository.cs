using Getaway.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.RepositoriesInterfaces
{
    public interface IProjectTaskRepository
    {
        Task UpdateProjectTask(int projectTaskId, string? title, string? details);
        Task ChangeStatusProjectTask(int projectTaskId, int status);
        Task AddInSprintProjectTask(int projectTaskId, int sprintId);
        Task SetExecutorProjectTask(int projectTaskId, string userTag);
        Task DeleteProjectTask(int projectTaskId);
        Task<ProjectTaskEntity> CreateProjectTask(int projectId, int? sprintId, string title, string details, int status);
        Task<List<ProjectTaskEntity>> GetListProjectTasks(int projectId);
    }
}
