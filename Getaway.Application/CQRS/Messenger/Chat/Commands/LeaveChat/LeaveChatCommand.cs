using Getaway.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.Messenger.Chat.Commands.LeaveChat
{
    public class LeaveChatCommand : IRequest
    {
        public int UserId { get; set; }
        public int ChatId { get; set; }
    }
}
