using System;
using System.Timers;

namespace Launcher {
    internal class MyTimer {
        
        private readonly Timer _timer;
        public MyTimer(double interval, ElapsedEventHandler elapsed, bool autoReset = true) {
            _timer = new Timer(interval) {

                Enabled = false,
                AutoReset = autoReset,
            };
            _timer.Elapsed += elapsed;//Подписка только одного события 
            Start();
        }

        public void Start() {
            _timer.Start();
        }

        public void Stop() {
            _timer.Stop();
        }
    }
}
