using Getaway.Application.ReturnsModels;
using Getaway.Core.Entities;
using Grpc.Net.Client;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using static System.Net.Mime.MediaTypeNames;

//HttpClient httpClient = new HttpClient();
//var s = await httpClient.GetAsync("https://localhost:7047/api/User/authorize/email=em1&password=q1w");
//string res = s.Content.ReadAsStringAsync().Result;

//Console.WriteLine(res);

GrpcChannel channel = GrpcChannel.ForAddress("https://localhost:7024");


//HubConnection _hubConnection = new HubConnectionBuilder()
//                .WithUrl("https://localhost:7130/chat")
//.Build();

//await _hubConnection.StartAsync();
//_hubConnection.On<int, MessageModel>("Receive", (chatId, messageModel) =>
//{
//    Console.WriteLine($"{messageModel.UserNameCreator}: {messageModel.TextMessage}");
//});


//while(Console.ReadKey().Key != ConsoleKey.Escape)
//{
//    var mess = Console.ReadLine();
//    await _hubConnection.SendAsync("Send", 2, 4, mess);
//}

//await _hubConnection.StopAsync();

HttpClient client = new HttpClient();

UserEntity userEntity = new UserEntity()
{

    FirstName = "dd",
    SecondName = "dd",
    LastName = "dd",
    Email = "tt",
    Password = "dd",
    PhoneNumber = "67867867678"
};

var c =  JsonContent.Create("uu");

var reply = await client.PostAsync("https://localhost:7130/1/api/Team/create", JsonContent.Create("uu"));

Console.WriteLine(reply.StatusCode);


var tt = await reply.Content.ReadFromJsonAsync<TeamModel>();
Console.WriteLine(reply.Content.ReadFromJsonAsync<TeamModel>().Id);


var b = JsonContent.Create(userEntity);

var httpReply = await client.PostAsync($"https://localhost:7130/api/Authentication/registration", b);

var y = await httpReply.Content.ReadFromJsonAsync<UserEntity>();

var t = httpReply.Content;

Console.ReadKey();