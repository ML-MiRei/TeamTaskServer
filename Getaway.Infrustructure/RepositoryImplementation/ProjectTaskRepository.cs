using Getaway.Application.RepositoriesInterfaces;
using Getaway.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Infrustructure.RepositoryImplementation
{
    internal class ProjectTaskRepository : IProjectTaskRepository
    {
        public async void AddInSprintProjectTask(int projectTaskId, int sprintId)
        {
            try
            {
                await Connections.ProjectTaskServiceClient.AddProjectTaskInSprintAsync(new AddProjectTaskInSprintRequest() { ProjectTasksId = projectTaskId, SprintId = sprintId });
            }
            catch
            {
                throw new Exception();
            }
        }

        public async void ChangeStatusProjectTask(int projectTaskId, int status)
        {

            try
            {
                await Connections.ProjectTaskServiceClient.ChangeStatusProjectTaskAsync(new ChangeStatusProjectTaskRequest() { ProjectTasksId = projectTaskId, Status = status });
            }
            catch
            {
                throw new Exception();
            }
        }



        public async Task<ProjectTaskEntity> CreateProjectTask(int projectId, int? sprintId, string title, string details, int status)
        {

            try
            {
                var projectTask = await Connections.ProjectTaskServiceClient.CreateProjectTaskAsync(new CreateProjectTaskRequest()
                {
                    ProjectId = projectId,
                    SprintId = sprintId,
                    Details = details,
                    Title = title,
                    Status = status
                });
                return new ProjectTaskEntity
                {
                    ID = projectTask.ProjectTasksId,
                    Detail = projectTask.Details,
                    Title = projectTask.Title,
                    SprintId = projectTask.SprintId,
                    ProjectId = projectTask.ProjectId,
                    Status = projectTask.Status
                };
            }
            catch
            {
                throw new Exception();
            }
        }

        public async void DeleteProjectTask(int projectTaskId)
        {

            try
            {
                await Connections.ProjectTaskServiceClient.DeleteProjectTaskAsync(new DeleteProjectTaskRequest() { ProjectTaskId = projectTaskId });
            }
            catch
            {
                throw new Exception();
            }
        }

        public async Task<List<ProjectTaskEntity>> GetListProjectTasks(int projectId)
        {

            try
            {
                var projectTasks = await Connections.ProjectTaskServiceClient.GetListProjectTasksAsync(new GetListProjectTasksRequest() { ProjectId = projectId });
                return projectTasks.ProjectTasks.Select(p => new ProjectTaskEntity
                {
                    Detail = p.Details,
                    Title = p.Title,
                    SprintId = p.SprintId,
                    ID = p.ProjectTasksId,
                    UserId = p.UserId,
                    Status = p.Status,
                    ProjectId = p.ProjectId

                }).ToList();


            }
            catch
            {
                throw new Exception();
            }
        }



        public async void SetExecutorProjectTask(int projectTaskId, string userTag)
        {

            try
            {
                await Connections.ProjectTaskServiceClient.SetExecutorProjectTaskAsync(new SetExecutorProjectTaskRequest() { ProjectTasksId = projectTaskId, UserTag = userTag });
            }
            catch
            {
                throw new Exception();
            }
        }

        public async void UpdateProjectTask(int projectTaskId, string? title, string? details)
        {

            try
            {
                await Connections.ProjectTaskServiceClient.UpdateProjectTaskAsync(new UpdateProjectTaskRequest() { ProjectTasksId = projectTaskId, Details = details, Title = title });
            }
            catch
            {
                throw new Exception();
            }
        }
    }
}
