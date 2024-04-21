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
        void UpdateMessage(int chatId, int messageId, string textMessage);
        void DeleteMessage(int chatId, int messageId);
        Task<MessageEntity> CreateMessage(int chatId, int userId, string textMessage);
        Task<List<MessageEntity>> GetListMessage(int chatId);


        Task<List<ChatEntity>> GetChatList(int userId);
        Task<ChatEntity> GetChat (int chatId);
        Task<ChatEntity> CreateGroupChat(int userId, string name);
        Task<ChatEntity> CreatePrivateChat(int userId, string secondUserTag);
        void UpdateGroupChat(int chatId, string? name, string? adminTag);
        void DeleteChat(int userId, int chatId);
        void AddUserInChat(int chatId, string userTag);
        void DeleteUserFromChat(int chatId, string userTag);

        Task<List<UserEntity>> GetUsersInChat(int chatId);

    }
}
