using Azure;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Server;
using Server.Data;
using Server.Data.Model;
using Services;
using System.Text;


namespace Server.Services
{



    public class ChatApiService : ChatService.ChatServiceBase
    {
        MyDbContext db;
        public ChatApiService()
        {
            db = Config.myDbContext;
        }

        public override Task<VoidChatReply> AddUserChat(AddUserChatRequest request, ServerCallContext context)
        {
            try
            {
                Chat_User chat_User = db.Chats_Users.First(p => p.ID_Chat == request.IdChat && p.ID_User == db.Users.First(p => p.Tag == request.UserTag).ID);
                db.Chats_Users.Add(chat_User);
                db.SaveChanges();
                return Task.FromResult(new VoidChatReply());

            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, "add db error"));
            }
        }

        public override Task<ListChatItem> CreateChats(CreateChatRequest request, ServerCallContext context)
        {
            try
            {
                Chat chat = new Chat()
                {
                    ChatType = (int)ChatTypeEnum.CHAT
                };
                db.Chats.Add(chat);

                db.SaveChanges();


                db.Chats_Users.Add(new Chat_User() { ID_Chat = chat.ID, ID_User = request.IdUser });
                db.SaveChanges();

                foreach(var u in db.Users)
                {
                    Console.WriteLine(u.Tag);
                    Console.WriteLine(request.UserTag);
                    Console.WriteLine(u.Tag.Trim() == request.UserTag);
                }



                db.Chats_Users.Add(new Chat_User() { ID_Chat = chat.ID, ID_User = db.Users.First(p => p.Tag == request.UserTag).ID });
                db.SaveChanges();

                Console.WriteLine($"save changes end. chat = {chat.ID}");
                Console.WriteLine($"save changes end. user1 = {request.IdUser}");
                Console.WriteLine($"save changes end. user2 = {request.UserTag}");

                return Task.FromResult(new ListChatItem()
                {
                    IdChat = chat.ID,
                    Name = db.Users.Find(request.IdUser).FirstName,
                    ChatTypeN = (int)ChatTypeEnum.CHAT,
                    Image = "C:\\Users\\feyri\\source\\repos\\ChatTaskApp\\ChatTaskApp\\res\\profile.png"

                });

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
                Console.WriteLine(ex.Message);
                throw new RpcException(new Status(StatusCode.Internal, "add db error"));
            }
        }


        public override Task<ListChatsReply> GetListChats(GetListChatsRequest request, ServerCallContext context)
        {
            ListChatsReply listChats = new ListChatsReply();
          
            List<ListChatItem> replyList = (from c in db.Chats
                                            join cu in db.Chats_Users on c.ID equals cu.ID_Chat
                                            join u in db.Users on cu.ID_User equals u.ID
                                            join l in
                                                from msg in db.Messages
                                                group msg by msg.ID_Chat into grp
                                                select new
                                                {
                                                    IdChat = grp.Key,
                                                    MaxDate = grp.Max(msg => msg.DateCreated)
                                                }
                                                on c.ID equals l.IdChat
                                            join m in db.Messages on c.ID equals m.ID_Chat into jm
                                            from j in jm.DefaultIfEmpty()
                                            where j.DateCreated == l.MaxDate && cu.ID_User == request.IdUser
                                            select new ListChatItem()
                                            {
                                                IdAdmin = c.ID_Admin,
                                                IdChat = c.ID,
                                                Name = c.ChatType == (int)ChatTypeEnum.GROUP ? c.Name : db.Users.First(k => k.ID == db.Chats_Users.First(g => g.ID_Chat == c.ID && g.ID_User != request.IdUser).ID_User).FirstName,
                                                ChatTypeN = c.ChatType,
                                                LastMessage = j.TextMessage == "" ? "" : j.TextMessage,
                                                LastMessageCreator = u.FirstName,
                                                Image = "C:\\Users\\feyri\\source\\repos\\ChatTaskApp\\ChatTaskApp\\res\\profile.png"
                                            }).ToList();



            Console.WriteLine($"amount chats = {replyList.Count}");

            for (int i = 0; i < replyList.Count; i++)
            {
                replyList[i].ChatUsers.AddRange(db.Chats_Users.Where(c => c.ID_Chat == replyList[i].IdChat).Select(s=> new ChatUser() {IdChatUser = s.ID_User}));
            }

            listChats.Chats.AddRange(replyList);
            return Task.FromResult(listChats);
        }

        public override Task<LeaveChatReply> LeaveChat(LeaveChatRequst request, ServerCallContext context)
        {
            try
            {
                Chat_User chat_User = db.Chats_Users.First(p => p.ID_Chat == request.IdChat && p.ID_User == request.IdUser);
                db.Chats_Users.Remove(chat_User);
                db.SaveChanges();

                var pr = db.Chats_Users.Where(p => p.ID_Chat == request.IdChat);
                if (pr.Count() == 0)
                {
                    Chat chat = db.Chats.Find(request.IdChat);
                    db.Chats.Remove(chat);
                    db.SaveChanges();
                }

                return Task.FromResult(new LeaveChatReply()
                {
                    IdChat = request.IdChat
                });

            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, "leave group db error"));
            }
        }

        public override Task<ChatReply> UpdateChats(UpdateChatRequest request, ServerCallContext context)
        {
            try
            {
                Chat chat = db.Chats.Find(request.IdChat);
                chat.Name = request.Name;
                chat.ID_Admin = request.IdAdmin;
                db.Chats.Update(chat);
                db.SaveChanges();
                return Task.FromResult(new ChatReply() { ChatTypeN = request.ChatTypeN, IdAdmin = request.IdAdmin, IdChat = request.IdChat, Name = request.Name});

            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, "update db error"));
            }
        }

        public override Task<ListChatItem> CreateGroup(CreateGroupRequest request, ServerCallContext context)
        {
            try
            {
                Chat chat = new Chat()
                {
                    ChatType = (int)ChatTypeEnum.GROUP,
                    ID_Admin = request.IdUser,
                    Name = request.Name
                };
                db.Chats.Add(chat);
                db.SaveChanges();
                db.Chats_Users.Add(new Chat_User() { ID_Chat = chat.ID, ID_User = request.IdUser });
                db.SaveChanges();
                return Task.FromResult(new ListChatItem()
                {
                    IdChat = chat.ID,
                    IdAdmin = request.IdUser,
                    Name = request.Name,
                    Image = "C:\\Users\\feyri\\source\\repos\\ChatTaskApp\\ChatTaskApp\\res\\profile.png"
                });

            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, "add db error"));
            }
        }





    }

    

}
