using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TwitchBotAdmin.Models
{
    public class QuoteEditViewModel
    {
        [Required]
        public string Text { get; set; }
        [Required]
        public string AttributedTo { get; set; }
        [Required]
        public DateTime AttributedDate { get; set; }
    }
}