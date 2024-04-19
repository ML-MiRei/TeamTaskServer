using Getaway.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.Messenger.Chat.Commands.UpdateGroupChat
{
    public class UpdateGroupChatCommand : IRequest
    {
        public int ChatId { get; set; }
        public string? Name { get; set; }
        public string? AdminTag { get; set; }
    }
}
