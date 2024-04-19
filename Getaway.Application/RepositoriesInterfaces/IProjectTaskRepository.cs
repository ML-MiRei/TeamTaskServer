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
        void UpdateProjectTask(int projectTaskId, string? title, string? details);
        void ChangeStatusProjectTask(int projectTaskId, int status);
        void AddInSprintProjectTask(int projectTaskId, int sprintId);
        void SetExecutorProjectTask(int projectTaskId, string userTag);
        void DeleteProjectTask(int projectTaskId);
        Task<ProjectTaskEntity> CreateProjectTask(int projectId, int? sprintId, string title, string details, int status);
        Task<List<ProjectTaskEntity>> GetListProjectTasks(int projectId);
    }
}
