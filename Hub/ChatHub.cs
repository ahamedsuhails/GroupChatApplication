using GroupChatApplication.Models;
using Microsoft.AspNetCore.SignalR;

namespace GroupChatApplication.Hub
{
    public class ChatHub : Microsoft.AspNetCore.SignalR.Hub
    {

        //dictionary for storing userRoomConnection details
        private readonly IDictionary<string, UserRoomConnection> _connection;

        public ChatHub(IDictionary<string, UserRoomConnection> connection)
        {
            _connection = connection;
        }

        //method for joining a room
        public async Task JoinRoom(UserRoomConnection userRoomConnection)
        {
            //save connection to dictionary
            _connection[Context.ConnectionId] = userRoomConnection;
            // use signalR Hub methods to join a group
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName: userRoomConnection.RoomName);
            // notify the group
            await Clients.Group(userRoomConnection.RoomName)
                .SendAsync(method: "ReceiveMessage", "Lets Program Bot", $"{userRoomConnection.UserName} has joined the group");
        }

        //method for sending a message in the group
        public async Task SendMessage(string message)
        {
            if (_connection.TryGetValue(Context.ConnectionId, out UserRoomConnection? userRoomConnection))
            {
                await Clients.Group(userRoomConnection.RoomName)
                    .SendAsync(method: "ReceiveMessage", userRoomConnection.UserName, message, DateTime.Now);
            }
        }
    }
}
