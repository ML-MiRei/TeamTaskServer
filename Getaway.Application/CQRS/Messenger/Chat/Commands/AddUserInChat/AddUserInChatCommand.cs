using Getaway.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.Messenger.Chat.Commands.AddUserInChat
{
    public class AddUserInChatCommand : IRequest
    {
        public int ChatId { get; set; }
        public string UserTag { get; set; }
    }
}
