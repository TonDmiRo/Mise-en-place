using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Launcher.ViewModel {
    public abstract class  BaseVM : INotifyPropertyChanged, IDisposable {
        protected BaseVM() {

            }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName="") {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if ( handler != null ) {
                handler.Invoke(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        public void Dispose() {
            this.OnDispose();
            }

        protected virtual void OnDispose() {
            throw new NotImplementedException();
            }
        }
    
    }


