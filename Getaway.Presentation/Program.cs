using Getaway.Infrustructure;
using Getaway.Application;
using Getaway.Presentation.Hubs;
using Microsoft.AspNetCore.Http.HttpResults;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSignalR();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddInfrustructure();
builder.Services.AddApplication();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapHub<ChatHub>("/online-chat");
app.MapHub<TeamHub>("/online-teams");
app.MapHub<ProjectHub>("/online-projects");


app.MapGet("{userId}/api/try-connection", async (int userId) =>
{
    Console.WriteLine($"User with id = {userId} is connected");
    return Results.Ok();
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
