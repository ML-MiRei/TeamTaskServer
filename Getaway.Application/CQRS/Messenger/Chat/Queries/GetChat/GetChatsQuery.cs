using Getaway.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.Messenger.Chat.Queries.GetChat
{
    public class GetChatQuery : IRequest<ChatEntity>
    {
        public int ChatId {  get; set; }
    }
}
