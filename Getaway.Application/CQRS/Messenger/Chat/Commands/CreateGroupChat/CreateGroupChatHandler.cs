using Getaway.Application.RepositoriesInterfaces;
using Getaway.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.Messenger.Chat.Commands.CreateGroupChat
{
    public class CreateGroupChatHandler(IMessangerRepository messangerRepository) : IRequestHandler<CreateGroupChatCommand, ChatEntity>
    {
        public async Task<ChatEntity> Handle(CreateGroupChatCommand request, CancellationToken cancellationToken)
        {
            try
            {
                return await messangerRepository.CreateGroupChat(request.UserId, request.Name);
            }
            catch
            {
                throw new Exception();
            }

        }
    }
}
