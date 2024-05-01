using Getaway.Application.CQRS.Notification.Commands.CreateNotification;
using Getaway.Application.CQRS.Project.Commands.AddTeamInProject;
using Getaway.Application.CQRS.Project.Commands.AddUserInProject;
using Getaway.Application.CQRS.Project.Commands.CreateProject;
using Getaway.Application.CQRS.Project.Commands.DeleteProject;
using Getaway.Application.CQRS.Project.Commands.DeleteUserFromProject;
using Getaway.Application.CQRS.Project.Commands.LeaveFromProject;
using Getaway.Application.CQRS.Project.Commands.UpdateProject;
using Getaway.Application.CQRS.Project.Queries.GetProject;
using Getaway.Application.CQRS.Project.Queries.GetProjects;
using Getaway.Application.CQRS.Project.Queries.GetUsersByProject;
using Getaway.Application.CQRS.ProjectTask.Commands.AddInSprintProjectTask;
using Getaway.Application.CQRS.ProjectTask.Commands.ChangeStatusProjectTask;
using Getaway.Application.CQRS.ProjectTask.Commands.CreateProjectTask;
using Getaway.Application.CQRS.ProjectTask.Commands.DeleteProjectTask;
using Getaway.Application.CQRS.ProjectTask.Commands.SetExecutorProjectTask;
using Getaway.Application.CQRS.ProjectTask.Commands.UpdateProjectTask;
using Getaway.Application.CQRS.ProjectTask.Queries.GetProjectTasks;
using Getaway.Application.CQRS.Sprint.Commands.CreateSprint;
using Getaway.Application.CQRS.Sprint.Commands.DeleteSprint;
using Getaway.Application.CQRS.Sprint.Commands.GetSprints;
using Getaway.Application.CQRS.Sprint.Commands.UpdateDateEndSprint;
using Getaway.Application.CQRS.Sprint.Commands.UpdateDateStartSprint;
using Getaway.Application.CQRS.Team.Queries.GetTeam;
using Getaway.Application.CQRS.Team.Queries.GetTeamByTag;
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

namespace Getaway.Presentation.Hubs
{
    public class ProjectHub : Hub
    {
        private const string GROUP_PROJECT_PREFIX = "project_";
        private const string USER_PREFIX = "user_";
        private const string NOTIFICATION_PROJECT_PREFIX = "Projects";
        private const string NOTIFICATION_SPRINT_PREFIX = "Sprints";
        private const string NOTIFICATION_PROJECT_TASK_PREFIX = "Project tasks";

        private static IMediator _mediator;
        private static ILogger<ProjectHub> _logger;

        private static UserConnections _userConnections = new UserConnections();


        public ProjectHub(ILogger<ProjectHub> logger, IMediator mediator)
        {
            _mediator = mediator;
            _logger = logger;
        }


        public async Task ConnectUserWithProjects(int userId, string userTag)
        {
            var projects = await _mediator.Send(new GetProjectsQuery { UserId = userId });
            foreach (var project in projects)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, GROUP_PROJECT_PREFIX + project.ID);
            }

            _userConnections.ListUserConnections.Add(new UserConnection { ConnectionId = Context.ConnectionId, Id = userId, Tag = userTag });
            await Groups.AddToGroupAsync(Context.ConnectionId, USER_PREFIX + userId);
        }
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            base.OnDisconnectedAsync(exception);
            _userConnections.RemoveUserConnectionId(Context.ConnectionId);
            return Task.CompletedTask;
        }



        #region project

        public async Task CreateProject(int userId, string name)
        {
            try
            {
                var reply = await _mediator.Send(new CreateProjectCommand() { Name = name, UserId = userId });
                var creator = await _mediator.Send(new GetUserByIdQuery { UserId = reply.ProjectLeadId.Value });

                await Clients.Group(USER_PREFIX + userId).SendAsync("NewProjectReply", new ProjectModel
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

                await Groups.AddToGroupAsync(Context.ConnectionId, GROUP_PROJECT_PREFIX + reply.ID);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
        public async Task DeleteProject(int projectId)
        {
            try
            {
                await _mediator.Send(new DeleteProjectCommand() { ProjectId = projectId });

                var notification = await _mediator.Send(new CreateNotificationCommand
                {
                    Details = "One of the projects has been deleted",
                    Title = NOTIFICATION_PROJECT_PREFIX,
                    UserId = (await _mediator.Send(new GetUsersByProjectQuery { ProjectId = projectId })).Where(u => u.Tag != _userConnections.GetUserTag(Context.ConnectionId))
                        .Select(u => u.ID)
                        .ToList()

                });

                await Clients.Group(GROUP_PROJECT_PREFIX + projectId).SendAsync("DeleteProjectReply", projectId);
                await Clients.OthersInGroup(GROUP_PROJECT_PREFIX + projectId).SendAsync("NotificationReply", new NotificationModel
                {
                    Details = notification.Detail,
                    Title = notification.Title,
                    NotificationId = notification.ID
                });

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public async Task AddUserInProject(int projectId, string userTag)
        {
            try
            {
                await _mediator.Send(new AddUserInProjectCommand() { ProjectId = projectId, UserTag = userTag });

                var newUser = await _mediator.Send(new GetUserByTagQuery { UserTag = userTag });
                var project = await _mediator.Send(new GetProjectQuery { ProjectId = projectId });

                var notificationForOne = await _mediator.Send(new CreateNotificationCommand
                {
                    Details = "You have been added to the project",
                    Title = NOTIFICATION_PROJECT_PREFIX,
                    UserId = new List<int> { newUser.ID }
                });
                var notificationForAll = await _mediator.Send(new CreateNotificationCommand
                {
                    Details = "A new user has been added to the project",
                    Title = NOTIFICATION_PROJECT_PREFIX,
                    UserId = (await _mediator.Send(new GetUsersByProjectQuery { ProjectId = projectId })).Where(u => u.ID != newUser.ID)
                        .Select(u => u.ID)
                        .ToList()
                });

                await Clients.Group(GROUP_PROJECT_PREFIX + projectId).SendAsync("AddUserInProjectReply", projectId, new UserModel
                {
                    Email = newUser.Email,
                    SecondName = newUser.SecondName,
                    FirstName = newUser.FirstName,
                    LastName = newUser.FirstName,
                    PhoneNumber = newUser.PhoneNumber,
                    UserTag = userTag,
                    ColorNumber = 1
                });
                await Clients.OthersInGroup(GROUP_PROJECT_PREFIX + projectId).SendAsync("NotificationReply", new NotificationModel
                {
                    Details = notificationForAll.Detail,
                    Title = notificationForAll.Title,
                    NotificationId = notificationForAll.ID
                });

                if (_userConnections.IsConnectedUser(userTag))
                {
                    await Groups.AddToGroupAsync(_userConnections.GetUserConnectionId(userTag), GROUP_PROJECT_PREFIX + projectId);
                    await Clients.Group(USER_PREFIX + newUser.ID).SendAsync("NewProjectReply", new ProjectModel
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
                                ExecutorTag = pt.ExecutorTag,
                                Status = pt.Status.Value,
                                ExecutorName = pt.ExecutorId != null ? _mediator.Send(new GetUserByIdQuery { UserId = pt.ExecutorId.Value }).Result.FirstName : "",
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
                            ExecutorTag = pt.ExecutorTag,
                            Status = pt.Status.Value,
                            ExecutorName = pt.ExecutorId != null ? _mediator.Send(new GetUserByIdQuery { UserId = pt.ExecutorId.Value }).Result.FirstName : "",
                            ProjectTaskId = pt.ID
                        }).ToList()
                    });
                    await Clients.Group(USER_PREFIX + newUser.ID).SendAsync("NotificationReply", new NotificationModel
                    {
                        Details = notificationForOne.Detail,
                        Title = notificationForOne.Title,
                        NotificationId = notificationForOne.ID
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
        public async Task DeleteUserFromProject(int projectId, string userTag)
        {
            try
            {
                await _mediator.Send(new DeleteUserFromProjectCommand() { ProjectId = projectId, UserTag = userTag });

                var kickedUser = await _mediator.Send(new GetUserByTagQuery { UserTag = userTag });


                var notificationForAll = await _mediator.Send(new CreateNotificationCommand
                {
                    Details = $"User {kickedUser.FirstName} has been removed from the project",
                    Title = NOTIFICATION_PROJECT_PREFIX,
                    UserId = (await _mediator.Send(new GetUsersByProjectQuery { ProjectId = projectId })).Where(u => u.Tag != userTag)
                   .Select(u => u.ID)
                   .ToList()

                });
                var notificationForOne = await _mediator.Send(new CreateNotificationCommand
                {
                    Details = "You were kicked out of the project",
                    Title = NOTIFICATION_PROJECT_PREFIX,
                    UserId = new List<int> { kickedUser.ID }
                });


                if (_userConnections.IsConnectedUser(userTag))
                {
                    await Groups.RemoveFromGroupAsync(_userConnections.GetUserConnectionId(userTag), GROUP_PROJECT_PREFIX + projectId);
                    await Clients.Group(USER_PREFIX + _userConnections.GetUserId(userTag)).SendAsync("DeleteProjectReply", projectId);
                    await Clients.Group(userTag + kickedUser.ID).SendAsync("NotificationReply", new NotificationModel
                    {
                        Details = notificationForOne.Detail,
                        Title = notificationForOne.Title,
                        NotificationId = notificationForOne.ID
                    });
                }

                await Clients.Group(GROUP_PROJECT_PREFIX + projectId).SendAsync("DeleteUserFromProjectReply", projectId, userTag);
                await Clients.OthersInGroup(GROUP_PROJECT_PREFIX + projectId).SendAsync("NotificationReply", new NotificationModel
                {
                    Details = notificationForAll.Detail,
                    Title = notificationForAll.Title,
                    NotificationId = notificationForAll.ID
                });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
        public async Task LeaveFromProject(int projectId, int userId)
        {
            try
            {
                await _mediator.Send(new LeaveFromProjectCommand() { ProjectId = projectId, UserId = userId });

                var user = await _mediator.Send(new GetUserByIdQuery { UserId = userId });
                var users = (await _mediator.Send(new GetUsersByProjectQuery { ProjectId = projectId })).Where(u => u.ID != userId).Select(u => u.ID).ToList();

                await Clients.Group(USER_PREFIX + userId).SendAsync("DeleteProjectReply", projectId);
                await Groups.RemoveFromGroupAsync(_userConnections.GetUserConnectionId(userId), GROUP_PROJECT_PREFIX + projectId);

                if (users?.Count > 0)
                {
                    var notificationForAll = await _mediator.Send(new CreateNotificationCommand
                    {
                        Details = $"The user {user.FirstName} has left the chat",
                        Title = NOTIFICATION_PROJECT_PREFIX,
                        UserId = users
                    });
                    await Clients.OthersInGroup(GROUP_PROJECT_PREFIX + projectId).SendAsync("DeleteUserFromProjectReply", projectId, user.Tag);
                    await Clients.OthersInGroup(GROUP_PROJECT_PREFIX + projectId).SendAsync("NotificationReply", new NotificationModel
                    {
                        Details = notificationForAll.Detail,
                        Title = notificationForAll.Title,
                        NotificationId = notificationForAll.ID
                    });
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
        public async Task UpdateProject(ProjectEntity projectEntity)
        {
            try
            {
                await _mediator.Send(new UpdateProjectCommand() { ProjectId = projectEntity.ID, ProjectLeadTag = projectEntity.ProjectLeadTag, Name = projectEntity.ProjectName });

                var project = await _mediator.Send(new GetProjectQuery { ProjectId = projectEntity.ID });
                var notification = await _mediator.Send(new CreateNotificationCommand
                {
                    Details = "One of the projects has been updated",
                    Title = NOTIFICATION_PROJECT_PREFIX,
                    UserId = (await _mediator.Send(new GetUsersByProjectQuery { ProjectId = projectEntity.ID }))
                                              .Where(u => u.ID != project.ProjectLeadId)
                                              .Select(u => u.ID)
                                              .ToList()

                });

                await Clients.Group(GROUP_PROJECT_PREFIX + projectEntity.ID).SendAsync("UpdateProjectReply", new ProjectEntity
                {
                    ProjectLeadName = (await _mediator.Send(new GetUserByIdQuery { UserId = project.ProjectLeadId.Value })).FirstName,
                    ProjectName = project.ProjectName,
                    ID = project.ID,
                    ProjectLeadTag = projectEntity.ProjectLeadTag
                });
                await Clients.OthersInGroup(GROUP_PROJECT_PREFIX + projectEntity.ID).SendAsync("NotificationReply", new NotificationModel
                {
                    Details = notification.Detail,
                    Title = notification.Title,
                    NotificationId = notification.ID
                });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
        public async Task AddTeamInProject(int projectId, string teamTag)
        {
            try
            {
                Console.WriteLine("start add team");
                await _mediator.Send(new AddTeamInProjectCommand() { ProjectId = projectId, TeamTag = teamTag });

                var team = await _mediator.Send(new GetTeamByTagQuery { TeamTag = teamTag });
                var users = (await _mediator.Send(new GetUsersByTeamQuery { TeamId = team.ID.Value })).ToList();
                var project = await _mediator.Send(new GetProjectQuery { ProjectId = projectId });


                var notificationForTeam = await _mediator.Send(new CreateNotificationCommand
                {
                    Details = "You have been added to the project",
                    Title = NOTIFICATION_PROJECT_PREFIX,
                    UserId = users.Select(u => u.ID).ToList()
                });
                var notificationForAll = await _mediator.Send(new CreateNotificationCommand
                {
                    Details = "A new team has been added to the project",
                    Title = NOTIFICATION_PROJECT_PREFIX,
                    UserId = (await _mediator.Send(new GetUsersByProjectQuery { ProjectId = projectId })).Where(u => !users.Contains(u))
                                             .Select(u => u.ID).ToList()
                });

                await Clients.OthersInGroup(GROUP_PROJECT_PREFIX + projectId).SendAsync("NotificationReply", new NotificationModel
                {
                    Details = notificationForAll.Detail,
                    Title = notificationForAll.Title,
                    NotificationId = notificationForAll.ID
                });


                foreach (var user in users)
                {
                    if (_userConnections.IsConnectedUser(user.ID))
                    {
                        await Groups.AddToGroupAsync(_userConnections.GetUserConnectionId(user.ID), GROUP_PROJECT_PREFIX + projectId);
                        await Clients.Group(USER_PREFIX + user.ID).SendAsync("NewProjectReply", new ProjectModel
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
                                    ExecutorTag = pt.ExecutorTag,
                                    Status = pt.Status.Value,
                                    ExecutorName = pt.ExecutorId != null ? _mediator.Send(new GetUserByIdQuery { UserId = pt.ExecutorId.Value }).Result.FirstName : "",
                                    ProjectTaskId = pt.ID
                                }).ToList()
                            }).ToList(),
                            UserRole = project.ProjectLeadId == _userConnections.GetUserId(user.Tag) ? (int)UserRole.LEAD : (int)UserRole.EMPLOYEE,
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
                                ExecutorTag = pt.ExecutorTag,
                                Status = pt.Status.Value,
                                ExecutorName = pt.ExecutorId != null ? _mediator.Send(new GetUserByIdQuery { UserId = pt.ExecutorId.Value }).Result.FirstName : "",
                                ProjectTaskId = pt.ID
                            }).ToList()
                        });
                        await Clients.Group(USER_PREFIX + user.ID).SendAsync("NotificationReply", new NotificationModel
                        {
                            Details = notificationForTeam.Detail,
                            Title = notificationForTeam.Title,
                            NotificationId = notificationForTeam.ID
                        });
                    }
                    await Clients.Group(GROUP_PROJECT_PREFIX + projectId).SendAsync("AddUserInProjectReply", projectId, new UserModel
                    {
                        Email = user.Email,
                        SecondName = user.SecondName,
                        FirstName = user.FirstName,
                        LastName = user.FirstName,
                        PhoneNumber = user.PhoneNumber,
                        UserTag = user.Tag,
                        ColorNumber = 1
                    });

                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
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
                var notification = await _mediator.Send(new CreateNotificationCommand
                {
                    Details = $"The task in the {project.ProjectName} project has been updated",
                    Title = NOTIFICATION_PROJECT_TASK_PREFIX,
                    UserId = (await _mediator.Send(new GetUsersByProjectQuery { ProjectId = project.ID }))
                                              .Where(u => u.ID != project.ProjectLeadId)
                                              .Select(u => u.ID)
                                              .ToList()
                });

                await Clients.Group(GROUP_PROJECT_PREFIX + projectTaskEntity.ProjectId).SendAsync("UpdateProjectTaskReply", new ProjectTaskEntity
                {
                    Detail = projectTaskEntity.Detail,
                    Title = projectTaskEntity.Title,
                    ID = projectTaskEntity.ID,
                    ProjectId = project.ID
                });
                await Clients.OthersInGroup(GROUP_PROJECT_PREFIX + projectTaskEntity.ProjectId).SendAsync("NotificationReply", new NotificationModel
                {
                    Details = notification.Detail,
                    Title = notification.Title,
                    NotificationId = notification.ID
                });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
        public async Task ChangeStatusProjectTask(ProjectTaskEntity projectTaskEntity)
        {
            try
            {
                await _mediator.Send(new ChangeStatusProjectTaskCommand() { ProjectTaskId = projectTaskEntity.ID, Status = projectTaskEntity.Status.Value });

                var project = await _mediator.Send(new GetProjectQuery { ProjectId = projectTaskEntity.ProjectId.Value });
                var notification = await _mediator.Send(new CreateNotificationCommand
                {
                    Details = "The task status has changed",
                    Title = NOTIFICATION_PROJECT_TASK_PREFIX,
                    UserId = (await _mediator.Send(new GetUsersByProjectQuery { ProjectId = project.ID }))
                                             .Where(u => u.ID != project.ProjectLeadId)
                                             .Select(u => u.ID).ToList()
                });

                await Clients.Group(GROUP_PROJECT_PREFIX + project.ID).SendAsync("ChangeStatusProjectTaskReply", new ProjectTaskEntity
                {
                    ProjectId = projectTaskEntity.ProjectId,
                    SprintId = projectTaskEntity.SprintId,
                    ID = projectTaskEntity.ID,
                    Status = projectTaskEntity.Status,
                });
                await Clients.OthersInGroup(GROUP_PROJECT_PREFIX + projectTaskEntity.ProjectId).SendAsync("NotificationReply", new NotificationModel
                {
                    Details = notification.Detail,
                    Title = notification.Title,
                    NotificationId = notification.ID
                });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
        public async Task SetExecutorProjectTask(int projectId, int projectTaskId, string userTag)
        {
            try
            {
                await _mediator.Send(new SetExecutorProjectTaskCommand() { ProjectTaskId = projectTaskId, UserTag = userTag });

                var user = await _mediator.Send(new GetUserByTagQuery { UserTag = userTag });
                var notification = await _mediator.Send(new CreateNotificationCommand
                {
                    Details = "You have been appointed as the task executor",
                    Title = NOTIFICATION_PROJECT_TASK_PREFIX,
                    UserId = new List<int> { user.ID }
                });

                await Clients.Group(GROUP_PROJECT_PREFIX + projectId).SendAsync("SetExecutorProjectTaskReply", new ProjectTaskEntity
                {
                    ID = projectTaskId,
                    ExecutorName = user.FirstName,
                    ExecutorTag = userTag,
                    ProjectId = projectId
                });
                await Clients.Groups(USER_PREFIX + user.ID).SendAsync("NotificationReply", new NotificationModel
                {
                    Details = notification.Detail,
                    Title = notification.Title,
                    NotificationId = notification.ID
                });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
        public async Task DeleteProjectTask(int projectId, int projectTaskId)
        {
            try
            {
                await _mediator.Send(new DeleteProjectTaskCommand() { ProjectTaskId = projectTaskId });

                var project = await _mediator.Send(new GetProjectQuery { ProjectId = projectId });
                var notification = await _mediator.Send(new CreateNotificationCommand
                {
                    Details = "The task has been deleted",
                    Title = NOTIFICATION_PROJECT_TASK_PREFIX,
                    UserId = (await _mediator.Send(new GetUsersByProjectQuery { ProjectId = project.ID }))
                                             .Where(u => u.ID != project.ProjectLeadId)
                                             .Select(u => u.ID).ToList()
                });

                await Clients.Group(GROUP_PROJECT_PREFIX + projectId).SendAsync("DeleteProjectTaskReply", projectId, projectTaskId);
                await Clients.OthersInGroup(GROUP_PROJECT_PREFIX + projectId).SendAsync("NotificationReply", new NotificationModel
                {
                    Details = notification.Detail,
                    Title = notification.Title,
                    NotificationId = notification.ID
                });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
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
                    Status = projectTaskEntity.Status.Value
                });
                var project = await _mediator.Send(new GetProjectQuery { ProjectId = projectTaskEntity.ProjectId.Value });

                var notification = await _mediator.Send(new CreateNotificationCommand
                {
                    Details = $"A new task has been added to the {project.ProjectName} project",
                    Title = "Project task",
                    UserId = (await _mediator.Send(new GetUsersByProjectQuery { ProjectId = project.ID }))
                                              .Where(u => u.ID != project.ProjectLeadId)
                                              .Select(u => u.ID).ToList()
                });

                await Clients.Group(GROUP_PROJECT_PREFIX + projectTaskEntity.ProjectId).SendAsync("AddNewProjectTaskReply", new ProjectTaskEntity
                {
                    Detail = projectTask.Detail,
                    Title = projectTask.Title,
                    Status = projectTask.Status,
                    ExecutorName = null,
                    ExecutorTag = null,
                    ID = projectTask.ID,
                    ProjectId = projectTaskEntity.ProjectId,
                    SprintId = projectTaskEntity.SprintId

                });
                await Clients.OthersInGroup(GROUP_PROJECT_PREFIX + projectTaskEntity.ProjectId).SendAsync("NotificationReply", new NotificationModel
                {
                    Details = notification.Detail,
                    Title = notification.Title,
                    NotificationId = notification.ID
                });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
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



        public async Task CreateSprint(int projectId, SprintModel sprintModel)
        {
            try
            {
                Console.WriteLine("create sprint start");
                var sprint = await _mediator.Send(new CreateSprintCommand() { ProjectId = projectId, DateEnd = sprintModel.DateEnd, DateStart = sprintModel.DateStart });

                foreach (var task in sprintModel.Tasks)
                {
                    await _mediator.Send(new AddInSprintProjectTaskCommand { ProjectTaskId = task.ProjectTaskId, SprintId = sprint.ID });
                }

                var project = await _mediator.Send(new GetProjectQuery { ProjectId = projectId });


                await _mediator.Send(new CreateNotificationCommand
                {
                    Details = "The task status has changed",
                    Title = "Project task",
                    UserId = (await _mediator.Send(new GetUsersByProjectQuery { ProjectId = project.ID })).Where(u => u.ID != project.ProjectLeadId)
                        .Select(u => u.ID)
                        .ToList()
                });

                sprintModel.SprintId = sprint.ID;
                sprintModel.Tasks = sprintModel.Tasks.Select(t => new ProjectTaskModel
                {
                    Title = t.Title,
                    Details = t.Details,
                    ExecutorName = t.ExecutorName,
                    ExecutorTag = t.ExecutorTag,
                    ProjectTaskId = t.ProjectTaskId,
                    Status = 1
                }).ToList();

                await Clients.Group(GROUP_PROJECT_PREFIX + projectId).SendAsync("CreateSprintReply", projectId, sprintModel);

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
