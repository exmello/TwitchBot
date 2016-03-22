using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchBot.Data
{
    public interface IViewerRepository
    {
        void AddUpdateViewer(string user, string channel, string streamID);
        void AddUpdateViewers(IEnumerable<string> users, string channel, string streamID);
        int GetUniqueViewerCount(string channel, string streamID);
    }
}
