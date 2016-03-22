using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TwitchBot.Data;
using TwitchBot.Model;
using TwitchBot.TwitchApi;

namespace TwitchBot.Commands
{
    /// <summary>
    /// Responds to !viewer with how many viewers have watched the current stream from beginning to end
    /// </summary>
    public class Viewers : ICommand, IEvent
    {
        private readonly Regex regViewers;
        private readonly TwitchResponseWriter tw;
        private readonly TwitchApiClient api;
        private readonly IViewerRepository repo;
        private readonly string[] ignoreBots;

        public Viewers(TwitchResponseWriter tw, TwitchApiClient api, IViewerRepository repo)
        {
            this.tw = tw;
            this.api = api;
            this.repo = repo;
            this.ignoreBots = new string[] { "moobot", "nightbot", "whale_bot" };
            this.regViewers = new Regex("^!viewers\\s$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        }

        public bool IsMatch(MessageInfo message)
        {
            return regViewers.IsMatch(message.Content);
        }

        void ICommand.Process(MessageInfo message)
        {
            //get data
            var stream = api.Stream(message.Channel);

            if (stream != null && stream.stream != null)
            {
                string streamID = stream.stream._id;

                int uniqueViewers = repo.GetUniqueViewerCount(message.Channel.Trim(), streamID);

                if (uniqueViewers > 0)
                {
                    tw.RespondMessage(string.Format("There have been {0} unique viewers since the beginning of {1}'s stream", uniqueViewers, message.Channel));
                }
            }
        }

        void IEvent.Process(MessageInfo message)
        {
            //add data

            if(message.Action == MessageActionType.Join)
            {
                var stream = api.Stream(message.Channel);

                if (stream != null && stream.stream != null)
                {
                    string streamID = stream.stream._id;

                    if (message.Username != Config.Nickname && !ignoreBots.Contains(message.Username.ToLowerInvariant()))
                    {
                        repo.AddUpdateViewer(message.Username, message.Channel.Trim(), streamID);
                    }

                    var chatters = api.Chatters(message.Channel.Trim());

                    if (chatters != null)
                    {
                        var users = chatters.chatters.admins
                            .Union(chatters.chatters.global_mods)
                            .Union(chatters.chatters.moderators)
                            .Union(chatters.chatters.staff)
                            .Union(chatters.chatters.viewers)
                            .Where(u => u != Config.Nickname && !ignoreBots.Contains(u.ToLowerInvariant()))
                            .Distinct();

                        repo.AddUpdateViewers(users, message.Channel.Trim(), streamID);
                    }
                }
            }
        }

    }
}
