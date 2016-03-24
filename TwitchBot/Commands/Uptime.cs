using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TwitchBot.Model;
using TwitchBot.TwitchApi;

namespace TwitchBot.Commands
{
    /// <summary>
    /// Responds to !uptime with how long the current channel has been active
    /// </summary>
    public class Uptime : ICommand
    {
        private readonly TwitchResponseWriter tw;
        private readonly TwitchApiClient api;
        private readonly Regex regUptime;

        public Uptime(TwitchResponseWriter tw, TwitchApiClient api)
        {
            this.tw = tw;
            this.api = api;
            this.regUptime = new Regex("^!uptime\\s(?<channel>[0-9a-zA-Z_]*?)\\s$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        }

        public bool IsMatch(MessageInfo message)
        {
            return regUptime.IsMatch(message.Content);// || regHowWet.IsMatch(message.Content);
        }

        public void Process(MessageInfo message)
        {
            Match match = regUptime.Match(message.Content);
            if (match.Success)
            {
                if (!string.IsNullOrWhiteSpace(match.Groups["channel"].Value))
                {
                    RespondUptimeMesssage(match.Groups["channel"].Value);
                }
                else
                {
                    RespondUptimeMesssage(message.Channel);
                }
                return;
            }
        }

        private void RespondUptimeMesssage(string channel, bool wet = false)
        {
            var streamData = api.Stream(channel);

            if(streamData != null)
            {
                if (streamData.stream != null)
                {
                    DateTime created = DateTime.SpecifyKind(streamData.stream.created_at, DateTimeKind.Utc).ToLocalTime();
                    TimeSpan uptime = DateTime.Now - created;

                    StringBuilder sb = new StringBuilder();
                    
                    sb.AppendFormat("@{0} has been streaming for", channel);

                    if (uptime.Days == 1)
                        sb.AppendFormat(" {0} day", uptime.Days);
                    else if (uptime.Days > 1)
                        sb.AppendFormat(" {0} days", uptime.Days);

                    if (uptime.Hours == 1)
                        sb.AppendFormat(" {0} hour", uptime.Hours);
                    else if (uptime.Hours > 1)
                        sb.AppendFormat(" {0} hours", uptime.Hours);
                    else if (uptime.Hours == 0 && uptime.Days > 0)
                        sb.Append(" 0 hours");

                    if (uptime.Minutes == 1)
                        sb.AppendFormat(" {0} minute", uptime.Minutes);
                    else if (uptime.Minutes > 1)
                        sb.AppendFormat(" {0} minutes", uptime.Minutes);
                    else if (uptime.Minutes == 0 && uptime.Hours > 0)
                        sb.Append(" 0 minutes");

                    if (uptime.Seconds == 1)
                        sb.AppendFormat(" {0} second", uptime.Seconds);
                    else if (uptime.Seconds > 1)
                        sb.AppendFormat(" {0} seconds", uptime.Seconds);

                    sb.AppendLine();

                    tw.RespondMessage(sb.ToString());
                }
                else
                {
                    tw.RespondMessage(string.Format("{0} is not currently streaming!", channel));
                }
            }
        }
    }
}
