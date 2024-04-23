using Getaway.Application.CQRS.Messenger.Chat.Queries.GetChat;
using Getaway.Application.CQRS.Messenger.Message.Queries.GetMessages;
using Getaway.Application.CQRS.Notification.Commands.CreateNotification;
using Getaway.Application.CQRS.Project.Commands.LeaveFromProject;
using Getaway.Application.CQRS.Project.Commands.UpdateProject;
using Getaway.Application.CQRS.Project.Queries.GetUsersByProject;
using Getaway.Application.CQRS.ProjectTask.Commands.AddInSprintProjectTask;
using Getaway.Application.CQRS.ProjectTask.Commands.ChangeStatusProjectTask;
using Getaway.Application.CQRS.ProjectTask.Commands.CreateProjectTask;
using Getaway.Application.CQRS.ProjectTask.Commands.DeleteProjectTask;
using Getaway.Application.CQRS.ProjectTask.Commands.SetExecutorProjectTask;
using Getaway.Application.CQRS.ProjectTask.Commands.UpdateProjectTask;
using Getaway.Application.CQRS.Team.Queries.GetUsersByChhat;
using Getaway.Application.CQRS.User.Queries.GetUserByTag;
using Getaway.Application.ReturnsModels;
using Getaway.Core.Entities;
using Getaway.Core.Enums;
using Getaway.Presentation.Hubs;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Client;
using System;

namespace Getaway.Presentation.Controllers
{
    [Route("{userId}/api/[controller]")]
    [ApiController]
    public class ProjectTaskController(IMediator mediator) : ControllerBase
    {


        // WARNING


        [HttpPatch("{projectTaskId}/add-in-sprint")]
        public async Task<ActionResult> AddInSprintProjectTask(int projectTaskId, [FromBody] int sprintId)
        {
            try
            {
                await mediator.Send(new AddInSprintProjectTaskCommand() { ProjectTaskId = projectTaskId, SprintId = sprintId });
                return Ok();
            }
            catch
            {
                throw new Exception();
            }
        }


    }
}
