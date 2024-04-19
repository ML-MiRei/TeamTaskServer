using Getaway.Application.RepositoriesInterfaces;
using Getaway.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.Messenger.Message.Commands.DeleteMessage
{
    public class DeleteMessageHandler(IMessangerRepository messangerRepository) : IRequestHandler<DeleteMessageCommand>
    {
        public Task Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                messangerRepository.DeleteMessage(request.ChatID, request.MessageId);
                return Task.CompletedTask;

            }
            catch
            {
                throw new Exception();
            }
        }
    }
}
