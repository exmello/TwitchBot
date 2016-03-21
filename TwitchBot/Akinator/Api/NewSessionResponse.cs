using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchBot.Akinator.Api
{
    public class NewSessionResponse
    {
        public Parameters parameters { get; set; }

        public class Parameters
        {
            public Indentification identification { get; set; }
            public class Indentification
            {
                public int channel { get; set; }
                public string session { get; set; }
                public string signature { get; set; }
            }
            
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
                public decimal progression { get; set; }
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
