using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TwitchBot.Model;

namespace TwitchBot.Commands
{
    /// <summary>
    /// Responds to !howlong with a message about how long a user has followed a channel
    /// </summary>
    public class HowLong : ICommand
    {
        private readonly Regex regHowLong;
        private readonly TwitchResponseWriter tw;
        private readonly TwitchApiClient api;

        public HowLong(TwitchResponseWriter tw, TwitchApiClient api)
        {
            this.tw = tw;
            this.api = api;
            this.regHowLong = new Regex("^!howlong\\s(?<user>[0-9a-zA-Z_]*?)\\s$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        }

        public bool IsMatch(MessageInfo message)
        {
            return regHowLong.IsMatch(message.Content);
        }

        public void Process(MessageInfo message)
        {
            Match match = regHowLong.Match(message.Content);
            if (match.Success)
            {
                if (!string.IsNullOrWhiteSpace(match.Groups["user"].Value))
                {
                    if (match.Groups["user"].Value.ToLowerInvariant() == "roflgator")
                        tw.RespondMessage("16 inches");
                    else
                        RespondFollowMesssage(match.Groups["user"].Value, message.Channel);
                }
                else
                    RespondFollowMesssage(message.Username, message.Channel);
            }
        }

        private void RespondFollowMesssage(string username, string channel)
        {
            var followData = api.FollowTarget(username, channel);

            if (followData != null)
            {
                DateTime created = DateTime.SpecifyKind(followData.created_at, DateTimeKind.Utc).ToLocalTime();
                int months = ((DateTime.Now.Year - created.Year) * 12) + DateTime.Now.Month - created.Month;
                if (months > 0)
                {
                    tw.RespondMessage(string.Format("user @{0} has been following {1} for {2} month{3}!"
                        , username, channel, months, months == 1 ? string.Empty : "s"));
                }
                else
                {
                    int days = Convert.ToInt32((DateTime.Now - followData.created_at).TotalDays);
                    tw.RespondMessage(string.Format("user @{0} has been following {1} for {2} day{3}!"
                        , username, channel, days, days == 1 ? string.Empty : "s"));
                }
            }
        }
    }
}
