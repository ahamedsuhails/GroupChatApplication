import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ChatService {
  //building connection with backend
  public connection: signalR.HubConnection = new signalR.HubConnectionBuilder()
    .withUrl('http://localhost:5000/chat')
    .configureLogging(signalR.LogLevel.Information)
    .build();

  //use behavior subjects to get realtime data from the server
  public messages$ = new BehaviorSubject<any>([]);
  public connectedUsers$ = new BehaviorSubject<string[]>([]);
  public messages: any[] = [];
  public connectedUsers: string[] = [];

  constructor() {
    this.start();

    this.connection.on(
      'ReceiveMessage',
      (user: string, message: string, messageTime: string) => {
        this.messages = [...this.messages, { user, message, messageTime }];
        this.messages$.next(this.messages);
      }
    );

    this.connection.on('ConnectedUsers', (users: any) => {
      console.log('Connected Users: ', users);
      this.connectedUsers$.next(users);
    });
  }

  //start connection
  public async start() {
    try {
      await this.connection.start();
      console.log('SignalR connection is established.');
    } catch (ex) {
      console.log(ex);
      // restart connection every 5 second
      setTimeout(() => {
        this.start();
      }, 5000);
    }
  }

  //join room
  public async joinRoom(user: string, room: string) {
    return this.connection.invoke('JoinRoom', { user, room });
  }

  //send messages
  public async sendMessages(message: string) {
    return this.connection.invoke('SendMessage', message);
  }

  //leave chat
  public async leaveChat() {
    return this.connection.stop();
  }
}
