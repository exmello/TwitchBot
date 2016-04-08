using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchBot.TwitchApi.Result
{
    public class StreamResult
    {
        public StreamObject stream { get; set; }

        public class StreamObject
        {
            public string _id { get; set; }
            public string game { get; set; }
            public int viewers { get; set; }
            public DateTime created_at { get; set; }
        }        
    }
}
