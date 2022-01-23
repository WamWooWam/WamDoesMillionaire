using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Threading;

namespace Millionaire
{
    public class PhoneAFriendViewModel : ViewModelBase
    {
        private int index = 0;
        private DispatcherTimer timer;

        public PhoneAFriendViewModel()
        {
            Reset();
        }

        public int State { get; set; }
        public int Remaining { get; set; }
        public bool[] Seconds { get; set; }

        public event EventHandler OnCompleted;

        public void Start()
        {
            State = 2;
            timer = new DispatcherTimer(TimeSpan.FromSeconds(1), DispatcherPriority.Normal, OnTimerTick, Dispatcher.CurrentDispatcher);
            InvokePropertyChanged(nameof(Seconds));
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            Remaining--;
            Seconds[index] = false;
            index++;

            InvokePropertyChanged("");

            if (index >= 30)
            {
                Stop(true);
            }
        }

        public void Stop(bool isImplicit = false)
        {
            State = 3;
            this.timer.Stop();
            if (!isImplicit)
            {
                AudioManager.StopSong();
                AudioManager.PlayCue("Phone_a_Friend_End");
            }

            OnCompleted?.Invoke(this, EventArgs.Empty);
        }

        internal void Reset()
        {
            index = 0;
            Remaining = 30;
            Seconds = new bool[30];
            for (int i = 0; i < 30; i++)
                Seconds[i] = true;
        }
    }
}
