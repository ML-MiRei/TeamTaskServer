using Getaway.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.Messenger.Chat.Commands.CreatePrivateChat
{
    public class CreatePrivateChatCommand : IRequest<ChatEntity>
    {
        public int UserId { get; set; }
        public string SecondUserTag { get; set; }
    }
}
