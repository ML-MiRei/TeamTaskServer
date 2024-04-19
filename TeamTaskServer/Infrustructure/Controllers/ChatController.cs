using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelsLibrary;
using ModelsLibrary.Entities;
using System;
using TeamTaskServerAPI;

namespace ApiGetaway.Infrustructure.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        //private static ChatService.ChatServiceClient _chatServiceClient = new ChatService.ChatServiceClient(Config.GrpcChannel);
        //private static MessageService.MessageServiceClient _messageServiceClient = new MessageService.MessageServiceClient(Config.GrpcChannel);
        //private static UserService.UserServiceClient _userServiceClient = new UserService.UserServiceClient(Config.GrpcChannel);

        //[HttpGet("user-id={userId}")]
        //public async Task<ActionResult> GetList(int userId)
        //{
        //    List<Chat> chats = new List<Chat>();
        //    List<ListChatItem> chatsReply = (await _chatServiceClient.GetListChatsAsync(new GetListChatsRequest() { IdUser = userId })).Chats.ToList();
        //    for (int i = 0; i < chatsReply.Count; i++)
        //    {
        //        Chat chat = await GetMessagesAndUsersChat(chatId: chatsReply[i].IdChat);
        //        chat.Name = chatsReply[i].Name;
        //        chat.ID = chatsReply[i].IdChat;
        //        chat.Type = chatsReply[i].ChatTypeN;
        //        chat.Image = chatsReply[i].Image;

        //        chats.Add(chat);
        //    }

        //    return Ok(chats);
        //}

        //private async Task<Chat> GetMessagesAndUsersChat(int chatId)
        //{
        //    var messages = (await _messageServiceClient.GetListMessageAsync(new GetListMessageRequest()
        //    {
        //        IdChat = chatId,
        //        Limit = 20,
        //        Skip = 0
        //    })).Messages.Select(m => new Message()
        //    {
        //        ChatId = m.IdChat,
        //        DateCreated = m.DateCreated.ToDateTime(),
        //        ID = m.IdMessage,
        //        Text = m.Text,
        //        UserNameCreator = m.Creator
        //    }).ToList();

        //    var users = (await _userServiceClient.GetUserByChatAsync(new GetUserByChatRequest()
        //    {
        //        IdChat = chatId
        //    })).Users.Select(u => new User()
        //    {
        //        Email = u.Email,
        //        FirstName = u.FirstName,
        //        LastName = u.LastName,
        //        SecondName = u.SecondName,
        //        PhoneNumber = u.Phone,
        //        Tag = u.Tag
        //    }).ToList();

        //    return new Chat()
        //    {
        //        Messages = messages,
        //        Users = users
        //    };
        //}


        //[HttpPost("private-chat/user-id={userId}&second-user-tag={secondUserTag}")]
        //public async Task<ActionResult<Chat>> PostPrivateChat(int userId, string secondUserTag)
        //{
        //    try
        //    {
        //        var chatReply = await _chatServiceClient.CreateChatsAsync(new CreateChatRequest() { IdUser = userId, UserTag = secondUserTag });
        //        Chat chat = await GetMessagesAndUsersChat(chatReply.IdChat);
        //        chat.Type = chatReply.ChatTypeN;
        //        chat.Name = chatReply.Name;
        //        chat.ID = chatReply.IdChat;
        //        chat.Image = chatReply.Image;
        //        return new ActionResult<Chat>(chat);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //        return new EmptyResult();
        //    }

        //}


        //[HttpPost("group-chat/user-id={userId}&group-chat-name={name}")]
        //public async Task<ActionResult> PostGroupChat(int userId, string name)
        //{
        //    try
        //    {
        //        await _chatServiceClient.CreateGroupAsync(new CreateGroupRequest() { IdUser = userId, Name = name });
        //        return Ok();
        //    }
        //    catch (Exception)
        //    {
        //        return new EmptyResult();
        //    }
        //}



    }
}
