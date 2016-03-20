using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchBot.Akinator.Api
{
    public class ListResponse
    {
        public Parameters parameters { get; set; }

        public class Parameters
        {            
            public StepInformation step_information { get; set; }
            public class StepInformation
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
            }

            public Element[] elements { get; set; }
            public class Element
            {
                public string id { get; set; }
                public string name { get; set; }
                public decimal proba { get; set; }
                public string picture_path { get; set; }
            }
        }
    }
}
