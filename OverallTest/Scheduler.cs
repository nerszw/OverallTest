using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OverallTest
{
    class Scheduler : IDisposable
    {
        private List<MultipleTask> Tasks;

        public Scheduler(List<MultipleTask> tasks = null)
        {
            Tasks = tasks ?? new List<MultipleTask>();
        }
        
        public bool IsRunning { get; private set; }

        public void Start()
        {
            if (!IsRunning)
            {
                IsRunning = true;
                Tasks.ForEach(x => x.Start());
            }
        }

        public void Stop()
        {
            if (IsRunning)
            {
                Tasks.ForEach(x => x.Stop());
                IsRunning = false;
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
