using Getaway.Application.CQRS.Notification.Commands.CreateNotification;
using Getaway.Application.CQRS.Team.Commands.AddUserInTeam;
using Getaway.Application.CQRS.Team.Commands.CreateTeam;
using Getaway.Application.CQRS.Team.Commands.DeleteUserFromTeam;
using Getaway.Application.CQRS.Team.Commands.LeaveTeam;
using Getaway.Application.CQRS.Team.Commands.UpdateTeam;
using Getaway.Application.CQRS.Team.Queries.GetTeam;
using Getaway.Application.CQRS.Team.Queries.GetTeams;
using Getaway.Application.CQRS.Team.Queries.GetUsersByChhat;
using Getaway.Application.CQRS.Team.Queries.GetUsersByTeam;
using Getaway.Application.CQRS.User.Queries.GetUserById;
using Getaway.Application.CQRS.User.Queries.GetUserByTag;
using Getaway.Application.ReturnsModels;
using Getaway.Core.Entities;
using Getaway.Core.Enums;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace Getaway.Presentation.Hubs
{




    public class TeamHub : Hub
    {
        private const string GROUP_TEAM_PREFIX = "team_";
        private const string USER_PREFIX = "user_";
        private const string NOTIFICATION_TEAM_PREFIX = "Teams";

        private static IMediator _mediator;
        private static ILogger<TeamHub> _logger;

        private static UserConnections _userConnections = new UserConnections();

        public TeamHub(IMediator mediator, ILogger<TeamHub> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }


        public async Task ConnectUserWithTeams(int userId, string userTag)
        {
            var teams = await _mediator.Send(new GetTeamsQuery { UserId = userId });
            foreach (var team in teams)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, GROUP_TEAM_PREFIX + team.ID);
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



        public async Task CreateTeam(int userId, string name)
        {
            try
            {
                var team = _mediator.Send(new CreateTeamCommand() { UserId = userId, Name = name }).Result;
                var user = _mediator.Send(new GetUserByIdQuery() { UserId = userId }).Result;

                await Groups.AddToGroupAsync(Context.ConnectionId, GROUP_TEAM_PREFIX + team.ID);
                await Clients.Group(GROUP_TEAM_PREFIX + team.ID).SendAsync("NewTeamReply", new TeamModel()
                {
                    TeamId = team.ID.Value,
                    TeamName = team.Name,
                    UserRole = (int)UserRole.LEAD,
                    TeamTag = team.Tag,
                    Users = new List<UserModel> {new UserModel()
                    {
                        Email = user.Email,
                        FirstName = user.FirstName,
                        SecondName = user.SecondName,
                        LastName = user.LastName,
                        UserTag = user.Tag,
                        PhoneNumber = user.PhoneNumber,
                        ColorNumber = 3
                    }},
                    TeamLeadName = user.FirstName
                });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.InnerException.Message);
                _logger.LogError(ex.Message);
            }
        }
        public async Task Update(TeamEntity teamEntity)
        {
            try
            {
                await _mediator.Send(new UpdateTeamCommand() { LeadTag = teamEntity.TeamLeadTag, Name = teamEntity.Name, TeamId = teamEntity.ID.Value });

                var users = (await _mediator.Send(new GetUsersByTeamQuery { TeamId = teamEntity.ID.Value })).Select(u => u.ID).ToList();
                var notification = await _mediator.Send(new CreateNotificationCommand
                {
                    Details = "One of the teams has been updated",
                    Title = NOTIFICATION_TEAM_PREFIX,
                    UserId = users
                });

                await Clients.OthersInGroup(GROUP_TEAM_PREFIX + teamEntity.ID).SendAsync("NotificationReply", new NotificationModel
                {
                    Details = notification.Detail,
                    Title = notification.Title,
                    NotificationId = notification.ID
                });
                await Clients.Group(GROUP_TEAM_PREFIX + teamEntity.ID).SendAsync("TeamUpdatedReply", teamEntity.TeamLeadTag, new TeamModel
                {
                    TeamId = teamEntity.ID.Value,
                    TeamLeadName = (await _mediator.Send(new GetUserByTagQuery { UserTag = teamEntity.TeamLeadTag })).FirstName,
                    TeamName = teamEntity.Name,
                });

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException.Message);
            }
        }
        public async Task AddUser(int teamId, string userTag)
        {
            try
            {
                await _mediator.Send(new AddUserInTeamCommand() { TeamId = teamId, UserTag = userTag });

                var newUser = await _mediator.Send(new GetUserByTagQuery { UserTag = userTag });
                var team = await _mediator.Send(new GetTeamQuery { TeamId = teamId });

                var notificationForOne = await _mediator.Send(new CreateNotificationCommand
                {
                    Details = "You have been added to a new team",
                    Title = NOTIFICATION_TEAM_PREFIX,
                    UserId = new List<int> { newUser.ID }
                });
                var notificationForAll = await _mediator.Send(new CreateNotificationCommand
                {
                    Details = "A new user has been added to the team",
                    Title = NOTIFICATION_TEAM_PREFIX,
                    UserId = (await _mediator.Send(new GetUsersByTeamQuery { TeamId = teamId })).Where(u => u.ID != newUser.ID && u.ID != team.TeamLeadId)
                         .Select(u => u.ID)
                         .ToList()
                });


                await Clients.Group(GROUP_TEAM_PREFIX + teamId).SendAsync("AddUserInTeamReply", teamId, new UserModel
                {
                    Email = newUser.Email,
                    SecondName = newUser.SecondName,
                    FirstName = newUser.FirstName,
                    LastName = newUser.FirstName,
                    PhoneNumber = newUser.PhoneNumber,
                    UserTag = userTag,
                    ColorNumber = 1
                });
                await Clients.OthersInGroup(GROUP_TEAM_PREFIX + teamId).SendAsync("NotificationReply", new NotificationModel
                {
                    Details = notificationForAll.Detail,
                    Title = notificationForAll.Title,
                    NotificationId = notificationForAll.ID
                });


                if (_userConnections.IsConnectedUser(userTag))
                {
                    await Groups.AddToGroupAsync(_userConnections.GetUserConnectionId(newUser.ID), GROUP_TEAM_PREFIX + teamId);
                    await Clients.Group(USER_PREFIX + newUser.ID).SendAsync("NewTeamReply", new TeamModel
                    {
                        TeamId = team.ID.Value,
                        TeamTag = team.Tag,
                        TeamName = team.Name,
                        TeamLeadName = (await _mediator.Send(new GetUserByIdQuery { UserId = team.TeamLeadId.Value })).FirstName,
                        UserRole = (int)UserRole.EMPLOYEE,
                        Users = _mediator.Send(new GetUsersByTeamQuery() { TeamId = team.ID.Value }).Result.Select(t => new UserModel()
                        {
                            Email = t.Email,
                            FirstName = t.FirstName,
                            SecondName = t.SecondName,
                            LastName = t.LastName,
                            PhoneNumber = t.PhoneNumber,
                            UserTag = t.Tag,
                            ColorNumber = 4
                        }).ToList()
                    });
                    await Clients.Group(USER_PREFIX + _userConnections.GetUserId(userTag)).SendAsync("NotificationReply", new NotificationModel
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
                _logger.LogError(ex.InnerException.Message);
            }
        }
        public async Task DeleteUser(int teamId, string userTag)
        {
            try
            {
                await _mediator.Send(new DeleteUserFromTeamCommand() { TeamId = teamId, UserTag = userTag });

                var kickedUser = await _mediator.Send(new GetUserByTagQuery { UserTag = userTag });
                var team = await _mediator.Send(new GetTeamQuery { TeamId = teamId });

                var notificationForOne = await _mediator.Send(new CreateNotificationCommand
                {
                    Details = "You were kicked out of the team",
                    Title = NOTIFICATION_TEAM_PREFIX,
                    UserId = new List<int> { kickedUser.ID }
                });
                var notificationForAll = await _mediator.Send(new CreateNotificationCommand
                {
                    Details = $"User {kickedUser.FirstName} has left the team {team.Name}",
                    Title = NOTIFICATION_TEAM_PREFIX,
                    UserId =
                        (await _mediator.Send(new GetUsersByTeamQuery { TeamId = teamId })).Where(u => u.ID != kickedUser.ID && u.ID != team.TeamLeadId)
                        .Select(u => u.ID)
                        .ToList()

                }); ;




                if (_userConnections.IsConnectedUser(userTag))
                {
                    await Groups.RemoveFromGroupAsync(USER_PREFIX + _userConnections.GetUserConnectionId(kickedUser.ID), GROUP_TEAM_PREFIX + teamId);
                    await Clients.Group(USER_PREFIX + kickedUser.ID).SendAsync("DeleteTeamReply", teamId);
                    await Clients.Group(USER_PREFIX + kickedUser.ID).SendAsync("NotificationReply", new NotificationModel
                    {
                        Details = notificationForOne.Detail,
                        Title = notificationForOne.Title,
                        NotificationId = notificationForOne.ID
                    });

                }

                await Clients.Group(GROUP_TEAM_PREFIX + teamId).SendAsync("DeleteUserFromTeamReply", teamId, userTag);
                await Clients.OthersInGroup(GROUP_TEAM_PREFIX + teamId).SendAsync("NotificationReply", new NotificationModel
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
        public async Task LeaveFromTeam(int userId, int teamId)
        {
            try
            {
                await _mediator.Send(new LeaveTeamCommand() { TeamId = teamId, UserId = userId });

                var kickedUser = await _mediator.Send(new GetUserByIdQuery { UserId = userId });
                var users = (await _mediator.Send(new GetUsersByTeamQuery { TeamId = teamId }));

                if (users != null && users.Count > 0)
                {
                    var notification = await _mediator.Send(new CreateNotificationCommand
                    {
                        Details = $"User {kickedUser.FirstName} has left the team",
                        Title = NOTIFICATION_TEAM_PREFIX,
                        UserId = users.Select(u => u.ID).ToList()
                    });

                    await Clients.Group(GROUP_TEAM_PREFIX + teamId).SendAsync("DeleteUserFromTeamReply", teamId, kickedUser.Tag);
                    await Clients.OthersInGroup(GROUP_TEAM_PREFIX + teamId).SendAsync("NotificationReply", new NotificationModel
                    {
                        Details = notification.Detail,
                        Title = notification.Title,
                        NotificationId = notification.ID
                    });
                }

                await Clients.Group(USER_PREFIX + userId).SendAsync("DeleteTeamReply", teamId);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, GROUP_TEAM_PREFIX + teamId);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

            }
        }

    }
}
