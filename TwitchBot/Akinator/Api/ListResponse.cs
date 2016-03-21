using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchBot.Akinator.Api
{
    public class ListResponse
    {
        public string completion { get; set; }

        public Parameters parameters { get; set; }

        public class Parameters
        {            
            public Element[] elements { get; set; }
            public class Element
            {
                public ElementInfo element { get; set; }
                public class ElementInfo
                {
                    public string id { get; set; }
                    public string name { get; set; }
                    public decimal proba { get; set; }
                    public string description { get; set; }
                    public string picture_path { get; set; }
                    public string absolute_picture_path { get; set; }
                }
            }
        }

        public int NbObjetsPertinents { get; set; }
    }
}
