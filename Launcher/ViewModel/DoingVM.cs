using System;
using System.Windows;
using System.Windows.Input;

namespace Launcher.ViewModel {
    public class DoingVM : BaseVM {

        public DoingVM() {
            magnifierForElapsedTime = new TimeSpanMagnifier(TimeSpan.Zero, ChangedUsageTimeTotal) {
                MillisecondsForTimer = 1000
            };
        }

        private void ChangedUsageTimeTotal(object sender, TimeSpanHasChangedEventArgs e) {
            ElapsedTime = e.IncreasedTimeSpan;
        }

        public TimeSpan ElapsedTime {
            get => elapsedTime;
            private set {
                elapsedTime = value;
                OnPropertyChanged();
                _counterToCheck++;

                if (_counterToCheck > 60) {
                    _counterToCheck = 0;
                    CheckTime();
                }
            }
        }
        private void CheckTime() {
            if (ElapsedTime.TotalMinutes < 60) {
                if (ElapsedTime.Minutes < 50) {
                    if (ElapsedTime.Minutes > 25) {
                        TwentyFiveMinutesPassed = true;
                    }
                }
                else { FiftyMinutesPassed = true; }
            }
            else { TimeToRelax = true; }
        }

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

        #region Commands
        private ICommand _skipTaskCommand;
        public ICommand SkipTaskCommand => _skipTaskCommand ?? ( _skipTaskCommand = new RelayCommand(CloseDoingV) );

        private ICommand _completeTaskCommand;
        public ICommand CompleteTaskCommand => _completeTaskCommand ?? ( _completeTaskCommand = new RelayCommand(CloseDoingV, CanDone) );
        private void CloseDoingV(object parameter) {
            Window window = (Window)parameter;
            window.Close();
        }
        private bool CanDone(object parameter) {
            return TwentyFiveMinutesPassed || FiftyMinutesPassed || TimeToRelax;
        }
        #endregion
        #region private
        private TimeSpan elapsedTime;
        private TimeSpanMagnifier magnifierForElapsedTime;

        private bool twentyFiveMinutesPassed;
        private bool fiftyMinutesPassed;
        private bool timeToRelax;
        private int _counterToCheck;
        #endregion

        public override void Dispose() {
            base.Dispose();
            magnifierForElapsedTime.Dispose();
            magnifierForElapsedTime = null;
        }
    }
}
