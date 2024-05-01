using Getaway.Application.RepositoriesInterfaces;
using Getaway.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.Messenger.Chat.Commands.DeleteUserFromChat
{
    public class DeleteUserFromChatHandler(IMessangerRepository messangerRepository) : IRequestHandler<DeleteUserFromChatCommand>
    {
        public async Task Handle(DeleteUserFromChatCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await messangerRepository.DeleteUserFromChat(request.ChatId, request.UserTag);
            }
            catch
            {
                throw new Exception();
            }

        }
    }
}
