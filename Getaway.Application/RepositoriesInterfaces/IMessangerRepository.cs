using Getaway.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.RepositoriesInterfaces
{
    public interface IMessangerRepository
    {
        Task UpdateMessage(int chatId, int messageId, string textMessage);
        Task DeleteMessage(int chatId, int messageId);
        Task<MessageEntity> CreateMessage(int chatId, int userId, string textMessage);
        Task<List<MessageEntity>> GetListMessage(int chatId);


        Task<List<ChatEntity>> GetChatList(int userId);
        Task<ChatEntity> GetChat (int chatId);
        Task<ChatEntity> CreateGroupChat(int userId, string name);
        Task<ChatEntity> CreateGroupChatWithUsers(int adminId, string name, int[] usersId);
        Task<ChatEntity> CreatePrivateChat(int userId, string secondUserTag);
        Task UpdateGroupChat(int chatId, string? name, string? adminTag);
        Task DeleteChat(int userId, int chatId);
        Task AddUserInChat(int chatId, string userTag);
        Task DeleteUserFromChat(int chatId, string userTag);

        Task<List<UserEntity>> GetUsersInChat(int chatId);

    }
}
