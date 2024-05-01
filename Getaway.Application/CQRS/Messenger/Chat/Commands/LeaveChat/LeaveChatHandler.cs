using Getaway.Application.RepositoriesInterfaces;
using Getaway.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.Messenger.Chat.Commands.LeaveChat
{
    public class LeaveChatHandler(IMessangerRepository messangerRepository) : IRequestHandler<LeaveChatCommand>
    {
        public async Task Handle(LeaveChatCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await messangerRepository.DeleteChat(request.UserId, request.ChatId);
            }
            catch
            {
                throw new Exception();
            }

        }
    }
}
