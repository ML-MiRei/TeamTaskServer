using Getaway.Application.RepositoriesInterfaces;
using Getaway.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.Messenger.Message.Commands.CreateMessage
{
    public class CreateMessageHandler(IMessangerRepository messangerRepository) : IRequestHandler<CreateMessageCommand, MessageEntity>
    {
        public Task<MessageEntity> Handle(CreateMessageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var message = messangerRepository.CreateMessage(request.ChatID, request.UserId, request.TextMessage);
                Console.WriteLine(message.Result.CreatorTag);
                return message;

            }
            catch
            {
                throw new Exception();
            }
        }
    }
}
