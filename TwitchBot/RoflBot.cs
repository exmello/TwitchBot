using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TwitchBot
{
    public class RoflBot
    {
        private readonly TwitchResponseWriter tw;

        private DateTime lastMessageSent;
        private Random random;
        private string[] messages;
        private int arraySize;

        public RoflBot(TwitchResponseWriter tw)
        {
            this.tw = tw;

            // Lets you know its working
            tw.RespondMessage("RoflBot Activating MrDestructoid");
            lastMessageSent = DateTime.Now;
            random = new Random();
            messages = GetMessages();
            arraySize = messages.Length;
        }

        public void SendRandomMessage()
        {
            if (lastMessageSent.AddSeconds(20) < DateTime.Now)
            {
                tw.RespondMessage(messages[random.Next(arraySize)]);
            }
        }

        private string[] GetMessages()
        {
            return new string[]
            {
                "SMOrc Me have no aim SMOrc Me have no brain SMOrc Me proud to be Winston main SMOrc",
                "Kappa",
                " ( ͡° ͜ʖ ͡°)┌∩┐THIS IS A TviQuoKusaii STREAM NOW ( ͡° ͜ʖ ͡°)┌∩┐",
                "Ṭ̷Ř̥̤̤̻̥̥ͧ̏ͦ̋͑͡Ɨ̘͉̲̯̹͔̿ͯͦ͋͂͡Ǥ̸̷͈͇͉̟̫͚͖͉̼̰̱̩͔̙̖̱̌͑ͥ̐ͤͧ̂͌̃ͬ͟͜ͅĠ̟͓͇̺̭̮̇̄̍̃ͬͣ͂ͪ̽̃̀͜Ɇ̛ͦ̄̓ͪ̇̌̄̒̊̓̾̐͒͋ͭ̀͗̚͝҉̧͙͍̦̣̤͇͓͙̲͍̪̤̻͢ͅṜ͓̠̘̥̼̈́̌ͬ͜ͅḚ̬̯͎͉̙̉ͧ͆̕Ƌ̶",
                "What kind of Alienware computer should I get?",
                "☐ Not rekt☑ Rekt☑ Really Rekt☑ Tyrannosaurus Rekt☑ Cash4Rekt.com☑ Grapes of Rekt☑ Ship Rekt☑ Rekt markes the spot☑ Caught rekt handed☑ The Rekt Side Story☑ Singin' In The Rekt☑ Painting The Roses Rekt☑ Rekt Van Winkle☑ Parks and Rekt☑ Lord of the Rekts: The Reking of the King☑ Star Trekt☑ The Rekt Prince of Bel-Air☑ A Game of Rekt☑ Rektflix☑ Rekt it like it's hot☑ RektBox 360☑ The Rekt-men☑ School Of Rekt☑ I am Fire, I am Rekt☑ Rekt and Roll☑ Pro",
                "ヽヽ｀ヽ｀、ヽヽ｀ヽ｀、ヽヽ｀ヽ、ヽヽ｀ヽ｀、ヽヽ｀ヽ｀、｀、ヽヽ｀ヽ｀、ヽヽ｀ヽ｀、ヽヽ｀ヽ｀、ヽヽ｀ヽ｀、ヽヽ｀ヽ｀、ヽヽ｀ヽ｀、ヽヽ༼ຈل͜ຈ༽ﾉ☂ ɪᴛs ʀᴀɪɴɪɴɢ sᴀʟᴛ! ヽ༼ຈل͜ຈ༽ﾉ☂ ヽ｀ヽ｀、ヽヽ｀ヽ｀、｀ヽ｀、ヽヽ｀ヽ｀、ヽヽ｀ヽ、ヽヽ｀ヽ",
                "~~~*~~~~*~~ don't let your memes be dreams ~~~*~~"
            };

        }

    }
}
