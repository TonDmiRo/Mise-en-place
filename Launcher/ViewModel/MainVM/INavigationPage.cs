using System.ComponentModel;
using System.Windows.Input;

namespace Launcher.ViewModel {
    public interface INavigationPage: INotifyPropertyChanged {

        /// <summary>Команда - Вперёд</summary>
        ICommand GoCommand { get; }
    }
}