using Getaway.Application.CQRS.Project.Commands.AddTeamInProject;
using Getaway.Application.CQRS.Project.Commands.AddUserInProject;
using Getaway.Application.CQRS.Project.Commands.CreateProject;
using Getaway.Application.CQRS.Project.Commands.DeleteProject;
using Getaway.Application.CQRS.Project.Commands.DeleteTeamFromProject;
using Getaway.Application.CQRS.Project.Commands.DeleteUserFromProject;
using Getaway.Application.CQRS.Project.Commands.LeaveFromProject;
using Getaway.Application.CQRS.Project.Commands.UpdateProject;
using Getaway.Application.CQRS.Project.Queries.GetProjects;
using Getaway.Application.CQRS.Project.Queries.GetUsersByProject;
using Getaway.Application.CQRS.ProjectTask.Queries.GetProjectTasks;
using Getaway.Application.CQRS.Sprint.Commands.GetSprints;
using Getaway.Application.CQRS.Team.Commands.AddUserInTeam;
using Getaway.Application.CQRS.User.Queries.GetUserById;
using Getaway.Application.ReturnsModels;
using Getaway.Core.Entities;
using Getaway.Core.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using System;

namespace Getaway.Presentation.Controllers
{

    [ApiController]
    [Route("{userId}/api/[controller]")]
    public class ProjectController(IMediator mediator) : ControllerBase
    {
        Random random = new Random();


        [HttpGet("list")]
        public async Task<ActionResult<List<ProjectModel>>> GetListProjects(int userId)
        {
            try
            {
                List<ProjectModel> result = new List<ProjectModel>();

                var projects = await mediator.Send(new GetProjectsQuery() { UserId = userId });

                foreach (var project in projects)
                {
                    result.Add(new ProjectModel
                    {
                        ProjectId = project.ID,
                        ProjectName = project.ProjectName,
                        ProjectLeaderName = mediator.Send(new GetUserByIdQuery() { UserId = project.ProjectLeadId.Value }).Result.FirstName,
                        Sprints = mediator.Send(new GetSprintsQuery { ProjectId = project.ID }).Result.Select(s => new SprintModel
                        {
                            DateEnd = s.DateEnd,
                            DateStart = s.DateStart,
                            SprintId = s.ID,
                            Tasks = mediator.Send(new GetProjectTasksQuery { ProjectId = project.ID }).Result.Where(p => p.SprintId == s.ID).Select(pt => new ProjectTaskModel
                            {
                                Details = pt.Detail,
                                Title = pt.Title,
                                ExecutorTag = pt.ExecutorId != null ? mediator.Send(new GetUserByIdQuery { UserId = pt.ExecutorId.Value }).Result.Tag : null,
                                Status = pt.Status.Value,
                                ExecutorName = pt.ExecutorId != null ? mediator.Send(new GetUserByIdQuery { UserId = pt.ExecutorId.Value }).Result.FirstName : null,
                                ProjectTaskId = pt.ID
                            }).ToList()
                        }).ToList(),
                        UserRole = project.ProjectLeadId == userId ? (int)UserRole.LEAD : (int)UserRole.EMPLOYEE,
                        Users = mediator.Send(new GetUsersByProjectQuery { ProjectId = project.ID }).Result.Select(u => new UserModel
                        {
                            Email = u.Email,
                            UserTag = u.Tag,
                            FirstName = u.FirstName,
                            SecondName = u.SecondName,
                            LastName = u.LastName,
                            PhoneNumber = u.PhoneNumber,
                            ColorNumber = random.Next(5)

                        }).ToList(),
                        Tasks = mediator.Send(new GetProjectTasksQuery { ProjectId = project.ID }).Result.Select(pt => new ProjectTaskModel
                        {
                            Details = pt.Detail,
                            Title = pt.Title,
                            ExecutorTag = pt.ExecutorId != null ? mediator.Send(new GetUserByIdQuery { UserId = pt.ExecutorId.Value }).Result.Tag : null,
                            Status = pt.Status.Value,
                            ExecutorName = pt.ExecutorId != null ? mediator.Send(new GetUserByIdQuery { UserId = pt.ExecutorId.Value }).Result.FirstName : "",
                            ProjectTaskId = pt.ID
                        }).ToList()

                    });

                }

                return Ok(result);

            }
            catch
            {
                throw new Exception();
            }
        }

    }
}
