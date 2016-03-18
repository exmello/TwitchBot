using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TwitchBot.Model;

namespace TwitchBot
{
    public class TwitchChatConnection : IDisposable
    {
        private readonly TwitchRawMessageProcessor _msgProc;
        private readonly TcpClient _client;
        
        public NetworkStream Stream { get; private set; }
        public TwitchResponseWriter Writer { get; private set; }

        public TwitchChatConnection(string chatServer, int port)
        {
            _client = new TcpClient(chatServer, port);
            Stream = _client.GetStream();

            Writer = new TwitchResponseWriter(Stream);

            _msgProc = new TwitchRawMessageProcessor();
        }

        /// <summary>
        /// Read a response of an expected length
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public string ReadResponse(int length)
        {
            byte[] data = new byte[length];
            Int32 bytes = Stream.Read(data, 0, data.Length);
            return System.Text.Encoding.ASCII.GetString(data, 0, bytes);
        }

        /// <summary>
        /// Read a response of an arbitrary length
        /// </summary>
        /// <returns></returns>
        public string ReadResponse()
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
                    numberOfBytesRead = Stream.Read(readBuffer, 0, readBuffer.Length);

                    sbMessage.Append(Encoding.ASCII.GetString(readBuffer, 0, numberOfBytesRead));
                }
                while (Stream.DataAvailable);
            }
            catch (Exception e)
            {

                Console.WriteLine("Error reading message\r\n", e);
            }

            return sbMessage.ToString();
        }

        public MessageInfo ReadMessage()
        {
            string rawMessage = ReadResponse();
            MessageInfo message = null;

#if TRACE
            // Print out the received message.
            Console.WriteLine(rawMessage);
#endif
            // Every 5 minutes the Twitch server will send a PING, this is to respond with a PONG to keepalive
            if(rawMessage == "PING :tmi.twitch.tv\r\n")
            {
                try
                {
                    Writer.WriteToStream("PONG :tmi.twitch.tv\r\n");
#if DEBUG
                    Console.WriteLine("Ping? Pong!");
#endif
                }
                catch (Exception e)
                {
                    //TODO: write to logfile
                    Console.WriteLine("Exception in PING handler\r\n", e);
                }
            }
            else
            {
                try
                {
                    message = _msgProc.GetMessageInfo(rawMessage);                   
#if TRACE
                    Console.WriteLine("You received the following message : {0}", rawMessage);
#endif
                }
                catch (Exception e)
                {
                    //TODO: write to logfile
                    Console.WriteLine("Exception in message handler\r\n", e);
                }
            }

            return message;
        }

        public void Dispose()
        {
            if (Stream != null)
            {
                Stream.Close();
                Stream.Dispose();
            }

            if (_client != null)
            {
                _client.Close();
            }   
            
        }

        public string SendLoginRequest(string oauth, string nickname)
        {
            Writer.WriteToStream(string.Format("PASS oauth:{0}\r\nNICK {1}\r\n", oauth, nickname));

            // Read the first batch of the TcpServer response bytes.
            return ReadResponse(512);
        }

        public void JoinChannel(string channelName)
        {
            Writer.WriteToStream(string.Format("JOIN #{0}\r\n", channelName));
        }
    }
}
