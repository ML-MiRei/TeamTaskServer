using Getaway.Application.RepositoriesInterfaces;
using Getaway.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.Messenger.Chat.Queries.GetChat
{
    public class GetChatHandler(IMessangerRepository messangerRepository) : IRequestHandler<GetChatQuery, ChatEntity>
    {
        public async Task<ChatEntity> Handle(GetChatQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var chats = await messangerRepository.GetChat(request.ChatId);
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
