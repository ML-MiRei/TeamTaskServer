using Getaway.Application.RepositoriesInterfaces;
using Getaway.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.Messenger.Chat.Commands.CreatePrivateChat
{
    public class CreatePrivateChatHandler(IMessangerRepository messangerRepository) : IRequestHandler<CreatePrivateChatCommand, ChatEntity>
    {
        public async Task<ChatEntity> Handle(CreatePrivateChatCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var chat = await messangerRepository.CreatePrivateChat(request.UserId, request.SecondUserTag);
                return chat;
            }
            catch
            {
                throw new Exception();
            }

        }
    }
}
