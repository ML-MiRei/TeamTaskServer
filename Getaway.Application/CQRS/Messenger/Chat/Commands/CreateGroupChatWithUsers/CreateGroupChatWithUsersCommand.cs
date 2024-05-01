using Getaway.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.Messenger.Chat.Commands.CreateGroupChatWithUsers
{
    public class CreateGroupChatWithUsersCommand : IRequest<ChatEntity>
    {
        public int AdminId { get; set; }
        public string Name { get; set; }
        public int[] UsersId { get; set; }
    }
}
