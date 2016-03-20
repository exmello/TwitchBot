using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TwitchBot.Akinator.Api;
using TwitchBot.Model;
using TwitchBot.TwitchApi;

namespace TwitchBot.Akinator
{
    public class AkinatorBot : ITwitchBot
    {
        //dependancies
        private readonly TwitchResponseWriter tw;
        private readonly TwitchApiClient twitchApi;
        private readonly AkinatorApiClient akinatorApi;

        //helper
        private Regex regCommand = new Regex("^!20q\\s(?<command>[a-zA-Z_]*?)\\s$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        private TextInfo textInfo = CultureInfo.InvariantCulture.TextInfo;
        private CancellationTokenSource cancelListen;

        //constants
        private const decimal THRESHOLD = 0.9M;
        private const int NUM_SECONDS_TO_WAIT = 20;

        //state
        private string _session = null;
        private string _signature = null;
        private int _step = 0;
        private DateTime _startedListening = DateTime.MinValue;
        private Dictionary<int, int> _answerCount = new Dictionary<int, int>();
        private QuestionData _currentQuestion = null;

        public State State { get; set; }

        public AkinatorBot(TwitchResponseWriter tw, TwitchApiClient api)
        {
            this.tw = tw;
            this.twitchApi = api;
            this.akinatorApi = new AkinatorApiClient();
            this.cancelListen = new CancellationTokenSource();
            
            State = State.Disabled;
        }

        public void ProcessMessage(MessageInfo message)
        {
            if (message.Action == MessageActionType.Message)
            {
                Match match = regCommand.Match(message.Content);
                if (match.Success)
                {
                    if (!string.IsNullOrWhiteSpace(match.Groups["command"].Value))
                    {
                        Command command;
                        if(Enum.TryParse<Command>(textInfo.ToTitleCase(match.Groups["command"].Value), out command))
                        {
                            ProcessCommand(command, message);
                        }
                        else
                        {
                            //ignore invalid command
                        }
                    }
                    else
                    {
                        //no command, show state and command list
                        if(UserIsMod(message.Username))
                        {
                            tw.RespondMessage(string.Format("20 Questions is currently {0}.  Commands: enable, disable, stop, start", GetStateString()));
                        }
                        else
                        {
                            tw.RespondMessage(string.Format("20 Questions is currently {0}.  Commands: stop, start", GetStateString()));
                        }
                    }
                }
                else
                {
                    //look for numeric answers
                    int answerId;
                    if( message.Content.Length < 5 && int.TryParse(message.Content, out answerId))
                    {
                        AddAnswer(answerId - 1);
                    }
                }
            }
        }

        private bool UserIsMod(string p)
        {
            //TODO: get mod info from twitch api.  cache mod data from X minutes

            //var followData = api.FollowTarget(username, channel);

            //if (followData != null)
            //{

            return true;
        }

        private void ProcessCommand(Command command, MessageInfo message)
        {
            switch (command)
            {
                case Command.Stop:
                    State = State.Stopped;
                    cancelListen.Cancel();
                    break;
                case Command.Start:
                    Start(message);
                    break;
                case Command.Enable:
                    if(State == State.Disabled && UserIsMod(message.Username))
                    {
                        State = State.Stopped;
                        tw.RespondMessage("20 Questions has been enabled.  [!20q start] to begin a new game.");
                    }
                    break;
                case Command.Disable:
                    if (State != State.Disabled && UserIsMod(message.Username))
                    { 
                        State = State.Disabled;
                        cancelListen.Cancel();
                        tw.RespondMessage("20 Questions has been disabled.");
                    }
                    break;
                default:
                    break;
            }
        }

        private void Start(MessageInfo message)
        {
            switch (State)
            {
                case State.Disabled:
                    //tw.RespondMessage("20 Questions has been disabled.");
                    break;
                case State.Stopped:
                    State = State.Started;
                    Hello(message);
                    break;
                case State.Started:
                    break;
                case State.ListeningForAnswers:
                    break;
                default:
                    break;
            }
        }   

        private void Hello(MessageInfo message)
        {
            _step = 0;

            var response = akinatorApi.NewSession(message.Username);

            this._session = response.parameters.identification.session;
            this._signature = response.parameters.identification.signature;

            QuestionData question = new QuestionData(response);

            OnAsk(question);
        }
        
        private void SendAnswer(int answerID)
        {
            var response = akinatorApi.Answer(this._session, this._signature, this._step, answerID);

            QuestionData question = new QuestionData(response);

            if(question.Last)
            {
                GetCharacters();
            }
            else
            {
                OnAsk(question);
            }

            _step++;
        }

        private void OnAsk(QuestionData question)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(question.Question.Value);
            sb.Append(string.Format(" Answer 1-{0}: ", question.Answers.Count));
            for (int i = 0; i < question.Answers.Count; i++)
            {
                sb.AppendFormat("{0}. {1}", i + 1, question.Answers[i]);
                if (i < question.Answers.Count - 1)
                    sb.Append(", ");
            }
            tw.RespondMessage(sb.ToString());
            _startedListening = DateTime.Now;
            _currentQuestion = question;

            Task.Run(async delegate
              {
                 await Task.Delay(TimeSpan.FromSeconds(NUM_SECONDS_TO_WAIT ));
                 ListenForAnswers();
              }, cancelListen.Token);
        }  

        private void GetCharacters()
        {
            var response = akinatorApi.List(this._session, this._signature, this._step);

            var characters = response.parameters.elements
                .Select(ele => new Character(ele))
                .OrderBy(c => c.Probability)
                .ToList();

            OnFound(characters);
            
            _step++;
        }

        private void OnFound(IList<Character> characters)
        {
            StringBuilder sb = new StringBuilder();
            if(characters.Count == 1)
            {
                sb.AppendFormat("Is your character {0}?", characters.First());
            }
            else
            {
                foreach (Character character in characters)
                {
                    if(character.Probability > THRESHOLD)
                    {
                        sb.AppendFormat("Is your character {0}?", characters);
                    }
                }
            }
            tw.RespondMessage(sb.ToString());

            //TODO: answer yes/no and continue
            //State = State.ListeningForYesNo;
            State = State.Stopped;
        }

        private string GetStateString()
        {
            switch (State)
            {
                case State.Disabled:
                    return "disabled";
                case State.Stopped:
                    return "stopped";
                case State.Started:
                    return "started";
                case State.ListeningForAnswers:
                    return "listening for answers";
                default:
                    return string.Empty;
            }
        }
        
        public void Update()
        {
            switch (State)
            {
                case State.Disabled:
                    break;
                case State.Stopped:
                    break;
                case State.Started:
                    break;
                case State.ListeningForAnswers:
                    ListenForAnswers();
                    break;
                default:
                    break;
            }
        }

        private void AddAnswer(int answerId)
        {
            if (answerId < _currentQuestion.Answers.Count)
            {
                if (_answerCount.ContainsKey(answerId))
                {
                    _answerCount[answerId]++;
                }
                else
                {
                    _answerCount.Add(answerId, 1);
                }
            }
        }

        private void ListenForAnswers()
        {
            if(_answerCount.Count > 0 && DateTime.Now > _startedListening.AddSeconds(NUM_SECONDS_TO_WAIT))
            {
                //grab top answer
                int topAnswer = _answerCount.ToList().OrderByDescending(ans => ans.Value).First().Key;

                tw.RespondMessage(string.Format("Times up! Answering {0}", _currentQuestion.Answers[topAnswer]));
                SendAnswer(topAnswer);
            }
        }
    }
}
