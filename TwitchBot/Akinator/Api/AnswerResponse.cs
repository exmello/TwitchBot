using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchBot.Akinator.Api
{
    public class AnswerResponse
    {
        public string completion { get; set; }

        public Parameters parameters { get; set; }

        public class Parameters
        {
            public string question { get; set; }
            public Answer[] answers { get; set; }
            
            public class Answer
            {
                public string answer { get; set; }
            }

            public string step { get; set; }
            public string progression { get; set; }
            public string questionid { get; set; }
            public string infogain { get; set; }
            public string status_minibase { get; set; }
        }
    }
}
