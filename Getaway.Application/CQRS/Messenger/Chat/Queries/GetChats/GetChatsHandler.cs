using Getaway.Application.RepositoriesInterfaces;
using Getaway.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.Messenger.Chat.Queries.GetChats
{
    public class GetChatsHandler(IMessangerRepository messangerRepository) : IRequestHandler<GetChatsQuery, List<ChatEntity>>
    {
        public async Task<List<ChatEntity>> Handle(GetChatsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var chats = await messangerRepository.GetChatList(request.UserId);
                return chats;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception();
            }
        }
    }
}
