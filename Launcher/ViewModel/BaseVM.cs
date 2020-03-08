using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Launcher.ViewModel {
    public abstract class BaseVM : INotifyPropertyChanged, IDisposable {
        public event PropertyChangedEventHandler PropertyChanged;
        private bool isDisposed = false;
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = "") {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null) {
                handler.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public virtual void Dispose() {
            if (isDisposed)
                return;
            PropertyChanged = null;
            isDisposed = true;
        }
    }
}


