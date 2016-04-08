using System.Collections.Generic;
using TwitchBot.Model;

namespace TwitchBot.Data
{
    public interface IViewerRepository
    {
        void AddUpdateViewer(string user, string channel, string streamID);
        void AddUpdateViewers(IEnumerable<string> users, string channel, string streamID);
        int GetUniqueViewerCount(string channel, string streamID);

        void AddUpdateBnet(string user, string bnet);
        void AddUpdateNote(string user, string note);
        ViewerInfo GetInfo(string user);
    }
}
