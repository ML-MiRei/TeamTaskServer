using Azure;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Server;
using Server.Data;
using Server.Data.Model;
using Services;


namespace Server.Services
{
    public class MessageApiService : MessageService.MessageServiceBase
    {
        MyDbContext db;
        public MessageApiService()
        {
            db = Config.myDbContext;
        }
        public override Task<MessageReply> CreateMessage(CreateMessageRequest request, ServerCallContext context)
        {
            try
            {
                
                Message message = new Message()
                {
                    TextMessage = request.Text,
                    DateCreated = DateTime.Now,
                    ID_Chat = request.IdChat,
                    ID_Creator = request.IdUser
                };
                db.Messages.Add(message);

                db.SaveChanges();

                Console.WriteLine(DateTime.Now);
                Console.WriteLine(message.DateCreated);

                return Task.FromResult(new MessageReply()
                {
                    Creator = db.Users.First(u => u.ID == message.ID_Creator).FirstName,
                    DateCreated = Timestamp.FromDateTimeOffset(message.DateCreated),
                    IdChat = message.ID_Chat,
                    IdMessage = message.ID,
                    Text = message.TextMessage,
                    IdCreator = message.ID_Creator
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw new RpcException(new Status(StatusCode.Internal, "add database error"));
            }
        }

        public override Task<MessageReply> DeleteMessage(DeleteMessageRequest request, ServerCallContext context)
        {
            try
            {
                Message message = db.Messages.First(u => u.ID == request.IdMessage);
                db.Messages.Remove(message);

                db.SaveChanges();
                return Task.FromResult(new MessageReply()
                {
                    Creator = db.Users.First(u => u.ID == message.ID_Creator).FirstName,
                    DateCreated = Timestamp.FromDateTimeOffset(message.DateCreated),
                    IdChat = message.ID_Chat,
                    IdMessage = message.ID,
                    Text = message.TextMessage,
                    IdCreator = message.ID_Creator
                });
            }
            catch (Exception)
            {
                throw new RpcException(new Status(StatusCode.Internal, "delete database error"));
            }
        }

        public override Task<ListMessageReply> GetListMessage(GetListMessageRequest request, ServerCallContext context)
        {
            ListMessageReply listMessage = new ListMessageReply();
            List<MessageReply> replyList = db.Messages.Where(m => m.ID_Chat == request.IdChat)
                                                      .Skip(request.Skip)
                                                      .Take(request.Limit)
                                                      .Select(m => new MessageReply()
                                                      {
                                                            Creator = db.Users.First(u => u.ID == m.ID_Creator).FirstName,
                                                            DateCreated = Timestamp.FromDateTimeOffset(m.DateCreated),
                                                            IdChat = m.ID_Chat,
                                                            IdMessage = m.ID,
                                                            Text = m.TextMessage,
                                                            IdCreator =m.ID_Creator
                                                        })
                                                      .ToList();
            listMessage.Messages.AddRange(replyList);
            return Task.FromResult(listMessage);
        }

        public override Task<MessageReply> UpdateMessage(UpdateMessageRequest request, ServerCallContext context)
        {
            try
            {
                Message message = db.Messages.First(u => u.ID == request.IdMessage);
                message.TextMessage = request.Text;
                db.Messages.Update(message);

                db.SaveChanges();
                return Task.FromResult(new MessageReply()
                {
                    Creator = db.Users.First(u => u.ID == message.ID_Creator).FirstName,
                    DateCreated = Timestamp.FromDateTime(message.DateCreated),
                    IdChat = message.ID_Chat,
                    IdMessage = message.ID,
                    Text = message.TextMessage,
                    IdCreator = message.ID_Creator
                });
            }
            catch (Exception)
            {
                throw new RpcException(new Status(StatusCode.Internal, "update database error"));
            }
        }

    }

}
