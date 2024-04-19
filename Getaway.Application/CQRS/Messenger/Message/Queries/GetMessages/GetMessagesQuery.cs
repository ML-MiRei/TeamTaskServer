using Getaway.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.Messenger.Message.Queries.GetMessages
{
    public class GetMessagesQuery : IRequest<List<MessageEntity>>
    {
        public int ChatId { get; set; }
    }
}
