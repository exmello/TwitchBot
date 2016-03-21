using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchBot.Akinator.Api;

namespace TwitchBot.Akinator
{
    public class QuestionData
    {
        public KeyValuePair<string, string> Question { get; set; }
        public IList<string> Answers { get; set; }
        public decimal Progression { get; set; }

        public QuestionData(NewSessionResponse response)
        {
            Question = new KeyValuePair<string, string>(
                response.parameters.step_information.questionid,
                response.parameters.step_information.question);
            Answers = response.parameters.step_information.answers
                .Select(ans => ans.answer)
                .ToList();
            Progression = response.parameters.step_information.progression;
        }

        public QuestionData(AnswerResponse response)
        {
            Question = new KeyValuePair<string, string>(
                response.parameters.questionid,
                response.parameters.question);
            Answers = response.parameters.answers
                .Select(ans => ans.answer)
                .ToList();
            Progression = response.parameters.progression;
        }
    }
}
