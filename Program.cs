using GroupChatApplication.Hub;
using GroupChatApplication.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

builder.Services.AddSingleton<IDictionary<string, UserRoomConnection>>(opt => 
    new Dictionary<string, UserRoomConnection>());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

// configuring signalR hub
app.MapHub<ChatHub>(pattern: "/chat");

app.MapControllers();

app.Run();
