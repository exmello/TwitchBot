using System;
using System.IO;

namespace TwitchBot
{
    public class TwitchResponseWriter
    {

        private readonly Stream _stream;
        private DateTime lastMessageSent = DateTime.MinValue;

        public TwitchResponseWriter(Stream stream)
        {
            _stream = stream;
        }

        public void RespondMessage(string message)
        {
            //safeguard against bot spam
            //TODO: queue messages
            if (lastMessageSent.AddMinutes(2) < DateTime.Now)
            {
                string commandText = string.Format("PRIVMSG #{0} :{1} says {2}\r\n", Config.ChannelName, Config.BotName, message);
                //string commandText = string.Format("{0}!{0}@{0}.tmi.twitch.tv PRIVMSG #{0} :KatBot says {1}\r\n", Config.ChannelName, message);
                Byte[] say = System.Text.Encoding.ASCII.GetBytes(commandText);
                _stream.Write(say, 0, say.Length);
#if DEBUG
                Console.WriteLine("You sent the following message : {0}", commandText);
#endif
                lastMessageSent = DateTime.Now;
            }
        }
    }
}
