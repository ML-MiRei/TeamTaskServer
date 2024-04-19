using Getaway.Application.CQRS.Project.Commands.LeaveFromProject;
using Getaway.Application.CQRS.Project.Commands.UpdateProject;
using Getaway.Application.CQRS.ProjectTask.Commands.AddInSprintProjectTask;
using Getaway.Application.CQRS.ProjectTask.Commands.ChangeStatusProjectTask;
using Getaway.Application.CQRS.ProjectTask.Commands.CreateProjectTask;
using Getaway.Application.CQRS.ProjectTask.Commands.DeleteProjectTask;
using Getaway.Application.CQRS.ProjectTask.Commands.SetExecutorProjectTask;
using Getaway.Application.CQRS.ProjectTask.Commands.UpdateProjectTask;
using Getaway.Application.ReturnsModels;
using Getaway.Core.Entities;
using Getaway.Presentation.Hubs;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Getaway.Presentation.Controllers
{
    [Route("{userId}/api/[controller]")]
    [ApiController]
    public class ProjectTaskController(IMediator mediator) : ControllerBase
    {
        NotificationHub notificationHub = new NotificationHub(mediator);


        [HttpPatch("{projectTaskId}/update")]
        public async Task<ActionResult> UpdateProjectTask(int projectTaskId, [FromBody] ProjectTaskEntity projectTaskEntity)
        {
            try
            {
                await mediator.Send(new UpdateProjectTaskCommand() { ProjectTaskId = projectTaskId, Title = projectTaskEntity.Title, Details = projectTaskEntity.Detail });

                return Ok();
            }
            catch
            {
                throw new Exception();
            }
        }


        [HttpPatch("{projectId}/{projectTaskId}/change-status")]
        public async Task<ActionResult> ChangeStatusProjectTask(int projectId, int projectTaskId, [FromBody] int status)
        {
            try
            {
                await mediator.Send(new ChangeStatusProjectTaskCommand() { ProjectTaskId = projectTaskId, Status = status });

                return Ok();
            }
            catch
            {
                throw new Exception();
            }
        }


        [HttpPatch("{projectTaskId}/set-executor")]
        public async Task<ActionResult> SetExecutorProjectTask(int projectTaskId, [FromBody] string userTag)
        {
            try
            {
                await mediator.Send(new SetExecutorProjectTaskCommand() { ProjectTaskId = projectTaskId, UserTag = userTag });
                return Ok();
            }
            catch
            {
                throw new Exception();
            }
        }


        [HttpPatch("{projectTaskId}/add-in-sprint")]
        public async Task<ActionResult> AddInSprintProjectTask(int projectTaskId, [FromBody] int sprintId)
        {
            try
            {
                Console.WriteLine();
                Console.WriteLine(sprintId);
                Console.WriteLine();
                await mediator.Send(new AddInSprintProjectTaskCommand() { ProjectTaskId = projectTaskId, SprintId = sprintId });
                return Ok();
            }
            catch
            {
                throw new Exception();
            }
        }

        [HttpPost("create")]
        public async Task<ActionResult<ProjectTaskModel>> CreateProjectTask([FromBody] ProjectTaskEntity projectTaskEntity)
        {
            try
            {
                Console.WriteLine(projectTaskEntity.SprintId);
                Console.WriteLine(projectTaskEntity.ProjectId);



                var projectTask = await mediator.Send(new CreateProjectTaskCommand
                {
                    Details = projectTaskEntity.Detail,
                    ProjectId = projectTaskEntity.ProjectId.Value,
                    Title = projectTaskEntity.Title,
                    SprintId = projectTaskEntity.SprintId,
                    Status = projectTaskEntity.Status
                });

                return Ok(new ProjectTaskModel
                {
                    Details = projectTask.Detail,
                    Title = projectTask.Title,
                    Status = projectTask.Status,
                    ExecutorName = null,
                    IsUserExecutor = false,
                    ProjectTaskId = projectTask.ID
                });
            }
            catch
            {
                throw new Exception();
            }
        }


        [HttpDelete("{projectTaskId}/delete")]
        public async Task<ActionResult> DeleteProjectTask(int projectTaskId)
        {
            try
            {
                await mediator.Send(new DeleteProjectTaskCommand() { ProjectTaskId = projectTaskId });
                return Ok();
            }
            catch
            {
                throw new Exception();
            }
        }



    }
}
