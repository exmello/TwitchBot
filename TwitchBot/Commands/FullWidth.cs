using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TwitchBot.Data;
using TwitchBot.Model;
using TwitchBot.TwitchApi;

namespace TwitchBot.Commands
{
    /// <summary>
    /// Responds to !full with a unicode conversion
    /// </summary>
    public class FullWidth : ICommand
    {
        private readonly Regex regFullWidth;
        private readonly TwitchResponseWriter tw;

        //"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ"
        private char[] map = { '\xfeff','\xff41','\xff42','\xff43','\xff44','\xff45','\xff46','\xff47','\xff48','\xff49','\xff4a','\xff4b','\xff4c','\xff4d','\xff4e','\xff4f','\xff50','\xff51','\xff52','\xff53','\xff54','\xff55','\xff56','\xff57','\xff58',
'\xff59','\xff5a','\xff21','\xff22','\xff23','\xff24','\xff25','\xff26','\xff27','\xff28','\xff29','\xff2a','\xff2b','\xff2c','\xff2d','\xff2e','\xff2f','\xff30','\xff31','\xff32','\xff33','\xff34','\xff35','\xff36','\xff37','\xff38','\xff39','\xff3a' };

        public FullWidth(TwitchResponseWriter tw)
        {
            this.tw = tw;
            this.regFullWidth = new Regex("^!full\\s(?<template>.+?)\\s$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        }

        public bool IsMatch(MessageInfo message)
        {
            return regFullWidth.IsMatch(message.Content);
        }

        void ICommand.Process(MessageInfo message)
        {
            //get template
            char[] template = null;
            Match match = regFullWidth.Match(message.Content);
            if (match.Success)
            {
                if (!string.IsNullOrWhiteSpace(match.Groups["template"].Value))
                {
                    byte[] fromBytes = Encoding.UTF8.GetBytes(match.Groups["template"].Value);
                    byte[] toBytes = UnicodeEncoding.Convert(Encoding.UTF8, Encoding.Unicode, fromBytes);

                    template = UnicodeEncoding.Unicode.GetChars(toBytes);

                    for (int i = 0; i < template.Length; i++)
                    {
                        if (template[i] >= 'a' && template[i] <= 'z')
                            template[i] = map[(int)template[i] - 97 + 1];
                        else if (template[i] >= 'A' && template[i] <= 'Z')
                            template[i] = map[(int)template[i] - 65 + 26 + 1];
                    }
                }
            }

            if (template != null)
            {
                string response = new string(template);

                tw.RespondMessage(response);
            }
        }

    }
}
