using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Net.WebSockets;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Linq;
using System.Numerics;
using TwitchLib.Client;
using TwitchLib.Client.Models;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Events;
using System.Diagnostics;

namespace Millionaire
{
    public class QuestionsFile
    {
        [JsonPropertyName("allQuestions")]
        public Question[] AllQuestions { get; set; }
    }

    public class Question
    {
        [JsonPropertyName("question")]
        public string Text { get; set; }

        [JsonPropertyName("choices")]
        public string[] Choices { get; set; }

        [JsonPropertyName("answer")]
        public string Answer { get; set; }
    }

    public class ControllerViewModel : ViewModelBase
    {
        public class StateViewModel
        {
            public int StateIndex { get; set; }
            public string StateName { get; set; }
            public ICommand GoToState { get; set; }
        }

        private SynchronizationContext context;
        private HttpListener httpListener;
        private ConcurrentBag<WebSocket> activeConnections;

        private TwitchClient twitchClient;
        private bool isInPlay;
        private GameViewModel gameViewModel;
        private string questionSource;

        public Random Random { get; set; }
        public bool IsInPlay { get => isInPlay; set => OnPropertySet(ref isInPlay, value); }

        public int State { get; set; }

        public Brush HttpListenerStatus => httpListener.IsListening ? Brushes.ForestGreen : Brushes.Orange;
        public Brush WebSocketStatus => activeConnections.Count == 0 ? Brushes.Orange : Brushes.ForestGreen;
        public Brush TwitchStatus => twitchClient?.IsConnected ?? false ? Brushes.ForestGreen : Brushes.Orange;
        public string HttpListenerStatusText => httpListener.IsListening ? "Listening on port 12345" : "Not listening";
        public string WebSocketStatusText => activeConnections.Count == 0 ? "No connections" : $"{activeConnections.Count} connections";
        public string TwitchStatusText => twitchClient?.IsConnected ?? false ?
            $"Conntected to {twitchClient.JoinedChannels.Count} channel." :
            "Connecting to Twitch";

        //public Question[] Questions { get; set; }

        public string QuestionSource { get => questionSource; set => OnPropertySet(ref questionSource, value); }
        public ObservableCollection<string> QuestionSources { get; set; }

        public ObservableCollection<StateViewModel> States { get; set; }
        public GameViewModel GameViewModel { get => gameViewModel; set => OnPropertySet(ref gameViewModel, value); }
        public float Duration { get; set; } = 0.75f;
        public bool IsLost { get; internal set; }

        public ControllerViewModel()
        {
            context = SynchronizationContext.Current;
            httpListener = new HttpListener();
            activeConnections = new ConcurrentBag<WebSocket>();
            States = new ObservableCollection<StateViewModel>();
            Random = new Random();
        }

        public ControllerViewModel(ControllerWindow parent) : this()
        {
            var doc = JsonDocument.Parse(File.ReadAllText("assets/fuck.json"));

            twitchClient = new TwitchClient();
            twitchClient.Initialize(new ConnectionCredentials("wambot_", doc.RootElement.GetProperty("accessToken").GetString()));
            twitchClient.OnConnected += OnTwitchConnected;
            twitchClient.OnJoinedChannel += OnJoinedChannel;
            twitchClient.OnLog += OnTwitchLog;

            States.Add(new StateViewModel() { StateIndex = 0, StateName = "Host Only", GoToState = new GoToStateCommand(parent) });
            States.Add(new StateViewModel() { StateIndex = 1, StateName = "Contestent and Host", GoToState = new GoToStateCommand(parent) });
            States.Add(new StateViewModel() { StateIndex = 2, StateName = "Contestant Only", GoToState = new GoToStateCommand(parent) });
            States.Add(new StateViewModel() { StateIndex = 3, StateName = "Contestant Scoreboard", GoToState = new GoToStateCommand(parent) });

            QuestionSources = new ObservableCollection<string>();
            foreach (var item in Directory.EnumerateFiles("Assets", "*.json"))
            {
                using var stream = File.OpenRead(item);
                var source = JsonDocument.Parse(stream);
                if (source.RootElement.ValueKind == JsonValueKind.Array)
                {
                    QuestionSources.Add(Path.GetFileName(item));
                }
            }

            QuestionSource = QuestionSources[0];

            NewGame();
        }

        public void NewGame()
        {
            IsInPlay = false;
            IsLost = false;

            var questions = JsonSerializer.Deserialize<Question[]>(File.ReadAllText($"Assets/{QuestionSource}"));
            GameViewModel = new GameViewModel(questions);
            GameViewModel.AskTheAudience = new AskTheAudienceViewModel(twitchClient);
            GameViewModel.PhoneAFriend = new PhoneAFriendViewModel();
        }

        private void OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            Refresh();
        }

        private void OnTwitchLog(object sender, OnLogArgs e)
        {
            Debug.WriteLine(e.Data);
        }

        private void OnTwitchConnected(object sender, OnConnectedArgs e)
        {
            twitchClient.JoinChannel("wamwoowam");
        }

        private void Refresh()
        {
            context.Post((o) => InvokePropertyChanged(""), null);
        }

        public void StartListening()
        {
            twitchClient.Connect();

            httpListener.Prefixes.Add("http://localhost:12345/");
            httpListener.Start();

            Refresh();
            _ = Task.Run(async () => await ListenAsync());
        }

        private async Task ListenAsync()
        {
            HttpListenerContext ctx;
            while ((ctx = await httpListener.GetContextAsync()) != null)
            {
                if (!ctx.Request.IsWebSocketRequest) return;

                var socketContext = await ctx.AcceptWebSocketAsync(null, 1024, TimeSpan.FromSeconds(10));
                activeConnections.Add(socketContext.WebSocket);

                await SendSceneChangeAsync(State);

                Refresh();
            }
        }

        internal async Task SendSceneChangeAsync(int state)
        {
            var oldState = State;
            State = state;
            foreach (var item in activeConnections)
            {
                try
                {
                    if (item.State == WebSocketState.Open)
                    {
                        var array = new byte[8] { 1, (byte)state, (byte)oldState, 0, 0, 0, 0, 0 };
                        BitConverter.TryWriteBytes(new Span<byte>(array, 4, 4), Duration);
                        await item.SendAsync(new ArraySegment<byte>(array), WebSocketMessageType.Binary, true, default);
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        internal async Task SendConfettiAsync()
        {
            foreach (var item in activeConnections)
            {
                try
                {
                    if (item.State == WebSocketState.Open)
                    {
                        var array = new byte[4] { 2, 0, 0, 0 };
                        await item.SendAsync(new ArraySegment<byte>(array), WebSocketMessageType.Binary, true, default);
                    }
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
