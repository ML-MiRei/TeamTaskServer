using Getaway.Application.CQRS.Messenger.Chat.Queries.GetChats;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Getaway.Presentation.Hubs
{
    public class UsersHub : Hub
    {
        private const string GROUP_USER_PREFIX = "user_";

       

    }

    public class CustomUserIdProvider : IUserIdProvider
    {
        public virtual string GetUserId(HubConnectionContext connection)
        {
            Console.WriteLine(connection.User?.Identity.Name);
            return connection.User?.Identity.Name;
        }
    }
}
