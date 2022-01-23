using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using System.Diagnostics;
using System.Windows.Threading;
using System.Linq;

namespace Millionaire
{
    public class AskTheAudienceViewModel : ViewModelBase
    {
        private const double MaxHeight = 230.0d;

        private TwitchClient client;
        private SynchronizationContext syncContext;
        private ConcurrentDictionary<string, char> guessDictionary;
        private DispatcherTimer timer;

        private int totalGuesses;
        private int[] guesses;

        private int actualTotalGuesses;
        private int[] actualGuesses;

        private DateTimeOffset? startTime;

        public int State { get; set; } = 0;
        public string AGuesses => totalGuesses == 0 ? "0%" : $"{((decimal)guesses[3] / (decimal)totalGuesses) * 100:N0}%";
        public string BGuesses => totalGuesses == 0 ? "0%" : $"{((decimal)guesses[2] / (decimal)totalGuesses) * 100:N0}%";
        public string CGuesses => totalGuesses == 0 ? "0%" : $"{((decimal)guesses[1] / (decimal)totalGuesses) * 100:N0}%";
        public string DGuesses => totalGuesses == 0 ? "0%" : $"{((decimal)guesses[0] / (decimal)totalGuesses) * 100:N0}%";
        public double AHeight => totalGuesses == 0 ? 0.0d : ((double)guesses[3] / (double)totalGuesses) * MaxHeight;
        public double BHeight => totalGuesses == 0 ? 0.0d : ((double)guesses[2] / (double)totalGuesses) * MaxHeight;
        public double CHeight => totalGuesses == 0 ? 0.0d : ((double)guesses[1] / (double)totalGuesses) * MaxHeight;
        public double DHeight => totalGuesses == 0 ? 0.0d : ((double)guesses[0] / (double)totalGuesses) * MaxHeight;

        public AskTheAudienceViewModel(TwitchClient client)
        {
            this.client = client;
            this.guesses = new int[4];
            this.actualGuesses = new int[4];

            this.syncContext = SynchronizationContext.Current;
            this.guessDictionary = new ConcurrentDictionary<string, char>();

            this.client.OnMessageReceived += OnGotMessage;
        }

        public void Reset()
        {
            this.guesses = new int[4];
            this.totalGuesses = 1000;

            this.actualGuesses = new int[4];
            this.actualTotalGuesses = 0;
            this.guessDictionary = new ConcurrentDictionary<string, char>();

            this.InvokePropertyChanged("");
        }

        public void Start(Question question)
        {
            this.Reset();
            this.State = 2;

            this.client.SendMessage("wamwoowam", $"OKAY CHAT '{question.Text}'. ");
            this.client.SendMessage("wamwoowam", $"-A: '{question.Choices[0]}', -B: '{question.Choices[1]}', -C: '{question.Choices[2]}', -D: '{question.Choices[3]}'. ");
            this.client.SendMessage("wamwoowam", $"Type A/B/C/D to cast your vote!");

            this.startTime = DateTimeOffset.Now;
            this.timer = new DispatcherTimer(TimeSpan.FromSeconds(1 / 60.0), DispatcherPriority.Normal, OnTimerTick, Dispatcher.CurrentDispatcher);
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            var delta = DateTimeOffset.Now - startTime.Value;
            if (delta > TimeSpan.FromSeconds(30))
            {
                Stop();
                return;
            }

            var progress = delta.TotalSeconds;
            var delay = 0.33f;
            this.guesses[3] = (int)(1000 * EaseInOutSine(Math.Max(0, progress)));
            this.guesses[2] = (int)(1000 * EaseInOutSine(Math.Max(0, progress - (delay * 1))));
            this.guesses[1] = (int)(1000 * EaseInOutSine(Math.Max(0, progress - (delay * 2))));
            this.guesses[0] = (int)(1000 * EaseInOutSine(Math.Max(0, progress - (delay * 3))));

            this.InvokePropertyChanged("");
        }

        public void Stop()
        {
            this.startTime = (DateTimeOffset.Now - TimeSpan.FromMinutes(1));
            this.guesses = this.actualGuesses;
            this.totalGuesses = this.actualTotalGuesses;
            this.timer.Stop();
            this.State = 3;

            AudioManager.StopSong();
            AudioManager.PlayCue("Ask_the_Audience_Result");
            this.InvokePropertyChanged("");
        }

        private double EaseInOutSine(double t)
        {
            return -(Math.Cos(Math.PI * t) - 1.0) / 2.0;
        }

        private void OnGotMessage(object sender, OnMessageReceivedArgs e)
        {
            if (startTime == null || DateTimeOffset.Now - startTime.Value > TimeSpan.FromSeconds(30))
                return;

            var text = e.ChatMessage.Message.ToLowerInvariant().Trim();
            if (text.Length != 1 || text[0] < 'a' || text[0] > 'd')
                return;

            if (this.guessDictionary.TryGetValue(e.ChatMessage.Username, out char c))
            {
                Interlocked.Decrement(ref actualGuesses[(int)('d' - c)]);
            }
            else
            {
                Interlocked.Increment(ref actualTotalGuesses);
            }

            this.guessDictionary[e.ChatMessage.Username] = text[0];

            int guess = (int)('d' - text[0]);
            Interlocked.Increment(ref actualGuesses[guess]);
        }
    }
}
