using Getaway.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.Messenger.Message.Commands.CreateMessage
{
    public class CreateMessageCommand : IRequest<MessageEntity>
    {
        public int ChatID { get; set; }
        public int UserId { get; set; }
        public string TextMessage { get; set; }
    }
}
