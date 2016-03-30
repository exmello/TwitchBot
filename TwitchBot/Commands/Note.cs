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
    /// Lets users add a note with !note [note], or query another user's note with !note [username]
    /// </summary>
    public class Note : ICommand
    {
        private readonly Regex regCommand;
        private readonly Regex regNote;
        private readonly Regex regUser;
        private readonly TwitchResponseWriter tw;
        private readonly IViewerRepository repo;

        public Note(TwitchResponseWriter tw, IViewerRepository repo)
        {
            this.tw = tw;
            this.repo = repo;
            this.regCommand = new Regex("^!note\\s", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
            this.regUser = new Regex("^!note\\s(?<user>[0-9a-zA-Z_]+?)\\s$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
            this.regNote = new Regex("^!note\\s(?<note>.*?)\\s$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        }

        public bool IsMatch(MessageInfo message)
        {
            return regCommand.IsMatch(message.Content);
        }

        void ICommand.Process(MessageInfo message)
        {

            Match match = regUser.Match(message.Content);

            if (match.Success)
            {
                if ((!string.IsNullOrWhiteSpace(match.Groups["user"].Value)))
                {
                    //query note by username
                    GetNote(match.Groups["user"].Value);

                    return;
                }
            }

            match = regNote.Match(message.Content);

            if (match.Success)
            {
                if (!string.IsNullOrWhiteSpace(match.Groups["note"].Value))
                {
                    //update user bnet info
                    string bnet = match.Groups["note"].Value;

                    repo.AddUpdateNote(message.Username, bnet);

                    tw.RespondMessage(string.Format("Updated note for {0}.", message.Username));
                }
                else
                {
                    //query note by current user
                    GetNote(message.Username);
                }

            }
        }

        private void GetNote(string user)
        {
            ViewerInfo info = repo.GetInfo(user);

            if (info != null && !string.IsNullOrEmpty(info.Note))
            {
                tw.RespondMessage(string.Format("Note for {0}: {1}", info.Username, info.Note));
            }
            else
            {
                tw.RespondMessage(string.Format("No note found for {0}.", user));
            }
        }

    }
}
