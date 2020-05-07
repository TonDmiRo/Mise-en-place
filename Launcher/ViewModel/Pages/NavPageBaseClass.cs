using Launcher.Model;
using System;
using System.Windows.Input;

namespace Launcher.ViewModel.Pages {
    public abstract class BasePageVM : BaseVM, INavigationPage {
        private Project _project;

        public ICommand GoCommand { get; }
        public Project Project {
            get => _project;
            set {
                _project = value;
                OnPropertyChanged();
            }
        }
        /// <summary>Безпараметрический конструктор</summary>
        public BasePageVM() { }


        /// <summary>Конструктор</summary>
        /// <param name="onGoPage">Метод для перехода на страницу</param>
        /// <param name="canGoPage">Метод проверяющий возможность перехода на страниц</param>
        public BasePageVM(Action<object> onGoPage, Func<object, bool> canGoPage) =>
             GoCommand = new RelayCommand((param) => onGoPage(param), (param) => canGoPage(param));
    }
}
