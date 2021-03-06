﻿//Written by Danny Walker (exmello) March 2016
//Original Framework from: http://chhopsky.tv/twitch-chat-csharpdotnet/ 

using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using TwitchBot.Akinator;
using TwitchBot.Commands;
using TwitchBot.Data;
using TwitchBot.Model;
using TwitchBot.TwitchApi;

namespace TwitchBot
{
    public class Program
    {
        public static void Main(string[] args)
        {            
            // Get a client stream for reading and writing.
            using (TwitchChatConnection connection = new TwitchChatConnection(Config.ChatServer, Config.Port))
            {
                // Send login request
                Console.WriteLine("Sent login.\r\n");
                string responseData = connection.SendLoginRequest(Config.OAuth, Config.Nickname);
                Console.WriteLine("Received WELCOME: \r\n\r\n{0}", responseData);

                // send message to join channel
                connection.JoinChannel(Config.ChannelName);
                Console.WriteLine("Sent channel join.\r\n");

                // subscribe to events
                responseData = connection.SubcribeToMembershipEvents(Config.ChannelName);
                Console.WriteLine("Subcribe to JOIN/PART: \r\n\r\n{0}", responseData);

                //twitch JSON api for bots to use
                TwitchApiClient api = new TwitchApiClient();

                //db storage
                IViewerRepository viewerDb = new SqlViewerRepository();
                IDictionaryRepository dictionaryDb = new SqlDictionaryRepository();
                IChannelRepository channelDb = new SqlChannelRepository();
                IQuoteRepository quoteDb = new SqlQuoteRepository();
                IBlasphemyRepository blasphDb = new SqlBlasphemyRepository();

                //start bot   
                KatBot katbot = new KatBot(connection.Writer, api);
                AkinatorBot akinatorBot = new AkinatorBot(connection.Writer, api);

                var quoteCommand = new Commands.Quote(connection.Writer, channelDb, quoteDb);

                //add commands
                katbot.CommandList.Add(new Commands.Command(connection.Writer));
                katbot.CommandList.Add(new Commands.HowLong(connection.Writer, api));
                katbot.CommandList.Add(new Commands.Uptime(connection.Writer, api));
                katbot.CommandList.Add(new Commands.Viewers(connection.Writer, api, viewerDb));
                katbot.CommandList.Add(new Commands.Madlib(connection.Writer, dictionaryDb));
                katbot.CommandList.Add(new Commands.FullWidth(connection.Writer));
                katbot.CommandList.Add(new Commands.Define(connection.Writer, dictionaryDb));
                katbot.CommandList.Add(new Commands.Blasphemy(connection.Writer, api, blasphDb));
                katbot.CommandList.Add(quoteCommand);
                katbot.CommandList.Add(new Commands.Bnet(connection.Writer, viewerDb));
                katbot.CommandList.Add(new Commands.Note(connection.Writer, viewerDb));

                katbot.KeywordProcessors.Add(new Commands.Question(connection.Writer, api, dictionaryDb));
                katbot.KeywordProcessors.Add(new Commands.KeywordMatcher(connection.Writer, channelDb));
                katbot.KeywordProcessors.Add(quoteCommand);
                katbot.KeywordProcessors.Add(new Commands.UrlExpander(connection.Writer));

                //Start message loop
                while (true)
                {
                    MessageInfo message = connection.ReadMessage();

                    if (message != null)
                    {
                        katbot.ProcessMessage(message);
                        akinatorBot.ProcessMessage(message);
                        akinatorBot.Update();
                    }
                }
            }
        }
    }
}