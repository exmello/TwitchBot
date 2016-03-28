using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchBot.Model
{
    public class Quote
    {
        public int ID { get; set; }
        public string Text { get; set; }
        public string AttributedTo { get; set; }
        public DateTime AttributedDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }
}
