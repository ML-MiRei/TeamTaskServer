using Getaway.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.Messenger.Chat.Queries.GetChats
{
    public class GetChatsQuery : IRequest<List<ChatEntity>>
    {
        public int UserId {  get; set; }
    }
}
