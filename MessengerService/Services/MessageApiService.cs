using Azure;
using Google.Protobuf.WellKnownTypes;
using GreatDatabase.Data;
using GreatDatabase.Data.Enums;
using GreatDatabase.Data.Model;
using Grpc.Core;
using Microsoft.AspNetCore.Components.Forms;



namespace MessengerService.Services
{




    public class MessageApiService : MessageService.MessageServiceBase
    {
        MyDbContext db;
        private static ILogger<MessageApiService> _logger;


        public MessageApiService(ILogger<MessageApiService> logger)
        {
            _logger = logger;
            db = new MyDbContext();
        }


     
        public override Task<MessageReply> CreateMessage(CreateMessageRequest request, ServerCallContext context)
        {
            try
            {

                Message message = new Message()
                {
                    TextMessage = request.TextMessage,
                    DateCreated = DateTime.Now,
                    ChatId = request.ChatId,
                    CreatorId = request.UserId,
                    LastModified = DateTime.Now
                };

                _logger.LogInformation($"chat id = {request.ChatId}");
                db.Messages.AddAsync(message);
                db.SaveChanges();

                _logger.LogInformation($"Create message chat = {request.ChatId}, text = {request.TextMessage}");

                var creator = db.Users.First(u => u.ID == message.CreatorId);

                return Task.FromResult(new MessageReply()
                {
                    CreatorName = creator.FirstName,
                    DateCreated = Timestamp.FromDateTimeOffset(message.DateCreated),
                    ChatId = message.ChatId,
                    MessageId = message.ID,
                    TextMessage = message.TextMessage,
                    CreatorTag = creator.UserTag,                    
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                _logger.LogError(e.InnerException.Message);
                throw new RpcException(new Status(StatusCode.Internal, "add database error"));
            }
        }

        public async override Task<VoidMessageReply> DeleteMessage(DeleteMessageRequest request, ServerCallContext context)
        {
            try
            {
                Message message = db.Messages.First(u => u.ID == request.MessageId);
                db.Messages.Remove(message);

                await db.SaveChangesAsync();

                _logger.LogInformation($"Delete message: {request.MessageId}, chat = {request.ChatId}");

                return new VoidMessageReply();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new RpcException(new Status(StatusCode.Internal, "delete database error"));
            }
        }

        public override Task<ListMessageReply> GetListMessage(GetListMessageRequest request, ServerCallContext context)
        {

            ListMessageReply listMessage = new ListMessageReply();


            List<MessageReply> replyList = db.Messages.Where(m => m.ChatId == request.ChatId)
                                                      .OrderByDescending(m => m.ID)
                                                      .Skip(request.Skip)
                                                      .Take(request.Limit)
                                                      .Select(m => new MessageReply()
                                                      {
                                                          CreatorName = db.Users.First(u => u.ID == m.CreatorId).FirstName,
                                                          DateCreated = Timestamp.FromDateTimeOffset(m.DateCreated),
                                                          ChatId = m.ChatId,
                                                          MessageId = m.ID,
                                                          TextMessage = m.TextMessage,
                                                          CreatorTag = db.Users.First(u => u.ID == m.CreatorId).UserTag
                                                      })
                                                      .ToList();

            _logger.LogInformation($"Return {replyList.Count} messages from chat with id = {request.ChatId}");

            listMessage.Messages.AddRange(replyList);
            return Task.FromResult(listMessage);
        }

        public async override Task<VoidMessageReply> UpdateMessage(UpdateMessageRequest request, ServerCallContext context)
        {
            try
            {
                Message message = db.Messages.First(u => u.ID == request.MessageId);
                message.TextMessage = request.TextMessage;
               
                db.Messages.Update(message);
                await db.SaveChangesAsync();

                _logger.LogInformation($"Update message: {request.MessageId}, chat = {request.ChatId}");

                return new VoidMessageReply();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new RpcException(new Status(StatusCode.Internal, "update database error"));
            }
        }


    }

}
