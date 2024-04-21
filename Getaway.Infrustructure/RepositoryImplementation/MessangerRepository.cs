using Getaway.Application.RepositoriesInterfaces;
using Getaway.Core.Entities;
using MessengerService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Getaway.Infrustructure.RepositoryImplementation
{
    internal class MessangerRepository : IMessangerRepository
    {
        public async void AddUserInChat(int chatId, string userTag)
        {
            try
            {
                await Connections.ChatServiceClient.AddUserInChatAsync(new AddUserChatRequest() { ChatId = chatId, UserTag = userTag });
            }
            catch
            {
                throw new Exception();
            }
        }

        public async Task<ChatEntity> CreateGroupChat(int userId, string name)
        {
            try
            {
                var chat = (await Connections.ChatServiceClient.CreateGroupChatAsync(new CreateGroupChatRequest() { UserId = userId, Name = name }));
                return new ChatEntity()
                {
                    Type = chat.ChatType,
                    ID = chat.ChatId,
                    Image = chat.Image,
                    ChatName = chat.Name
                };
            }
            catch
            {
                throw new Exception();
            }
        }

        public async Task<MessageEntity> CreateMessage(int chatId, int userId, string textMessage)
        {
            try
            {
                var message = await Connections.MessageServiceClient.CreateMessageAsync(new CreateMessageRequest() { ChatId = chatId, UserId = userId, TextMessage = textMessage });
                return new MessageEntity()
                {
                    ID = message.MessageId,
                    ChatId = message.ChatId,
                    TextMessage = message.TextMessage,
                    UserNameCreator = message.CreatorName,
                    CreatorTag = message.CreatorTag,
                    DateCreated = message.DateCreated.ToDateTime()
                };
            }
            catch
            {
                throw new Exception();
            }
        }

        public async Task<ChatEntity> CreatePrivateChat(int userId, string secondUserTag)
        {
            try
            {
                var chat = (await Connections.ChatServiceClient.CreatePrivateChatAsync(new CreatePrivateChatRequest() { UserId = userId, SecondUserTag =  secondUserTag}));

                Console.WriteLine("created chat " + chat.Name);


                return new ChatEntity()
                {
                    Type = chat.ChatType,
                    ID = chat.ChatId,
                    Image = chat.Image,
                    ChatName = chat.Name
                };
            }
            catch
            {
                throw new Exception();
            }
        }

        public async void DeleteChat(int userId, int chatId)
        {
            try
            {
                await Connections.ChatServiceClient.LeaveChatAsync(new LeaveChatRequst() { ChatId = chatId, UserId = userId });
            }
            catch
            {
                throw new Exception();
            }
        }

        public async void DeleteMessage(int chatId, int messageId)
        {
            try
            {
                await Connections.MessageServiceClient.DeleteMessageAsync(new DeleteMessageRequest() { ChatId = chatId, MessageId = messageId});
            }
            catch
            {
                throw new Exception();
            }
        }

        public async void DeleteUserFromChat(int chatId, string userTag)
        {
            try
            {
                await Connections.ChatServiceClient.DeleteUserFromChatAsync(new DeleteUserFromChatRequst() { ChatId = chatId, UserTag = userTag});
            }
            catch
            {
                throw new Exception();
            }
        }

        public async Task<ChatEntity> GetChat(int chatId)
        {
            try
            {
                var chat = await Connections.ChatServiceClient.GetChatAsync(new GetChatRequest() { ChatId = chatId });
                return new ChatEntity()
                {
                    ID = chat.ChatId,
                    ChatName = chat.Name,
                    Type = chat.ChatType,
                    AdminTag = chat.AdminTag
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception();
            }
        }

        public async Task<List<ChatEntity>> GetChatList(int userId)
        {
            try
            {
                var chats = await Connections.ChatServiceClient.GetListChatsAsync(new GetListChatsRequest() { UserId = userId });
                return chats.Chats.Select(c => new ChatEntity()
                {
                    ID = c.ChatId,
                    ChatName = c.Name,
                    Image = c.Image,
                    Type = c.ChatType
                }).ToList();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception();
            }
        }



        public async Task<List<MessageEntity>> GetListMessage(int chatId)
        {
            try
            {
                var messages = await Connections.MessageServiceClient.GetListMessageAsync(new GetListMessageRequest() { ChatId = chatId, Limit= 20, Skip = 0 });
                return messages.Messages.Select(m => new MessageEntity()
                {
                    ChatId = chatId,
                    ID = m.MessageId,
                    TextMessage = m.TextMessage,
                    UserNameCreator = m.CreatorName,
                    CreatorTag = m.CreatorTag,
                    DateCreated = m.DateCreated.ToDateTime()

                }).ToList();
            }
            catch(Exception ex)
            {
                Console.WriteLine( ex.Message);
                throw new Exception();
            }
        }

        public async Task<List<UserEntity>> GetUsersInChat(int chatId)
        {
            try
            {
                var users = await Connections.ChatServiceClient.GetUsersByChatAsync(new GetUsersByChatRequest() { ChatId = chatId });
                return users.Users.Select(u => new UserEntity()
                {
                    Email = u.Email,
                    FirstName = u.FirstName,
                    SecondName = u.SecondName,
                    LastName = u.LastName,
                    Tag = u.UserTag,
                    PhoneNumber = u.PhoneNumber
                }).ToList();
            }
            catch
            {
                throw new Exception();
            }
        }

        public async void UpdateGroupChat(int chatId, string name, string adminTag)
        {
            try
            {
                await Connections.ChatServiceClient.UpdateChatAsync(new UpdateChatRequest() { Name = name, ChatId = chatId, AdminTag = adminTag });
            }
            catch
            {
                throw new Exception();
            }
        }

        public async void UpdateMessage(int chatId, int messageId, string textMessage)
        {
            try
            {
                await Connections.MessageServiceClient.UpdateMessageAsync(new UpdateMessageRequest() { ChatId = chatId, MessageId = messageId, TextMessage = textMessage });
            }
            catch
            {
                throw new Exception();
            }
        }
    }
}
