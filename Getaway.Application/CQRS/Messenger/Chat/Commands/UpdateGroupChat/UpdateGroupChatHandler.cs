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
        public  Task Handle(UpdateGroupChatCommand request, CancellationToken cancellationToken)
        {
            try
            {
                messangerRepository.UpdateGroupChat(request.ChatId, request.Name, request.AdminTag);
                return Task.CompletedTask;
            }
            catch
            {
                throw new Exception();
            }

        }
    }
}
