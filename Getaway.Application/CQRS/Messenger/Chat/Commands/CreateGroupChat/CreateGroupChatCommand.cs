using Getaway.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.Messenger.Chat.Commands.CreateGroupChat
{
    public class CreateGroupChatCommand : IRequest<ChatEntity>
    {
        public int UserId { get; set; }
        public string Name { get; set; }
    }
}
