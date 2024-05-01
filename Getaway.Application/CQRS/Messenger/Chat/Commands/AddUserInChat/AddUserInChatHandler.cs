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
        public async Task Handle(AddUserInChatCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await messangerRepository.AddUserInChat(request.ChatId, request.UserTag);
            }
            catch
            {
                throw new Exception();
            }

        }
    }
}
