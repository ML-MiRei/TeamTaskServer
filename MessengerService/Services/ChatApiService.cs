using Azure;
using Google.Protobuf.WellKnownTypes;
using GreatDatabase.Data;
using GreatDatabase.Data.Enums;
using GreatDatabase.Data.Model;
using Grpc.Core;
using Microsoft.AspNetCore.Components.Forms;



namespace MessengerService.Services
{




    public class ChatApiService : ChatService.ChatServiceBase
    {
        MyDbContext db;
        private static ILogger<ChatApiService> _logger;


        public ChatApiService(ILogger<ChatApiService> logger)
        {
            _logger = logger;
            db = new MyDbContext();
        }


        public async override Task<ChatReply> GetChat(GetChatRequest request, ServerCallContext context)
        {
            var chat = await db.Chats.FindAsync(request.ChatId);
            return new ChatReply
            {
                AdminTag = db.Users.First(u => u.ID == chat.AdminId).UserTag,
                ChatType = chat.Type,
                ChatId = chat.ID,
                Name = chat.ChatName
            };

        }

        public override Task<GetUsersByChatReply> GetUsersByChat(GetUsersByChatRequest request, ServerCallContext context)
        {
            GetUsersByChatReply listUsers = new GetUsersByChatReply();
            List<ChatUserReply> replyList = (from tu in db.Chats_Users
                                             join u in db.Users on tu.UserId equals u.ID
                                             where tu.ChatId == request.ChatId
                                             select new ChatUserReply()
                                             {
                                                 Email = u.Email,
                                                 SecondName = u.SecondName,
                                                 FirstName = u.FirstName,
                                                 LastName = u.LastName,
                                                 PhoneNumber = u.PhoneNumber,
                                                 UserTag = u.UserTag,
                                                 UserId = u.ID
                                             }).ToList();

            _logger.LogInformation($"Return {replyList.Count} users");

            listUsers.Users.AddRange(replyList);
            return Task.FromResult(listUsers);
        }

      


        public override async Task<ChatReply> CreateGroupChat(CreateGroupChatRequest request, ServerCallContext context)
        {
            try
            {
                Chat chat = new Chat()
                {
                    DateCreated = DateTime.Now,
                    LastModified = DateTime.Now,
                    Type = (int)ChatTypeEnum.GROUP,
                    ChatName = request.Name,
                    AdminId = request.UserId
                };


                await db.Chats.AddAsync(chat);
                db.SaveChanges();

                db.Chats_Users.Add(new Chat_User { ChatId = chat.ID, DateCreated = DateTime.Now, UserId = request.UserId });
                db.SaveChanges();

                _logger.LogInformation($"Create group chat: {request.Name}");

                return new ChatReply()
                {
                    AdminTag = db.Users.First(u => u.ID == request.UserId).UserTag,
                    ChatId = chat.ID,
                    ChatType = chat.Type,
                    Name = chat.ChatName

                };
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception();
            }
        }

        public async override Task<ChatReply> CreatePrivateChat(CreatePrivateChatRequest request, ServerCallContext context)
        {
            try
            {
                Chat chat = new Chat()
                {
                    DateCreated = DateTime.Now,
                    LastModified = DateTime.Now,
                    Type = (int)ChatTypeEnum.PRIVATE,
                    AdminId = request.UserId
                };


                await db.Chats.AddAsync(chat);
                db.SaveChanges();

                db.Chats_Users.Add(new Chat_User { ChatId = chat.ID, DateCreated = DateTime.Now, UserId = request.UserId });
                db.Chats_Users.Add(new Chat_User { ChatId = chat.ID, DateCreated = DateTime.Now, UserId = db.Users.First(u => u.UserTag == request.SecondUserTag).ID });
                db.SaveChanges();

                _logger.LogInformation($"Create private chat with user with tag {request.SecondUserTag}");

                return new ChatReply()
                {
                    AdminTag = db.Users.First(u => u.ID == request.UserId).UserTag,
                    ChatId = chat.ID,
                    ChatType = chat.Type,
                    Name = db.Users.First(u => u.ID == db.Chats_Users.First(cu => cu.UserId != request.UserId && cu.ChatId == chat.ID).UserId).FirstName

                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException.Message);
                throw new Exception();
            }
        }

        public async override Task<VoidChatReply> AddUserInChat(AddUserChatRequest request, ServerCallContext context)
        {
            try
            {
                await db.Chats_Users.AddAsync(new Chat_User()
                {
                    DateCreated = DateTime.Now,
                    ChatId = request.ChatId,
                    UserId = db.Users.First(u => u.UserTag == request.UserTag).ID
                });

                await db.SaveChangesAsync();

                _logger.LogInformation($"Add user with tag {request.UserTag} in chat with id = {request.ChatId}");

                return new VoidChatReply();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception();
            }
        }

        public override async Task<VoidChatReply> DeleteUserFromChat(DeleteUserFromChatRequst request, ServerCallContext context)
        {
            try
            {
                var user = db.Chats_Users.First(c => c.ChatId == request.ChatId && c.UserId == db.Users.First(u => u.UserTag == request.UserTag).ID);
                db.Chats_Users.Remove(user);
                await db.SaveChangesAsync();

                _logger.LogInformation($"Delete user with tag {request.UserTag} from chat with id = {request.ChatId}");

                return new VoidChatReply();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception();
            }
        }

        public override Task<ListChatsReply> GetListChats(GetListChatsRequest request, ServerCallContext context)
        {
            try
            {
                ListChatsReply listChatsReply = new ListChatsReply();
                var chats = (from c in db.Chats
                             join cu in db.Chats_Users on c.ID equals cu.ChatId
                             where cu.UserId == request.UserId
                             select new ChatReply()
                             {
                                 AdminTag = db.Users.First(u => u.ID == cu.UserId).UserTag,
                                 ChatType = c.Type,
                                 ChatId = cu.ChatId,
                                 Name = c.Type == (int)ChatTypeEnum.PRIVATE ? db.Users.First(u => u.ID == db.Chats_Users.First(cc => cc.ChatId == c.ID && cc.UserId != request.UserId).UserId).FirstName : c.ChatName
                             }).ToList();

                listChatsReply.Chats.AddRange(chats);


                _logger.LogInformation($"Return {chats.Count} chats");

                return Task.FromResult(listChatsReply);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception();
            }
        }

        public async override Task<VoidChatReply> LeaveChat(LeaveChatRequst request, ServerCallContext context)
        {
            try
            {
                var user = db.Chats_Users.First(c => c.ChatId == request.ChatId && c.UserId == request.UserId);
                db.Chats_Users.Remove(user);
                db.SaveChanges();

                if (!db.Chats_Users.Any(cu => cu.ChatId == request.ChatId))
                {
                    Chat chat = db.Chats.First(c => c.ID == request.ChatId);
                    db.Chats.Remove(chat);
                    db.SaveChanges();
                }

                _logger.LogInformation($"User with id = {request.UserId} leave chat with id = {request.ChatId}");

                return new VoidChatReply();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception();
            }
        }

        public override async Task<VoidChatReply> UpdateChat(UpdateChatRequest request, ServerCallContext context)
        {
            try
            {
                var chat = db.Chats.First(c => c.ID == request.ChatId);

                chat.ChatName = String.IsNullOrEmpty(request.Name) ? chat.ChatName : request.Name;
                chat.AdminId = String.IsNullOrEmpty(request.AdminTag) ? chat.AdminId : db.Users.First(u => u.UserTag == request.AdminTag).ID;

                db.Chats.Update(chat);
                db.SaveChanges();

                _logger.LogInformation($"Update chat with id = {request.ChatId}");


                return new VoidChatReply();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception();
            }
        }
    }

}
