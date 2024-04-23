using Getaway.Application.CQRS.Notification.Commands.CreateNotification;
using Getaway.Application.CQRS.Project.Commands.AddTeamInProject;
using Getaway.Application.CQRS.Project.Commands.AddUserInProject;
using Getaway.Application.CQRS.Project.Commands.DeleteProject;
using Getaway.Application.CQRS.Project.Commands.DeleteUserFromProject;
using Getaway.Application.CQRS.Project.Commands.LeaveFromProject;
using Getaway.Application.CQRS.Project.Commands.UpdateProject;
using Getaway.Application.CQRS.Project.Queries.GetProject;
using Getaway.Application.CQRS.Project.Queries.GetProjects;
using Getaway.Application.CQRS.Project.Queries.GetUsersByProject;
using Getaway.Application.CQRS.ProjectTask.Commands.ChangeStatusProjectTask;
using Getaway.Application.CQRS.ProjectTask.Commands.CreateProjectTask;
using Getaway.Application.CQRS.ProjectTask.Commands.DeleteProjectTask;
using Getaway.Application.CQRS.ProjectTask.Commands.SetExecutorProjectTask;
using Getaway.Application.CQRS.ProjectTask.Commands.UpdateProjectTask;
using Getaway.Application.CQRS.ProjectTask.Queries.GetProjectTasks;
using Getaway.Application.CQRS.Sprint.Commands.CreateSprint;
using Getaway.Application.CQRS.Sprint.Commands.DeleteSprint;
using Getaway.Application.CQRS.Sprint.Commands.GetSprint;
using Getaway.Application.CQRS.Sprint.Commands.GetSprints;
using Getaway.Application.CQRS.Sprint.Commands.UpdateDateEndSprint;
using Getaway.Application.CQRS.Sprint.Commands.UpdateDateStartSprint;
using Getaway.Application.CQRS.Team.Queries.GetTeam;
using Getaway.Application.CQRS.Team.Queries.GetTeams;
using Getaway.Application.CQRS.Team.Queries.GetUsersByTeam;
using Getaway.Application.CQRS.User.Queries.GetUserById;
using Getaway.Application.CQRS.User.Queries.GetUserByTag;
using Getaway.Application.ReturnsModels;
using Getaway.Core.Entities;
using Getaway.Core.Enums;
using MediatR;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using System;

namespace Getaway.Presentation.Hubs
{
    public class ProjectHub : Hub
    {
        private const string GROUP_PROJECT_PREFIX = "project_";
        private static IMediator _mediator;
        private static UserConnections _userConnections = new UserConnections();

        public ProjectHub(IMediator mediator)
        {
            _mediator = mediator;
        }


        public async Task ConnectUserWithProjects(int userId, string userTag)
        {
            var projects = await _mediator.Send(new GetProjectsQuery { UserId = userId });
            foreach (var project in projects)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, GROUP_PROJECT_PREFIX + project.ID);
            }

            _userConnections.ListUserConnections.Add(new UserConnection { ConnectionId = Context.ConnectionId, Id = userId, Tag = userTag });

            await Console.Out.WriteLineAsync(Context.ConnectionId);
            await Groups.AddToGroupAsync(Context.ConnectionId, "user_" + userId);

        }


        public override Task OnDisconnectedAsync(Exception? exception)
        {
            base.OnDisconnectedAsync(exception);
            _userConnections.RemoveUserConnectionId(Context.ConnectionId);
            return Task.CompletedTask;
        }

        #region project


        public async Task AddUserInProject(int projectId, string userTag)
        {
            try
            {
                await _mediator.Send(new AddUserInProjectCommand() { ProjectId = projectId, UserTag = userTag });
                var newUser = await _mediator.Send(new GetUserByTagQuery { UserTag = userTag });


                await _mediator.Send(new CreateNotificationCommand
                {
                    Details = "You have been added to a new chat",
                    Title = "Chats",
                    UserId = new List<int> { newUser.ID }
                });



                await _mediator.Send(new CreateNotificationCommand
                {
                    Details = "A new user has been added to the chat",
                    Title = "Chats",
                    UserId = (await _mediator.Send(new GetUsersByProjectQuery { ProjectId = projectId })).Where(u => u.ID != newUser.ID)
                        .Select(u => u.ID)
                        .ToList()

                });


                var project = await _mediator.Send(new GetProjectQuery { ProjectId = projectId });


                if (_userConnections.IsConnectedUser(userTag))
                {
                    await Clients.Group("user_" + newUser.ID).SendAsync("NewProjectReply",
                 new ProjectModel
                 {
                     ProjectId = project.ID,
                     ProjectName = project.ProjectName,
                     ProjectLeaderName = _mediator.Send(new GetUserByIdQuery() { UserId = project.ProjectLeadId.Value }).Result.FirstName,
                     Sprints = _mediator.Send(new GetSprintsQuery { ProjectId = project.ID }).Result.Select(s => new SprintModel
                     {
                         DateEnd = s.DateEnd,
                         DateStart = s.DateStart,
                         SprintId = s.ID,
                         Tasks = _mediator.Send(new GetProjectTasksQuery { ProjectId = project.ID }).Result.Where(p => p.SprintId == s.ID).Select(pt => new ProjectTaskModel
                         {
                             Details = pt.Detail,
                             Title = pt.Title,
                             IsUserExecutor = pt.UserId != null && pt.UserId == _userConnections.GetUserId(userTag) ? true : false,
                             Status = pt.Status,
                             ExecutorName = pt.UserId != null ? _mediator.Send(new GetUserByIdQuery { UserId = pt.UserId.Value }).Result.FirstName : "",
                             ProjectTaskId = pt.ID
                         }).ToList()
                     }).ToList(),
                     UserRole = project.ProjectLeadId == _userConnections.GetUserId(userTag) ? (int)UserRole.LEAD : (int)UserRole.EMPLOYEE,
                     Users = _mediator.Send(new GetUsersByProjectQuery { ProjectId = project.ID }).Result.Select(u => new UserModel
                     {
                         Email = u.Email,
                         UserTag = u.Tag,
                         FirstName = u.FirstName,
                         SecondName = u.SecondName,
                         LastName = u.LastName,
                         PhoneNumber = u.PhoneNumber,
                         ColorNumber = 4

                     }).ToList(),
                     Tasks = _mediator.Send(new GetProjectTasksQuery { ProjectId = project.ID }).Result.Select(pt => new ProjectTaskModel
                     {
                         Details = pt.Detail,
                         Title = pt.Title,
                         IsUserExecutor = pt.UserId != null && pt.UserId == _userConnections.GetUserId(userTag) ? true : false,
                         Status = pt.Status,
                         ExecutorName = pt.UserId != null ? _mediator.Send(new GetUserByIdQuery { UserId = pt.UserId.Value }).Result.FirstName : "",
                         ProjectTaskId = pt.ID
                     }).ToList()
                 }

                   );

                }


                await Clients.Group(GROUP_PROJECT_PREFIX + projectId).SendAsync("AddUserInProjectReply",
                                  projectId,
                                  new UserModel
                                  {
                                      Email = newUser.Email,
                                      SecondName = newUser.SecondName,
                                      FirstName = newUser.FirstName,
                                      LastName = newUser.FirstName,
                                      PhoneNumber = newUser.PhoneNumber,
                                      UserTag = userTag,
                                      ColorNumber = 1
                                  }
                                  );

                if (_userConnections.IsConnectedUser(userTag))
                    await Groups.AddToGroupAsync(_userConnections.GetUserConnectionId(newUser.ID), GROUP_PROJECT_PREFIX + projectId);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }



        public async Task DeleteProject(int projectId)
        {
            try
            {
                await _mediator.Send(new DeleteProjectCommand() { ProjectId = projectId });

                var project = await _mediator.Send(new GetProjectQuery { ProjectId = projectId });

                await _mediator.Send(new CreateNotificationCommand
                {
                    Details = "A new user has been added to the chat",
                    Title = "Chats",
                    UserId = (await _mediator.Send(new GetUsersByProjectQuery { ProjectId = projectId })).Where(u => u.ID != project.ProjectLeadId)
                        .Select(u => u.ID)
                        .ToList()

                });


                await Clients.Group(GROUP_PROJECT_PREFIX + projectId).SendAsync("DeleteProjectReply", projectId);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }



        public async Task DeleteUserFromProject(int projectId, string userTag)
        {
            try
            {
                await _mediator.Send(new DeleteUserFromProjectCommand() { ProjectId = projectId, UserTag = userTag });

                await _mediator.Send(new CreateNotificationCommand
                {
                    Details = "A new user has been added to the chat",
                    Title = "Chats",
                    UserId = (await _mediator.Send(new GetUsersByProjectQuery { ProjectId = projectId })).Where(u => u.Tag != userTag)
                   .Select(u => u.ID)
                   .ToList()

                });


                if (_userConnections.IsConnectedUser(userTag))
                {

                    await Clients.Group("user_" + _userConnections.GetUserId(userTag)).SendAsync("DeleteProjectReply", projectId);
                    await Groups.RemoveFromGroupAsync(_userConnections.GetUserConnectionId(userTag), GROUP_PROJECT_PREFIX + projectId);
                }


                await Clients.Group(GROUP_PROJECT_PREFIX + projectId).SendAsync("DeleteUserFromProjectReply", projectId, userTag);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }



        public async Task LeaveFromProject(int projectId, int userId)
        {
            try
            {
                await _mediator.Send(new LeaveFromProjectCommand() { ProjectId = projectId, UserId = userId });
                await _mediator.Send(new CreateNotificationCommand
                {
                    Details = "A new user has been added to the chat",
                    Title = "Chats",
                    UserId = (await _mediator.Send(new GetUsersByProjectQuery { ProjectId = projectId })).Where(u => u.ID != userId)
                  .Select(u => u.ID)
                  .ToList()

                });


                var user = await _mediator.Send(new GetUserByIdQuery { UserId = userId });

                if (_userConnections.IsConnectedUser(userId))
                {

                    await Clients.Group("user_" + userId).SendAsync("DeleteProjectReply", projectId);
                    await Groups.RemoveFromGroupAsync(_userConnections.GetUserConnectionId(userId), GROUP_PROJECT_PREFIX + projectId);
                }


                await Clients.Group(GROUP_PROJECT_PREFIX + projectId).SendAsync("DeleteUserFromProjectReply", projectId, user.Tag);


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }



        public async Task UpdateProject(ProjectEntity projectEntity)
        {
            try
            {
                await _mediator.Send(new UpdateProjectCommand() { ProjectId = projectEntity.ID, ProjectLeadTag = projectEntity.ProjectLeadTag, Name = projectEntity.ProjectName });

                var project = await _mediator.Send(new GetProjectQuery { ProjectId = projectEntity.ID });


                await _mediator.Send(new CreateNotificationCommand
                {
                    Details = "A new user has been added to the chat",
                    Title = "Chats",
                    UserId = (await _mediator.Send(new GetUsersByProjectQuery { ProjectId = projectEntity.ID })).Where(u => u.ID != project.ProjectLeadId)
                 .Select(u => u.ID)
                 .ToList()

                });



                await Clients.Group(GROUP_PROJECT_PREFIX + projectEntity.ID).SendAsync("UpdateProjectReply",
                    new ProjectModel
                    {
                        ProjectName = project.ProjectName,
                        ProjectLeaderName = (await _mediator.Send(new GetUserByIdQuery { UserId = project.ProjectLeadId.Value })).FirstName,
                        UserRole = _userConnections.GetUserTag(Context.ConnectionId) == projectEntity.ProjectLeadTag ? (int)UserRole.LEAD : (int)UserRole.EMPLOYEE
                    }

                    );

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        #endregion

        #region project_task



        public async Task UpdateProjectTask(ProjectTaskEntity projectTaskEntity)
        {
            try
            {
                await _mediator.Send(new UpdateProjectTaskCommand() { ProjectTaskId = projectTaskEntity.ID, Title = projectTaskEntity.Title, Details = projectTaskEntity.Detail });


                var project = await _mediator.Send(new GetProjectQuery { ProjectId = projectTaskEntity.ProjectId.Value });


                await _mediator.Send(new CreateNotificationCommand
                {
                    Details = "A new user has been added to the chat",
                    Title = "Chats",
                    UserId = (await _mediator.Send(new GetUsersByProjectQuery { ProjectId = project.ID })).Where(u => u.ID != project.ProjectLeadId)
                 .Select(u => u.ID)
                 .ToList()

                });



                await Clients.Group(GROUP_PROJECT_PREFIX + project.ID).SendAsync("UpdateProjectTaskReply",
                    new ProjectTaskModel
                    {
                        Details = projectTaskEntity.Detail,
                        Title = projectTaskEntity.Title,
                        ProjectTaskId = projectTaskEntity.ID
                    }
                    );
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        public async Task ChangeStatusProjectTask(ProjectTaskEntity projectTaskEntity)
        {
            try
            {
                await _mediator.Send(new ChangeStatusProjectTaskCommand() { ProjectTaskId = projectTaskEntity.ID, Status = projectTaskEntity.Status });

                var project = await _mediator.Send(new GetProjectQuery { ProjectId = projectTaskEntity.ProjectId.Value });

                await _mediator.Send(new CreateNotificationCommand
                {
                    Details = "The task status has changed",
                    Title = "Project task",
                    UserId = (await _mediator.Send(new GetUsersByProjectQuery { ProjectId = project.ID })).Where(u => u.ID != project.ProjectLeadId)
                        .Select(u => u.ID)
                        .ToList()
                });


                await Clients.Group(GROUP_PROJECT_PREFIX + project.ID).SendAsync("ChangeStatusProjectTaskReply",
                     new ProjectTaskModel
                     {
                         ProjectTaskId = projectTaskEntity.ID,
                         Status = projectTaskEntity.Status,
                     }
                     );

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }



        public async Task SetExecutorProjectTask(int projectId, int projectTaskId, string userTag)
        {
            try
            {
                await _mediator.Send(new SetExecutorProjectTaskCommand() { ProjectTaskId = projectTaskId, UserTag = userTag });

                var user = await _mediator.Send(new GetUserByTagQuery { UserTag = userTag });


                await _mediator.Send(new CreateNotificationCommand
                {
                    Details = "The task status has changed",
                    Title = "Project task",
                    UserId = new List<int> { (await _mediator.Send(new GetUserByTagQuery { UserTag = userTag })).ID }
                });


                await Clients.Group(GROUP_PROJECT_PREFIX + projectId).SendAsync("SetExecutorProjectTaskReply",
                     new ProjectTaskModel
                     {
                         ProjectTaskId = projectTaskId,
                         ExecutorName = user.FirstName,
                         IsUserExecutor = _userConnections.GetUserTag(Context.ConnectionId) == userTag
                     }
                     );

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }



        public async Task DeleteProjectTask(int projectId, int projectTaskId)
        {
            try
            {
                await _mediator.Send(new DeleteProjectTaskCommand() { ProjectTaskId = projectTaskId });

                var project = await _mediator.Send(new GetProjectQuery { ProjectId = projectId });


                await _mediator.Send(new CreateNotificationCommand
                {
                    Details = "The task status has changed",
                    Title = "Project task",
                    UserId = (await _mediator.Send(new GetUsersByProjectQuery { ProjectId = project.ID })).Where(u => u.ID != project.ProjectLeadId)
                        .Select(u => u.ID)
                        .ToList()
                });


                await Clients.Group(GROUP_PROJECT_PREFIX + projectId).SendAsync("DeleteProjectTaskReply", projectTaskId);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }



        public async Task CreateProjectTask(ProjectTaskEntity projectTaskEntity)
        {
            try
            {
                var projectTask = await _mediator.Send(new CreateProjectTaskCommand
                {
                    Details = projectTaskEntity.Detail,
                    ProjectId = projectTaskEntity.ProjectId.Value,
                    Title = projectTaskEntity.Title,
                    SprintId = projectTaskEntity.SprintId,
                    Status = projectTaskEntity.Status
                });


                var project = await _mediator.Send(new GetProjectQuery { ProjectId = projectTaskEntity.ProjectId.Value });


                await _mediator.Send(new CreateNotificationCommand
                {
                    Details = "The task status has changed",
                    Title = "Project task",
                    UserId = (await _mediator.Send(new GetUsersByProjectQuery { ProjectId = project.ID })).Where(u => u.ID != project.ProjectLeadId)
                        .Select(u => u.ID)
                        .ToList()
                });



                await Clients.Group(GROUP_PROJECT_PREFIX + projectTaskEntity.ProjectId).SendAsync("AddNewProjectTaskReply", new ProjectTaskModel
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


        #endregion


        #region sprints


        public async Task UpdateDateStartSprint(int projectId, int sprintId, DateTime dateStart)
        {
            try
            {
                await _mediator.Send(new UpdateDateStartSprintCommand() { SprintId = sprintId, DateStart = dateStart });

                var project = await _mediator.Send(new GetProjectQuery { ProjectId = projectId });


                await _mediator.Send(new CreateNotificationCommand
                {
                    Details = "The task status has changed",
                    Title = "Project task",
                    UserId = (await _mediator.Send(new GetUsersByProjectQuery { ProjectId = project.ID })).Where(u => u.ID != project.ProjectLeadId)
                        .Select(u => u.ID)
                        .ToList()
                });


                await Clients.Group(GROUP_PROJECT_PREFIX + projectId).SendAsync("UpdateDateStartSprintReply", new SprintEntity
                {
                    DateStart = dateStart,
                    ID = sprintId,
                    ProjectId = projectId
                });

            }
            catch
            {
                throw new Exception();
            }
        }


        public async Task UpdateDateEndSprint(int projectId, int sprintId, DateTime dateEnd)
        {
            try
            {
                await _mediator.Send(new UpdateDateEndSprintCommand() { SprintId = sprintId, DateEnd = dateEnd });
                var project = await _mediator.Send(new GetProjectQuery { ProjectId = projectId });


                await _mediator.Send(new CreateNotificationCommand
                {
                    Details = "The task status has changed",
                    Title = "Project task",
                    UserId = (await _mediator.Send(new GetUsersByProjectQuery { ProjectId = project.ID })).Where(u => u.ID != project.ProjectLeadId)
                        .Select(u => u.ID)
                        .ToList()
                });


                await Clients.Group(GROUP_PROJECT_PREFIX + projectId).SendAsync("UpdateDateEndSprintReply", new SprintEntity
                {
                    DateEnd = dateEnd,
                    ID = sprintId,
                    ProjectId = projectId
                });
            }
            catch
            {
                throw new Exception();
            }
        }



        public async Task CreateSprint(SprintEntity sprintEntity)
        {
            try
            {
                var sprint = await _mediator.Send(new CreateSprintCommand() { ProjectId = sprintEntity.ProjectId, DateEnd = sprintEntity.DateEnd, DateStart = sprintEntity.DateStart });

                var project = await _mediator.Send(new GetProjectQuery { ProjectId = sprintEntity.ProjectId });


                await _mediator.Send(new CreateNotificationCommand
                {
                    Details = "The task status has changed",
                    Title = "Project task",
                    UserId = (await _mediator.Send(new GetUsersByProjectQuery { ProjectId = project.ID })).Where(u => u.ID != project.ProjectLeadId)
                        .Select(u => u.ID)
                        .ToList()
                });


                await Clients.Group(GROUP_PROJECT_PREFIX + sprintEntity.ProjectId).SendAsync("CreateSprintReply", new SprintModel
                {
                    DateEnd = sprint.DateEnd,
                    DateStart = sprint.DateStart,
                    SprintId = sprint.ID,
                    Tasks = new List<ProjectTaskModel>()
                });

            }
            catch
            {
                throw new Exception();
            }
        }


        public async Task DeleteSprint(int projectId, int sprintId)
        {
            try
            {
                await _mediator.Send(new DeleteSprintCommand() { SprintId = sprintId });

                var project = await _mediator.Send(new GetProjectQuery { ProjectId = projectId });

                await _mediator.Send(new CreateNotificationCommand
                {
                    Details = "The task status has changed",
                    Title = "Project task",
                    UserId = (await _mediator.Send(new GetUsersByProjectQuery { ProjectId = project.ID })).Where(u => u.ID != project.ProjectLeadId)
                        .Select(u => u.ID)
                        .ToList()
                });


                await Clients.Group(GROUP_PROJECT_PREFIX + projectId).SendAsync("DeleteSprintReply", projectId, sprintId);
            }
            catch
            {
                throw new Exception();
            }
        }


        #endregion

    }
}
