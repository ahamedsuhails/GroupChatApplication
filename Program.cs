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

builder.Services.AddCors(opt =>
{
    opt.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://localhost:4200")        
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();

    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseCors();

// configuring signalR hub
app.MapHub<ChatHub>(pattern: "/chat");

app.MapControllers();

app.Run();
