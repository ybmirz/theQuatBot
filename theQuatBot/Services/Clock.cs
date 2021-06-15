using System;
using System.Timers;

namespace TheQuatBot.Services
{
    public class Clock
    {
        private readonly Timer _timer = new Timer(1000);
        private DateTime _yesturday = DateTime.Today;

        public event EventHandler NewDay;

        public Clock()
        {
            _timer.Elapsed += Timer_Elapsed;
            _timer.Start();
            Console.WriteLine("Timer per second check is alive!");
        }

        void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _timer.Stop();
            if (_yesturday != DateTime.Today)
            {
                if (NewDay != null) NewDay(this, EventArgs.Empty);
                _yesturday = DateTime.Today;
            }            
            _timer.Start();
        }

        public void Dispose()
        {
            _timer.Stop();
            _timer.Dispose();
        }

    }
}
