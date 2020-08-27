using System;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace ChatBotTwitchLib
{
    /// <summary>
    /// Bot class.
    /// Conteins all bot methods for performing basic bot functions.
    /// </summary>
    /// <remarks>
    /// This class can connect/disconnect with chat and also handles occurring events.
    /// </remarks>
    internal class ChatBot
    {
        ConnectionCredentials credensials = new ConnectionCredentials(TwitchBotInfo.ChannelName, TwitchBotInfo.BotToken);

        TwitchClient client;
        
        Random rand;

        /// <summary>
        /// Void method responsible for connecting, throttling chat messages and defining event handler methods.
        /// </summary>
        /// <param name="logging">A bool parameter which define if bot should show the logs or not.</param>
        internal void Connect(bool logging)
        {

            
            client = new TwitchClient();
            client.Initialize(credensials, TwitchBotInfo.ChannelName);

            rand = new Random();


            Console.WriteLine("Connecting...");

            //chat message throttler to abide by Twitch use requirements

            var clientOptions = new ClientOptions
            {
                MessagesAllowedInPeriod = 750,
                ThrottlingPeriod = TimeSpan.FromSeconds(30)
            };
            WebSocketClient customClient = new WebSocketClient(clientOptions);

            //EVENTS

            if (logging)
                client.OnLog += Client_OnLog;

            client.OnConnectionError += Client_OnConnectionError;
            client.OnMessageReceived += Client_OnMessageReceived;
            client.OnChatCommandReceived += Client_OnChatCommandReceived;
            client.OnWhisperReceived += Client_OnWhisperReceived;
            client.OnUserTimedout += Client_OnUserTimedout;
            

            
            
            client.Connect();
            client.OnConnected += Client_OnConnected;


        }

        

        /// <summary>
        /// Event handler which is activated when event of timeouting user occur.
        /// </summary>
        /// <remarks>
        /// When timeout occur bot will send simple message in chat.
        /// </remarks>
        private void Client_OnUserTimedout(object sender, OnUserTimedoutArgs e)
        {
            client.SendMessage(e.UserTimeout.Username, $"Watch yourself {e.UserTimeout.Username}! :/");
        }
        /// <summary>
        /// Event handler which is activated when event of receiving whisper by bot occur.
        /// </summary>
        /// <remarks>
        /// When whispered bot will send simple response. 
        /// </remarks>
        private void Client_OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
        {
            //client.SendWhisper(e.WhisperMessage.Username, "Hi, how can I help you? :)");
            client.SendMessage(e.WhisperMessage.Username, $"@{e.WhisperMessage.Username} Hi, how can I help you? :)");
        }
        /// <summary>
        /// Event handler which is activated when event of connecting to the chat by bot occur.
        /// </summary>
        /// <remarks>
        /// When connected bot will type both in console and also in the chat message.
        /// </remarks>
        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            Console.WriteLine("Connected");
            client.SendMessage(TwitchBotInfo.ChannelName, "Hi everyone!");

        }
        /// <summary>
        /// Event handler which is activated when event of receiving command by bot occur.
        /// </summary>
        private void Client_OnChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
        {
            switch (e.Command.CommandText.ToLower())
            {
                case "dice":
                    int roll = rand.Next(1, 6);
                    client.SendMessage(TwitchBotInfo.ChannelName, $"Roll: {roll}");
                    break;


            }
            
            //this comments work only if you are owner of the channel
            if(e.Command.ChatMessage.DisplayName == TwitchBotInfo.ChannelName)
            {
                switch (e.Command.CommandText.ToLower())
                {
                    case "realname":
                        client.SendMessage(TwitchBotInfo.ChannelName, $"My name is ...");
                        break;


                }
            }
        }


        /// <summary>
        /// Event handler which is activated when event of seeing message in the chat occur.
        /// </summary>
        /// <remarks>
        /// When connected bot will type chat logs in the console.
        /// </remarks>
        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            Console.WriteLine($"[{e.ChatMessage.DisplayName}]:{e.ChatMessage.Message}"); // chat logs

            if (e.ChatMessage.Message.StartsWith("hi", StringComparison.InvariantCultureIgnoreCase))
            {
                client.SendMessage(TwitchBotInfo.ChannelName, $"Hay there {e.ChatMessage.DisplayName}");
            }

            if (e.ChatMessage.Message.Contains("badword"))
            {
                client.TimeoutUser(e.ChatMessage.Channel, e.ChatMessage.Username, TimeSpan.FromMinutes(30), "Bad word! 30 minute timeout!");
            }
                

        }
        /// <summary>
        /// Event handler which is activated when event of connection error occur.
        /// </summary>
        /// <remarks>
        /// Bot will type all connection errors in the console
        /// </remarks>
        private void Client_OnConnectionError(object sender, OnConnectionErrorArgs e)
        {
            Console.WriteLine($"ERROR: {e.Error}");
        }
        /// <summary>
        /// Event handler which is activated when event of receiving logs occur.
        /// </summary>
        /// <remarks>
        /// Bot will type all logs in the console
        /// </remarks>
        private void Client_OnLog(object sender, TwitchLib.Client.Events.OnLogArgs e)
        {
            Console.WriteLine(e.Data);
        }
        /// <summary>
        /// Void method responsible for disconnecting with chat.
        /// </summary>
        internal void Disconnect()
        {
            client.Disconnect();
        }
    }
}