using Getaway.Application.RepositoriesInterfaces;
using Getaway.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.Messenger.Message.Queries.GetMessages
{
    public class GetMessagesHandler(IMessangerRepository messangerRepository) : IRequestHandler<GetMessagesQuery, List<MessageEntity>>
    {
        public Task<List<MessageEntity>> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                return messangerRepository.GetListMessage(request.ChatId);
            }
            catch
            {
                throw new Exception();
            }
        }
    }
}
