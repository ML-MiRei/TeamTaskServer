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
        public Task Handle(DeleteUserFromChatCommand request, CancellationToken cancellationToken)
        {
            try
            {
                messangerRepository.DeleteUserFromChat(request.ChatId, request.UserTag);
                return Task.CompletedTask;
            }
            catch
            {
                throw new Exception();
            }

        }
    }
}
