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
        private CancellationTokenSource cancel;

        //constants
        private const decimal THRESHOLD = 97M;
        private const int NUM_SECONDS_TO_WAIT = 20;
        private const int MAX_QUESTIONS = 79;
        private const int MIN_STEPS_BETWEEN_PROPOSAL = 5;
        private const int MAX_BEFORE_FIRST_PROPOSAL = 20;

        //state
        private string _session = null;
        private string _signature = null;
        private int _step = 0;
        private int _stepOfLastProposal = 0;
        private string _userThatStartedGame = null;
        private DateTime _startedListening = DateTime.MinValue;
        private Dictionary<int, int> _answerCount = new Dictionary<int, int>();
        private Dictionary<string, bool> _mods = new Dictionary<string, bool>();
        private QuestionData _currentQuestion = null;

        public State State { get; set; }

        public AkinatorBot(TwitchResponseWriter tw, TwitchApiClient api)
        {
            this.tw = tw;
            this.twitchApi = api;
            this.akinatorApi = new AkinatorApiClient();
            this.cancel = new CancellationTokenSource();
            
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
                        if(UserIsMod(message))
                        {
                            tw.RespondMessage(string.Format("20 Questions is currently {0}.  Commands: enable, disable, start, stop", GetStateString()));
                        }
                        else
                        {
                            tw.RespondMessage(string.Format("20 Questions is currently {0}.  Commands: start, stop", GetStateString()));
                        }
                    }
                }
                else
                {
                    //look for numeric answers
                    int answerId;
                    if(State == State.ListeningForAnswers 
                        && message.Content.Length < 5 
                        && int.TryParse(message.Content, out answerId))
                    {
                        AddAnswer(answerId - 1);
                    }
                }
            }
        }

        private bool UserIsMod(MessageInfo message)
        {
            //TODO: cache mod data from X minutes. remember mods per channel if bot in multiple channels!

            if(_mods.ContainsKey(message.Username))
            {
                return _mods[message.Username];
            }

            var chatters = twitchApi.Chatters(message.Channel);

            if(chatters == null)
            {
                tw.RespondMessage("Failed getting moderator data. Try again shortly.");
                return false;
            }

            if (chatters.chatters.moderators.Contains(message.Username)
                || chatters.chatters.staff.Contains(message.Username)
                || chatters.chatters.admins.Contains(message.Username)
                || chatters.chatters.global_mods.Contains(message.Username))
            {
                //cache: Is a mod
                _mods.Add(message.Username, true);
                return true;
            }
            else
            {
                //cache: Is not a mod
                _mods.Add(message.Username, false);
                return false;
            }
        }

        private void ProcessCommand(Command command, MessageInfo message)
        {
            switch (command)
            {
                case Command.Stop:
                    if (message.Username == _userThatStartedGame || UserIsMod(message))
                    {
                        State = State.Stopped;
                        Stop();
                        tw.RespondMessage("20 Questions has been stopped.");
                    }
                    break;
                case Command.Start:
                    Start(message);
                    break;
                case Command.Enable:
                    if(State == State.Disabled && UserIsMod(message))
                    {
                        State = State.Stopped;
                        tw.RespondMessage("20 Questions has been enabled.  Think of any character (real or imaginary) and type \"!20q start\" to begin a new game.");
                    }
                    break;
                case Command.Disable:
                    if (State != State.Disabled && UserIsMod(message))
                    { 
                        State = State.Disabled;
                        Stop();
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
                    tw.RespondMessage("20 Questions has been disabled.");
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

        private void Stop()
        {
            //clear state
            _step = 0;
            _stepOfLastProposal = 0;
            _answerCount.Clear();
            _startedListening = DateTime.MinValue;
            _userThatStartedGame = null;
            _session = null;
            _signature = null;
            _currentQuestion = null;

            //cancel
            cancel.Cancel();
        }

        private void Hello(MessageInfo message)
        {
            _step = 0;

            var response = akinatorApi.NewSession(message.Username);

            this._session = response.parameters.identification.session;
            this._signature = response.parameters.identification.signature;
            this._userThatStartedGame = message.Username;

            QuestionData question = new QuestionData(response);

            tw.RespondMessage(string.Format("20 Questions has started! You will have {0} seconds to answer and the top answer will be submitted.", NUM_SECONDS_TO_WAIT));

            OnAsk(question);
        }
        
        private void SendAnswer(int answerID)
        {
            if (State != State.ListeningForAnswers)
                return;

            State = State.Started;

            var response = akinatorApi.Answer(this._session, this._signature, this._step, answerID);

            QuestionData question = new QuestionData(response);

            //clear the last round of answers
            _answerCount.Clear();
            _step++;

            if(_step >= MAX_QUESTIONS)
            {
                //always present answers at cap
                GetCharacters();
            }
            else if(_step - _stepOfLastProposal < MIN_STEPS_BETWEEN_PROPOSAL)
            {
                //don't present answers too often
                OnAsk(question);
            }
            else if(question.Progression > THRESHOLD || _step - _stepOfLastProposal == MAX_BEFORE_FIRST_PROPOSAL)
            {
                //if we meet the threshold or it or we're due to make a guess anyway
                if(_step + 5 >= MAX_QUESTIONS)
                {
                    //if we're near the end keep asking
                    OnAsk(question);
                }
                else
                {
                    //present the answer!
                    GetCharacters();
                }
            }
            else
            {
                OnAsk(question);
            }
        }

        private void OnAsk(QuestionData question)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0}. {1} Answer 1-{2}: ", _step + 1, question.Question.Value, question.Answers.Count);
            for (int i = 0; i < question.Answers.Count; i++)
            {
                sb.AppendFormat("{0}. {1}", i + 1, question.Answers[i]);
                if (i < question.Answers.Count - 1)
                    sb.Append(", ");
            }
            //show question
            tw.RespondMessage(sb.ToString());
            
            _currentQuestion = question;
            
            //start listening for answers
            State = State.ListeningForAnswers;
            _startedListening = DateTime.Now;
            Task.Run(async delegate
              {                 
                 await Task.Delay(TimeSpan.FromSeconds(NUM_SECONDS_TO_WAIT));
                 ListenForAnswers();
              }, cancel.Token);
        }  

        private void GetCharacters()
        {
            _stepOfLastProposal = _step;

            var response = akinatorApi.List(this._session, this._signature, this._step);

            var characters = response.parameters.elements
                .Select(ele => new Character(ele))
                .OrderByDescending(c => c.Probability)
                .ToList();

            OnFound(characters);
            
            _step++;
        }

        private void OnFound(IList<Character> characters)
        {
            StringBuilder sb = new StringBuilder();
            if(characters.Count == 1)
            {
                Character character = characters.First();
                sb.AppendFormat("Is your character {0}? {1} {2}", character.Name, character.Description, character.Photo);
            }
            else
            {
                foreach (Character character in characters)
                {
                    if(character.Probability > THRESHOLD)
                    {
                        sb.AppendFormat("Is your character {0}? ", character);
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
            //if valid answer
            if (answerId < _currentQuestion.Answers.Count)
            {
                if (DateTime.Now > _startedListening.AddSeconds(NUM_SECONDS_TO_WAIT))
                {
                    SendAnswer(answerId);
                } 
                else if (_answerCount.ContainsKey(answerId))
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
