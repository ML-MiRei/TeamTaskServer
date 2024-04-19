using Getaway.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.Messenger.Message.Commands.UpdateMessage
{
    public class UpdateMessageCommand : IRequest
    {
        public int ChatID { get; set; }
        public int MessageId { get; set; }
        public string TextMessage { get; set; }
    }
}
