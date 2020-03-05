﻿using System;
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

        /// <summary>
        /// Значение по умолчанию 60000.0
        /// </summary>
        public double MillisecondsForTimer {
            get => _millisecondsForTimer;
            set {
                if (( 100 < value ) && ( value < System.Int32.MaxValue )) {
                    _millisecondsForTimer = value;
                }
                else {
                    throw new ArgumentOutOfRangeException(
                   $"{nameof(value)} must be between 100 and Int32.MaxValue.");
                }
            }
        }
        /// <summary>
        /// Обновленный TimeSpan для пользователя.
        /// </summary>
        private TimeSpan IncreasedTimeSpan {
            get => _increasedTimeSpan;
            set {
                _increasedTimeSpan = value;
                OnTimeSpanHasChanged(new TimeSpanHasChangedEventArgs(IncreasedTimeSpan));
            }
        }

        /// <param name="handler">Метод для обновления TimeSpan.</param>
        public TimeSpanMagnifier(TimeSpan startInterval, EventHandler<TimeSpanHasChangedEventArgs> handler) {
            IncreasedTimeSpan = startInterval;
            TimeSpanHasChanged = handler;
            InitializeTimer();
        }
        /// <summary>
        /// TimeSpanMagnifier оповещает об изменении TimeSpan через это событие. Передает обновленный TimeSpan.
        /// </summary>
        private event EventHandler<TimeSpanHasChangedEventArgs> TimeSpanHasChanged;
        /// <summary>
        /// Произошло изменение свойства IncreasedTimeSpan равное +60* сек.
        /// * - интервал обновления можно изменить. 
        /// </summary>
        /// <param name="e">Интервал времени увеличенный на 60* сек. </param>
        protected virtual void OnTimeSpanHasChanged(TimeSpanHasChangedEventArgs e) {
            EventHandler<TimeSpanHasChangedEventArgs> handler;
            lock (this) {
                handler = TimeSpanHasChanged;
            }
            handler?.Invoke(this, e); // if(handler != null) handler(this,e);
        }


        private void InitializeTimer() {
            MillisecondsForTimer = 60000.0; // = 60 сек.
            _timer = new Timer(MillisecondsForTimer) {
                Enabled = true,
                AutoReset = true,
            };
            _timer.Elapsed += TimeHasElapsed;
        }
        /// <summary>
        /// Обрабатывает событие таймера через интервал времени = MillisecondsForTimer.
        /// </summary>
        private void TimeHasElapsed(object source, ElapsedEventArgs e) {
            IncreasedTimeSpan += TimeSpan.FromMilliseconds(MillisecondsForTimer);
        }
        private TimeSpan _increasedTimeSpan;

        private Timer _timer;
        private double _millisecondsForTimer;

        

        //TODO: удалить
        // остановить таймер ~ закрыть программу.
        private void Start() {
            _timer.Start();
        }
        private void Stop() {
            _timer.Stop();
        }
    }

    /// <summary>
    /// Вспомогательный класс для передачи интервала.
    /// </summary>
    public class TimeSpanHasChangedEventArgs : EventArgs {
        public TimeSpanHasChangedEventArgs(TimeSpan t) {
            IncreasedTimeSpan = t;
        }
        /// <summary>
        /// Интервал времени увеличенный на 60* сек.
        /// * - интервал обновления можно изменить. 
        /// </summary>
        public TimeSpan IncreasedTimeSpan { get; private set; }
    }
}

