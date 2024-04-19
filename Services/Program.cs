using Server;
using Server.Data;
using Server.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<UserApiService>();
app.MapGrpcService<MessageApiService>();
app.MapGrpcService<ProjectApiService>();
app.MapGrpcService<ProjectTaskApiService>();
app.MapGrpcService<ChatApiService>();
app.MapGrpcService<TeamApiService>();
app.MapGrpcService<LoginApiService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();


class Config : IDisposable
{

    public static MyDbContext myDbContext = new MyDbContext();
    public void Dispose()
    {
        myDbContext.Dispose();
    }
}