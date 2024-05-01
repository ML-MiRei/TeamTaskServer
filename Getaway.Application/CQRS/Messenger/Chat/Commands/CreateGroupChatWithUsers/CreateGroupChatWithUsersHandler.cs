using Getaway.Application.RepositoriesInterfaces;
using Getaway.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.Messenger.Chat.Commands.CreateGroupChatWithUsers
{
    public class CreateGroupChatWithUsersHandler(IMessangerRepository messangerRepository) : IRequestHandler<CreateGroupChatWithUsersCommand, ChatEntity>
    {
        public async Task<ChatEntity> Handle(CreateGroupChatWithUsersCommand request, CancellationToken cancellationToken)
        {
            try
            {
                return await messangerRepository.CreateGroupChatWithUsers(request.AdminId, request.Name, request.UsersId);
            }
            catch
            {
                throw new Exception();
            }

        }
    }
}
