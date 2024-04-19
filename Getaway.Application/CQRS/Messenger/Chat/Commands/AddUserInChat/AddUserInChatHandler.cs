using Getaway.Application.RepositoriesInterfaces;
using Getaway.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.Messenger.Chat.Commands.AddUserInChat
{
    public class AddUserInChatHandler(IMessangerRepository messangerRepository) : IRequestHandler<AddUserInChatCommand>
    {
        public Task Handle(AddUserInChatCommand request, CancellationToken cancellationToken)
        {
            try
            {
                messangerRepository.AddUserInChat(request.ChatId, request.UserTag);
                return Task.CompletedTask;
            }
            catch
            {
                throw new Exception();
            }

        }
    }
}
