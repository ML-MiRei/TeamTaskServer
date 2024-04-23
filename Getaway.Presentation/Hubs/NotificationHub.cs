using Getaway.Application.CQRS.Messenger.Chat.Queries.GetChats;
using Getaway.Application.CQRS.Messenger.Message.Commands.CreateMessage;
using Getaway.Application.CQRS.Messenger.Message.Commands.DeleteMessage;
using Getaway.Application.CQRS.Messenger.Message.Commands.UpdateMessage;
using Getaway.Application.CQRS.Project.Queries.GetProjects;
using Getaway.Application.CQRS.Team.Queries.GetTeams;
using Getaway.Application.ReturnsModels;
using Getaway.Core.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;

namespace Getaway.Presentation.Hubs
{
    public class NotificationHub : Hub
    {
        private const string GROUP_TEAM_PREFIX = "team_";
        private const string GROUP_PROJECT_PREFIX = "project_";
        private const string GROUP_CHAT_PREFIX = "chat_";
        private const string USER_PREFIX = "user_";



        private static HubConnection _hubConnection;


        public override Task OnConnectedAsync()
        {
            Console.WriteLine("dd");
            return base.OnConnectedAsync();
        }
        public static HubConnection HubConnection()
        {

            if (_hubConnection == null)
            {
                _hubConnection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7130/notification")
                .Build();

                _hubConnection.StartAsync();


            }
            return _hubConnection;
        }


        private static IMediator _mediator;

        public NotificationHub(IMediator mediator)
        {
            _mediator = mediator;
        }


        public async Task ConnectUserWithGroups(int userId)
        {

                var teams = await _mediator.Send(new GetTeamsQuery { UserId = userId });
                foreach (var team in teams)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, GROUP_TEAM_PREFIX + team.ID);
                }

                var projects = await _mediator.Send(new GetProjectsQuery { UserId = userId });
                foreach (var project in projects)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, GROUP_PROJECT_PREFIX + project.ID);
                }

                var chats = await _mediator.Send(new GetChatsQuery { UserId = userId });
                foreach (var chat in chats)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, GROUP_CHAT_PREFIX + chat.ID);
                }

                await Groups.AddToGroupAsync(Context.ConnectionId, USER_PREFIX + userId);
          
        }





        public async Task ChatNotification(NotificationAction action, int userId, ChatModel chatModel)
        {
            try
            {
                if (action == NotificationAction.CREATE)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, GROUP_CHAT_PREFIX + chatModel.ChatId);
                }
                else if (action == NotificationAction.DELETE)
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, GROUP_CHAT_PREFIX + chatModel.ChatId);

                int a = (int)action;

                await Console.Out.WriteLineAsync(USER_PREFIX + userId);
                await Clients.Group(USER_PREFIX + userId).SendAsync("NewChatNotification", a, chatModel);

                Console.WriteLine("New chat Notification");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        public async Task MembersChatNotification(NotificationAction action, int chatId, UserModel userModel)
        {
            try
            {
                await Clients.OthersInGroup(GROUP_CHAT_PREFIX + chatId).SendAsync("UpdateMembersChatNotification", action, chatId, userModel);

                Console.WriteLine("Update members chat notification");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        public async Task MembersTeamNotification(NotificationAction action, int teamId, UserModel userModel)
        {
            try
            {
                await Clients.Group(GROUP_CHAT_PREFIX + teamId).SendAsync("UpdateMembersTeamNotification", action, teamId, userModel);

                Console.WriteLine("Update members team notification");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        public async Task StatusProjectTaskNotification(NotificationAction action, int projectId, ProjectTaskModel projectTaskModel)
        {
            try
            {
                await Clients.Group(GROUP_PROJECT_PREFIX + projectId).SendAsync("ChangeStatusProjectTaskNotification", action, projectId, projectTaskModel);
                Console.WriteLine("Change status project task Nnotification");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }



        public async Task MembersProjectNotification(NotificationAction action, int projectId, UserModel userModel)
        {
            try
            {
                await Clients.Group(GROUP_PROJECT_PREFIX + projectId).SendAsync("UpdateMembersProjectNotification", action, projectId, userModel);

                Console.WriteLine("Update members project notification");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }



        public async Task ProjectTaskNotification(NotificationAction action, int projectId, ProjectTaskModel projectTaskModel)
        {
            try
            {
                await Clients.Group(GROUP_PROJECT_PREFIX + projectId).SendAsync("NewProjectTaskNotification", action, projectId, projectTaskModel);
                Console.WriteLine("New project task Nnotification");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task ProjectNotification(NotificationAction action, int userId, ProjectModel projectModel)
        {
            try
            {
                if (action == NotificationAction.CREATE)
                    await Groups.AddToGroupAsync(Context.ConnectionId, GROUP_PROJECT_PREFIX + projectModel.ProjectId);
                else if (action == NotificationAction.DELETE)
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, GROUP_PROJECT_PREFIX + projectModel.ProjectId);


                await Clients.Group(USER_PREFIX + userId).SendAsync("NewProjectNotification", action, projectModel);

                Console.WriteLine("New project Nnotification");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        public async Task TeamNotification(NotificationAction action, int userId, TeamModel teamModel)
        {
            try
            {
                if (action == NotificationAction.CREATE)
                    await Groups.AddToGroupAsync(Context.ConnectionId, GROUP_TEAM_PREFIX + teamModel.TeamId);
                else if (action == NotificationAction.DELETE)
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, GROUP_TEAM_PREFIX + teamModel.TeamId);

                await Clients.Group(USER_PREFIX + userId).SendAsync("NewTeamNotification", action, teamModel);

                Console.WriteLine("New team Nnotification");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

    }
}
