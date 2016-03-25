using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace TwitchBot
{
    public class TwitchResponseWriter
    {

        private readonly Stream _stream;
        private DateTime lastMessageSent = DateTime.MinValue;
        private Queue<string> messageQueue = new Queue<string>();
        private Object syncLock = new Object();

        public TwitchResponseWriter(Stream stream)
        {
            _stream = stream;
        }

        public void RespondMessage(string message)
        {
            //if the message queue is small, add.  otherwise only add if unique
            //Queue.Contains might be an expensive operation?
            if (messageQueue.Count < 4 || !messageQueue.Contains(message))
            {
                messageQueue.Enqueue(message);
            }

            Task.Run(() => RespondFromQueue());
        }

        public void RespondFromQueue()
        {
            lock (syncLock)
            {
                while (messageQueue.Count > 0)
                {
                    //safeguard against bot spam
                    if (lastMessageSent.AddSeconds(2) < DateTime.Now)
                    {
                        string message = messageQueue.Dequeue();

                        string commandText = string.Format("PRIVMSG #{0} :{1}\r\n", Config.ChannelName, message);
                        //string commandText = string.Format("{0}!{0}@{0}.tmi.twitch.tv PRIVMSG #{0} :KatBot says {1}\r\n", Config.ChannelName, message);
                        WriteToStream(commandText);
#if DEBUG
                        Console.WriteLine("You sent the following message : {0}", commandText);
#endif
                        lastMessageSent = DateTime.Now;
                    }

                    
                }
            }
        }

        public void WriteToStream(string text)
        {
            Byte[] bytes = System.Text.Encoding.UTF8.GetBytes(text);
            _stream.Write(bytes, 0, bytes.Length);
        }

        public void WriteToStreamUnicode(string text)
        {
            Byte[] bytes = System.Text.Encoding.Unicode.GetBytes(text);
            _stream.Write(bytes, 0, bytes.Length);
        }

    }
}
