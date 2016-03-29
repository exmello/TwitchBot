﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchBot.Dictionary;
using TwitchBot.Model;

namespace TwitchBot.Data
{
    public interface IQuoteRepository
    {
        void AddUpdate(Quote quote);
        Quote GetRandom();
        Quote GetRandomByName(string name);
    }
}