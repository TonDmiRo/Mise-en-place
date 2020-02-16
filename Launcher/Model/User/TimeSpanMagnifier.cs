using System;
using System.Timers;

namespace Launcher.Model {
    /// <summary>
    /// Пока
    /// Увеличивает временной интервал каждый раз когда истекает интервал времени таймера
    /// 
    /// Возможно:
    /// Настраивать запуск и остановку таймера
    /// Настраивать увелечения и уменьшение значения только это будет не Magnifier
    /// </summary>
    internal class TimeSpanMagnifier {
        private const double millisecondsForTimer = 1000.0;
        public TimeSpanMagnifier(TimeSpan startInterval, EventHandler<TimeSpanHasChangedEventArgs> handler) {
            //timespan увеличиваемый каждые millisecondsForTimer на millisecondsForTimer
            increasedTimeSpan = startInterval;

            //связь с моделью
            TimeSpanHasChanged = handler;

            _timer = new Timer(millisecondsForTimer) {
                Enabled = true,
                AutoReset = true,
            };
            _timer.Elapsed += TimeHasElapsed;
        }

        private TimeSpan increasedTimeSpan;
        private readonly Timer _timer;

        #region Methods for timer
        private void TimeHasElapsed(object source, ElapsedEventArgs e) {
            //через каждые millisecondsForTimer вызывается это событие
            increasedTimeSpan += TimeSpan.FromMilliseconds(millisecondsForTimer * 60);
            OnTimeSpanHasChanged(new TimeSpanHasChangedEventArgs(increasedTimeSpan));
        }
        private void Start() {
            _timer.Start();
        }
        private void Stop() {
            _timer.Stop();
        }
        #endregion

        #region уведомитель о изменение timespan
        private event EventHandler<TimeSpanHasChangedEventArgs> TimeSpanHasChanged;
        protected virtual void OnTimeSpanHasChanged(TimeSpanHasChangedEventArgs e) {
            EventHandler<TimeSpanHasChangedEventArgs> handler;
            lock (this) {
                handler = TimeSpanHasChanged;
            }

            handler?.Invoke(this, e); // if(handler != null) handler(this,e);

            /// почему студиа против? 
            /// Публикация событий, соответствующих рекомендациям .NET Framework изменилась?
            /// https://docs.microsoft.com/ru-ru/dotnet/csharp/programming-guide/events/how-to-publish-events-that-conform-to-net-framework-guidelines
        }
        #endregion
    }
    public class TimeSpanHasChangedEventArgs : EventArgs {

        public TimeSpanHasChangedEventArgs(TimeSpan t) {
            IncreasedTS = t;
        }
        public TimeSpan IncreasedTS { get; private set; }
    }
}

