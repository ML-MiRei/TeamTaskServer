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


        [HttpPost("{projectId}/add-team")]

        public async Task<ActionResult> AddTeamInProject(int projectId, [FromBody] string teamTag)
        {
            try
            {
                await mediator.Send(new AddTeamInProjectCommand() { ProjectId = projectId, TeamTag = teamTag });
                return Ok();
            }
            catch
            {
                throw new Exception();
            }
        }


        [HttpPost("{projectId}/add-user")]
        public async Task<ActionResult> AddUserInProject(int projectId, [FromBody] string userTag)
        {
            try
            {
                await mediator.Send(new AddUserInProjectCommand() { ProjectId = projectId, UserTag = userTag });
                return Ok();
            }
            catch
            {
                throw new Exception();
            }
        }


        [HttpPost("create")]
        public async Task<ActionResult<ProjectModel>> CreateProject(int userId, [FromBody] string name)
        {
            try
            {
                var reply = await mediator.Send(new CreateProjectCommand() { Name = name, UserId = userId });

                var creator = mediator.Send(new GetUserByIdQuery { UserId = reply.ProjectLeadId.Value }).Result;

                return Ok(new ProjectModel
                {
                    ProjectId = reply.ID,
                    ProjectLeaderName = creator.FirstName,
                    ProjectName = reply.ProjectName,
                    Sprints = new List<SprintModel>(),
                    Tasks = new List<ProjectTaskModel>(),
                    UserRole = (int)UserRole.LEAD,
                    Users = new List<UserModel> { new UserModel
                    {
                    Email = creator.Email,
                    FirstName = creator.FirstName,
                    LastName = creator.LastName,
                    SecondName = creator.SecondName,
                    PhoneNumber = creator.PhoneNumber,
                    UserTag = creator.Tag
                    } }
                });
            }
            catch
            {
                throw new Exception();
            }
        }


        [HttpDelete("{projectId}/delete")]
        public async Task<ActionResult> DeleteProject(int projectId)
        {
            try
            {
                await mediator.Send(new DeleteProjectCommand() { ProjectId = projectId });
                return Ok();
            }
            catch
            {
                throw new Exception();
            }
        }


        [HttpDelete("{projectId}/delete-user/{userTag}")]
        public async Task<ActionResult> DeleteUserFromProject(int projectId, string userTag)
        {
            try
            {
                await mediator.Send(new DeleteUserFromProjectCommand() { ProjectId = projectId, UserTag = userTag });
                return Ok();
            }
            catch
            {
                throw new Exception();
            }
        }


        [HttpDelete("{projectId}/leave")]
        public async Task<ActionResult> LeaveFromProject(int projectId, int userId)
        {
            try
            {
                await mediator.Send(new LeaveFromProjectCommand() { ProjectId = projectId, UserId = userId });
                return Ok();
            }
            catch
            {
                throw new Exception();
            }
        }


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
                                IsUserExecutor = pt.UserId != null && pt.UserId == userId ? true : false,
                                Status = pt.Status,
                                ExecutorName = pt.UserId != null ? mediator.Send(new GetUserByIdQuery { UserId = pt.UserId.Value }).Result.FirstName : "",
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
                            IsUserExecutor = pt.UserId != null && pt.UserId == userId ? true : false,
                            Status = pt.Status,
                            ExecutorName = pt.UserId != null ? mediator.Send(new GetUserByIdQuery { UserId = pt.UserId.Value }).Result.FirstName : "",
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


        [HttpPatch("{projectId}/update")]

        public async Task<ActionResult> UpdateProject(int projectId, [FromBody] ProjectEntity projectEntity)
        {
            try
            {
                await mediator.Send(new UpdateProjectCommand() { ProjectId = projectId, ProjectLeadTag = projectEntity.ProjectLeadTag, Name = projectEntity.ProjectName });
                return Ok();
            }
            catch
            {
                throw new Exception();
            }
        }
    }
}
