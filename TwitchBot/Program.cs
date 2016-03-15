//Written by Danny Walker (exmello) March 2016
//Original Framework from: http://chhopsky.tv/twitch-chat-csharpdotnet/ 

using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace TwitchBot
{
    public class Program
    {
        private static byte[] data;

        public static void Main(string[] args)
        {
            Int32 port = 6667;
            string[] ignoreBots = { "moobot", "nightbot", "whale_bot" };
            
            // Get a client stream for reading and writing.
            using(TcpClient client = new TcpClient("irc.twitch.tv", port))
            using (NetworkStream stream = client.GetStream())
            {

                string channel = Config.ChannelName;
                string oath = Config.OAuth;
                string nick = Config.Nickname;

                // Send the message to the connected TcpServer. 
                string loginstring = string.Format("PASS oauth:{0}\r\nNICK {1}\r\n", oath, nick);
                Byte[] login = System.Text.Encoding.ASCII.GetBytes(loginstring);
                stream.Write(login, 0, login.Length);

                Console.WriteLine("Sent login.\r\n");
                Console.WriteLine(loginstring);

                // Receive the TcpServer.response.
                // Buffer to store the response bytes.
                data = new Byte[512];

                // String to store the response ASCII representation.
                String responseData = String.Empty;

                // Read the first batch of the TcpServer response bytes.
                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                Console.WriteLine("Received WELCOME: \r\n\r\n{0}", responseData);

                // send message to join channel

                string joinstring = string.Format("JOIN #{0}\r\n", channel);
                Byte[] join = System.Text.Encoding.ASCII.GetBytes(joinstring);
                stream.Write(join, 0, join.Length);
                Console.WriteLine("Sent channel join.\r\n");
                Console.WriteLine(joinstring);

                //start bot               
                TwitchResponseWriter tw = new TwitchResponseWriter(stream);
                KatBot katbot = new KatBot(tw);

                while (true)
                {
                    // build a buffer to read the incoming TCP stream to, convert to a string
                    byte[] readBuffer = new byte[1024];
                    StringBuilder sbMessage = new StringBuilder();
                    int numberOfBytesRead = 0;

                    try
                    { 
                        // Incoming message may be larger than the buffer size.
                        do
                        {
                            //TODO: could put a continue statement here if testing for !commands
                            numberOfBytesRead = stream.Read(readBuffer, 0, readBuffer.Length);                        

                            sbMessage.AppendFormat("{0}", Encoding.ASCII.GetString(readBuffer, 0, numberOfBytesRead));
                        }
                        while (stream.DataAvailable); 
                    }
                    catch (Exception e)
                    {
                        //TODO: log error
                        Console.WriteLine("Error reading message\r\n", e);
                        continue; //give up and wait for next message
                    }

#if DEBUG
                    // Print out the received message to the console.
                    Console.WriteLine(sbMessage.ToString());
#endif

                    switch (sbMessage.ToString())
                    {
                        // Every 5 minutes the Twitch server will send a PING, this is to respond with a PONG to keepalive
                        case "PING :tmi.twitch.tv\r\n":
                            try
                            {
                                Byte[] say = System.Text.Encoding.ASCII.GetBytes("PONG :tmi.twitch.tv\r\n");
                                stream.Write(say, 0, say.Length);
#if DEBUG
                                Console.WriteLine("Ping? Pong!");
#endif
                            }
                            catch (Exception e)
                            {
                                //TODO: log error
                                Console.WriteLine("Exception in PING handler\r\n", e);
                            }
                            break;

                        // If it's not a ping, try to parse it for a message.
                        default:
                            try
                            {
                                string messageParser = sbMessage.ToString();
                                string[] message = messageParser.Split(':');
                                string[] preamble = message[1].Split(' ');
                                string[] sendingUser = preamble[0].Split('!');

                                // This means it's a message to the channel.  Yes, PRIVMSG is IRC for messaging a channel too
                                if (preamble[1] == "PRIVMSG")
                                {  
                                    // Ignore some well known bots
                                    if (!ignoreBots.Contains(sendingUser[0].ToLowerInvariant()))
                                    {
                                        katbot.ProcessMessage(sendingUser[0], message[2]);
                                    }
                                }
                                // A user joined.
                                else if (preamble[1] == "JOIN")
                                {
                                    katbot.ProcessJoinEvent(sendingUser[0]);
                                }

#if DEBUG
                                Console.WriteLine("Raw output: {0}::{1}::{2}", message[0], message[1], message[2]);
                                Console.WriteLine("You received the following message : {0}", sbMessage);
#endif
                            }
                            catch (Exception e)
                            {
                                //TODO: write to logfile
                                Console.WriteLine("Exception in message handler\r\n", e);
                            }

                            break;
                    }
                }
            }

        }
    }
}