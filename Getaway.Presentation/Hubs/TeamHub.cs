using Getaway.Application.CQRS.Messenger.Chat.Commands.AddUserInChat;
using Getaway.Application.CQRS.Messenger.Chat.Commands.CreatePrivateChat;
using Getaway.Application.CQRS.Messenger.Chat.Commands.DeleteUserFromChat;
using Getaway.Application.CQRS.Messenger.Chat.Commands.LeaveChat;
using Getaway.Application.CQRS.Messenger.Chat.Commands.UpdateGroupChat;
using Getaway.Application.CQRS.Messenger.Chat.Queries.GetChat;
using Getaway.Application.CQRS.Messenger.Chat.Queries.GetChats;
using Getaway.Application.CQRS.Messenger.Message.Commands.CreateMessage;
using Getaway.Application.CQRS.Messenger.Message.Commands.DeleteMessage;
using Getaway.Application.CQRS.Messenger.Message.Commands.UpdateMessage;
using Getaway.Application.CQRS.Messenger.Message.Queries.GetMessages;
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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;

namespace Getaway.Presentation.Hubs
{




    public class TeamHub : Hub
    {
        private const string GROUP_TEAM_PREFIX = "team_";
        private const string USER_PREFIX = "user_";
        private static IMediator _mediator;

        private static UserConnections _userConnections = new UserConnections();

        public TeamHub(IMediator mediator)
        {
            _mediator = mediator;
        }


        public async Task CreateTeam(int userId, string name)
        {
            try
            {
                var team = _mediator.Send(new CreateTeamCommand() { UserId = userId, Name = name }).Result;
                var user = _mediator.Send(new GetUserByIdQuery() { UserId = userId }).Result;


                await Groups.AddToGroupAsync(Context.ConnectionId, GROUP_TEAM_PREFIX + team.ID);
                await Clients.Group(GROUP_TEAM_PREFIX + team.ID).SendAsync("CreateTeamReply", new TeamModel()
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
                Console.WriteLine(ex.InnerException.Message);
                Console.WriteLine(ex.Message);
            }
        }



        public async Task ConnectUserWithTeams(int userId, string userTag)
        {
            var teams = await _mediator.Send(new GetTeamsQuery { UserId = userId });
            foreach (var team in teams)
            {
                Console.WriteLine("pr" + GROUP_TEAM_PREFIX + team.ID);
                await Groups.AddToGroupAsync(Context.ConnectionId, GROUP_TEAM_PREFIX + team.ID);
            }

            _userConnections.ListUserConnections.Add(new UserConnection { ConnectionId = Context.ConnectionId, Id = userId, Tag = userTag });

            await Console.Out.WriteLineAsync(Context.ConnectionId);

            await Groups.AddToGroupAsync(Context.ConnectionId, USER_PREFIX + userId);

        }


        public override Task OnDisconnectedAsync(Exception? exception)
        {
            base.OnDisconnectedAsync(exception);
            _userConnections.RemoveUserConnectionId(Context.ConnectionId);
            return Task.CompletedTask;
        }


        public async Task Update(TeamEntity teamEntity)
        {
            try
            {
                await _mediator.Send(new UpdateTeamCommand() { LeadTag = teamEntity.TeamLeadTag, Name = teamEntity.Name, TeamId = teamEntity.ID.Value });

                await Clients.Group(GROUP_TEAM_PREFIX + teamEntity.ID).SendAsync("TeamUpdatedReply",
                    new TeamModel
                    {
                        TeamId = teamEntity.ID.Value,
                        TeamLeadName = (await _mediator.Send(new GetUserByTagQuery { UserTag = teamEntity.TeamLeadTag })).FirstName,
                        TeamName = teamEntity.Name,
                        TeamTag = teamEntity.Tag,
                        Users = (await _mediator.Send(new GetUsersByTeamQuery { TeamId = teamEntity.ID.Value })).Select(u => new UserModel
                        {
                            Email = u.Email,
                            FirstName = u.FirstName,
                            SecondName = u.SecondName,
                            LastName = u.LastName,
                            PhoneNumber = u.PhoneNumber,
                            UserTag = u.Tag,
                            ColorNumber = 4
                        }).ToList(),
                        UserRole = _userConnections.GetUserTag(Context.ConnectionId) == teamEntity.TeamLeadTag ? (int)UserRole.LEAD : (int)UserRole.EMPLOYEE
                    });

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        public async Task AddUser(int teamId, string userTag)
        {
            try
            {
                await _mediator.Send(new AddUserInTeamCommand() { TeamId = teamId, UserTag = userTag });

                var newUser = await _mediator.Send(new GetUserByTagQuery { UserTag = userTag });

                await Console.Out.WriteLineAsync("Adduser");


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
                    UserId = (await _mediator.Send(new GetUsersByTeamQuery { TeamId = teamId })).Where(u => u.ID != newUser.ID)
                        .Select(u => u.ID)
                        .ToList()

                });


                var team = await _mediator.Send(new GetTeamQuery { TeamId = teamId });

                await Console.Out.WriteLineAsync("user = " + newUser.ID);

                await Clients.Group(USER_PREFIX + newUser.ID).SendAsync("NewTeamReply",
                    new TeamModel
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
                    }
                     );

                await Console.Out.WriteLineAsync(GROUP_TEAM_PREFIX + teamId);

                await Clients.Group(GROUP_TEAM_PREFIX + teamId).SendAsync("AddUserInTeamReply",
                                  teamId,
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
                    await Groups.AddToGroupAsync(USER_PREFIX + _userConnections.GetUserConnectionId(newUser.ID), GROUP_TEAM_PREFIX + teamId);



            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                Console.WriteLine(ex.Message);
            }
        }


        public async Task DeleteUser(int teamId, string userTag)
        {
            try
            {
                await _mediator.Send(new DeleteUserFromTeamCommand() { TeamId = teamId, UserTag = userTag });

                var kickedUser = await _mediator.Send(new GetUserByTagQuery { UserTag = userTag });


                await _mediator.Send(new CreateNotificationCommand
                {
                    Details = "You have been added to a new chat",
                    Title = "Chats",
                    UserId = new List<int> { kickedUser.ID }
                });



                await _mediator.Send(new CreateNotificationCommand
                {
                    Details = "A new user has been added to the chat",
                    Title = "Chats",
                    UserId = (await _mediator.Send(new GetUsersByTeamQuery { TeamId = teamId })).Where(u => u.ID != kickedUser.ID)
                        .Select(u => u.ID)
                        .ToList()

                });


                var team = await _mediator.Send(new GetTeamQuery { TeamId = teamId });



                Task task1 = Clients.Group(USER_PREFIX + kickedUser.ID).SendAsync("DeleteTeamReply", teamId);

                if (_userConnections.IsConnectedUser(userTag))
                    await Groups.RemoveFromGroupAsync(USER_PREFIX + _userConnections.GetUserConnectionId(kickedUser.ID), GROUP_TEAM_PREFIX + teamId);

                Task task2 = Clients.Group(GROUP_TEAM_PREFIX + teamId).SendAsync("DeleteUserFromTeamReply", teamId, userTag);


                Task.WaitAll(task1, task2);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        public async Task LeaveFromTeam(int userId, int teamId)
        {
            try
            {

                await Console.Out.WriteLineAsync("Leave team");


                await _mediator.Send(new LeaveTeamCommand() { TeamId = teamId, UserId = userId });

                var kickedUser = await _mediator.Send(new GetUserByIdQuery { UserId = userId });

                var users = (await _mediator.Send(new GetUsersByTeamQuery { TeamId = teamId })).Where(u => u.ID != kickedUser.ID)
                        .Select(u => u.ID)
                        .ToList();


                if (users != null && users.Count != 0)
                {

                    await _mediator.Send(new CreateNotificationCommand
                    {
                        Details = "A new user has been added to the chat",
                        Title = "Chats",
                        UserId = users

                    });
                }



                await Clients.Group(USER_PREFIX + kickedUser.ID).SendAsync("DeleteTeamReply", teamId);

                if (_userConnections.IsConnectedUser(userId))
                {
                    await Console.Out.WriteLineAsync("Delete user " + _userConnections.GetUserConnectionId(userId) + "  team = " + GROUP_TEAM_PREFIX + teamId);
                    await Groups.RemoveFromGroupAsync(_userConnections.GetUserConnectionId(userId), GROUP_TEAM_PREFIX + teamId);
                }

                await Clients.Group(GROUP_TEAM_PREFIX + teamId).SendAsync("DeleteUserFromTeamReply", teamId, kickedUser.Tag);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

    }
}
