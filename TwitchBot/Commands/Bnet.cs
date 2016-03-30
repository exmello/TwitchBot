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
    /// Lets users add their bnet info with !bnet [bnet#], or query another user's info with !bnet [username]
    /// </summary>
    public class Bnet : ICommand
    {
        private readonly Regex regCommand;
        private readonly Regex regBnet;
        private readonly Regex regUser;
        private readonly TwitchResponseWriter tw;
        private readonly IViewerRepository repo;

        public Bnet(TwitchResponseWriter tw, IViewerRepository repo)
        {
            this.tw = tw;
            this.repo = repo;
            this.regCommand = new Regex("^!bnet\\s", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
            this.regBnet = new Regex("^!bnet\\s(?<bnet>[0-9a-zA-Z]+#[0-9]{4}?)\\s$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
            this.regUser = new Regex("^!bnet\\s(?<user>[0-9a-zA-Z_]*?)\\s$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        }

        public bool IsMatch(MessageInfo message)
        {
            return regCommand.IsMatch(message.Content);
        }

        void ICommand.Process(MessageInfo message)
        {
            Match match = regBnet.Match(message.Content);

            if (match.Success)
            {
                if (!string.IsNullOrWhiteSpace(match.Groups["bnet"].Value))
                {
                    //update user bnet info
                    string bnet = match.Groups["bnet"].Value;

                    repo.AddUpdateBnet(message.Username, bnet);

                    tw.RespondMessage(string.Format("Updated bnet info for {0}.", message.Username));
                }

                return;
            }

            match = regUser.Match(message.Content);

            if (match.Success)
            {
                if ((!string.IsNullOrWhiteSpace(match.Groups["user"].Value)))
                {
                    //query bnet info by username
                    GetBnetInfo(match.Groups["user"].Value);
                }
                else
                {
                    //query bnet info by current user
                    GetBnetInfo(message.Username);
                }
            }
        }

        private void GetBnetInfo(string user)
        {
            ViewerInfo info = repo.GetInfo(user);

            if (info != null && !string.IsNullOrEmpty(info.Bnet))
            {
                tw.RespondMessage(string.Format("Bnet info for {0}: {1}", info.Username, info.Bnet));
            }
            else
            {
                tw.RespondMessage(string.Format("No bnet info found for {0}.", user));
            }
        }

    }
}
