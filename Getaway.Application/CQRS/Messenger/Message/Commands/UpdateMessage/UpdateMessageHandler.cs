using Getaway.Application.RepositoriesInterfaces;
using Getaway.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.Messenger.Message.Commands.UpdateMessage
{
    public class UpdateMessageHandler(IMessangerRepository messangerRepository) : IRequestHandler<UpdateMessageCommand>
    {
        public Task Handle(UpdateMessageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                messangerRepository.UpdateMessage(request.ChatID, request.MessageId, request.TextMessage);
                return Task.CompletedTask;

            }
            catch
            {
                throw new Exception();
            }
        }
    }
}
