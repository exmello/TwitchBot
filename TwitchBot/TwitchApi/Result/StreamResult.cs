﻿using System;
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
            public DateTime created_at { get; set; }
        }        
    }
}
