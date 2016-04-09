using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchBot.Model
{
    public class ChannelForDashboardResult
    {
        public string Name { get; set; }
        public int StreamCount { get; set; }
        public int KeywordCount { get; set; }
        public int QuoteCount { get; set; }
        public decimal QuoteIncreasePercent { get; set; }
    }
}
