using Getaway.Application.RepositoriesInterfaces;
using Getaway.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.Messenger.Chat.Commands.UpdateGroupChat
{
    public class UpdateGroupChatHandler(IMessangerRepository messangerRepository) : IRequestHandler<UpdateGroupChatCommand>
    {
        public async Task Handle(UpdateGroupChatCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await messangerRepository.UpdateGroupChat(request.ChatId, request.Name, request.AdminTag);
            }
            catch
            {
                throw new Exception();
            }

        }
    }
}
