using Launcher.Model;
using System;
namespace Launcher.ViewModel {
    public class DoingVM : BaseVM {

        public DoingVM() {
            magnifierForElapsedTime = new TimeSpanMagnifier(TimeSpan.Zero, ChangedUsageTimeTotal);
            magnifierForElapsedTime.MillisecondsForTimer = 1000;
        }

        private void ChangedUsageTimeTotal(object sender, TimeSpanHasChangedEventArgs e) {
            ElapsedTime = e.IncreasedTimeSpan;
        }
        public TimeSpan ElapsedTime {
            get => elapsedTime;
            set {
                elapsedTime = value;
                OnPropertyChanged();
            }
        }

        private TimeSpanMagnifier magnifierForElapsedTime;

        public bool TwentyFiveMinutesPassed {
            get => twentyFiveMinutesPassed;
            set {
                twentyFiveMinutesPassed = value;
                OnPropertyChanged();
            }
        }
        public bool FiftyMinutesPassed {
            get => fiftyMinutesPassed;
            set {
                fiftyMinutesPassed = value;
                OnPropertyChanged();
            }
        }
        public bool TimeToRelax {
            get => timeToRelax;
            set {
                timeToRelax = value;
                OnPropertyChanged();
            }
        }


        #region private
        private TimeSpan elapsedTime;
        private bool twentyFiveMinutesPassed;
        private bool fiftyMinutesPassed;
        private bool timeToRelax;
        #endregion
    }
}
