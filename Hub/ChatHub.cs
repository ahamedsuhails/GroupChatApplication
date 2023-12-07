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
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName: userRoomConnection.Room);
            // notify the group
            await Clients.Group(userRoomConnection.Room)
                .SendAsync(method: "ReceiveMessage", "Lets Program Bot", $"{userRoomConnection.User} has joined the group", DateTime.Now);
            await SendConnectedUser(userRoomConnection.Room);
        }

        //method for sending a message in the group
        public async Task SendMessage(string message)
        {
            if (_connection.TryGetValue(Context.ConnectionId, out UserRoomConnection? userRoomConnection))
            {
                await Clients.Group(userRoomConnection.Room)
                    .SendAsync(method: "ReceiveMessage", userRoomConnection.User, message, DateTime.Now);
            }
        }

        // to send user disconnected notification
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            if(!_connection.TryGetValue(Context.ConnectionId, out UserRoomConnection? roomConnection))
            {
                return base.OnDisconnectedAsync(exception);
            }
            Clients.Group(roomConnection.Room)
                .SendAsync(method: "ReceiveMessage", "Lets Program Bot", $"{roomConnection.User} has left the group", DateTime.Now);
            SendConnectedUser(roomConnection.Room);
            return base.OnDisconnectedAsync(exception);
        }

        //to get the list of users for a specific room
        public Task SendConnectedUser(string room)
        {
            var users = _connection.Values.Where(x => x.Room == room).Select(x => x.User);
            return Clients.Group(room)
                .SendAsync(method: "ConnectedUsers", users);
        }
    }
}
