﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TwitchBot.Akinator.Api;

namespace TwitchBot.Akinator
{
    public class Character
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Probability { get; set; }
        public string Photo { get; set; }

        public Character(ListResponse.Parameters.Element ele)
        {
            Id = ele.element.id;
            Name = ele.element.name;
            Probability = ele.element.proba;
            Photo = ele.element.absolute_picture_path;
        }
    }
}
