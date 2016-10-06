using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OverallTest
{
    class MultipleTask : IDisposable
    {
        private static readonly Random Rand = new Random();

        public bool IsRunning { get; private set; }

        private readonly double MinSec, MaxSec;
        private readonly Action Run;

        private CancellationTokenSource CTS;

        public MultipleTask(double minSec, double maxSec, Action run)
        {
            MinSec = minSec;
            MaxSec = maxSec;
            Run = run;
        }

        public MultipleTask(double sec, Action run)
            : this(sec, sec, run) { }

        public void Start()
        {
            if (!IsRunning)
            {
                IsRunning = true;
                StartWaiting();
            }
        }

        public void Stop()
        {
            CTS.Cancel();
            IsRunning = false;
        }

        private void Call()
        {
            Run.Invoke();
            StartWaiting();
        }

        private void StartWaiting()
        {
            var sec = MinSec + (MaxSec - MinSec) * Rand.NextDouble();
            var ms = (int)(sec * 1000);

            CTS = new CancellationTokenSource();
            Task.Delay(ms, CTS.Token).ContinueWith((x) => Call(), 
                TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
