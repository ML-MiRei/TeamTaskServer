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
        public Task Handle(LeaveChatCommand request, CancellationToken cancellationToken)
        {
            try
            {
                messangerRepository.DeleteChat(request.UserId, request.ChatId);
                return Task.CompletedTask;
            }
            catch
            {
                throw new Exception();
            }

        }
    }
}
