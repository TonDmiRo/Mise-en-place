using Launcher.Model;
using Launcher.ViewModel.Pages;
using System.Collections.Generic;
using System.Windows.Input;

namespace Launcher.ViewModel {
    public enum PageNumEnum {
        /// <summary>Нет страницы</summary>
        None = -1,
        /// <summary>Титульная страница</summary>
        Title = 0,
        /// <summary>Первая страница</summary>
        First = 1,
        /// <summary>Вторая страница</summary>
        Second = 2,
        /// <summary>Третья страница</summary>
        Third = 3
    }

    public sealed partial class MainVM : BaseVM {
        private void InitializePages() {
            dictPages.Add(PageNumEnum.First, new ProjectTasksPageVM(OnGoPage, CanGoPage));
            dictPages.Add(PageNumEnum.Second, new ProjectMaterialsPageVM(OnGoPage, CanGoPage));

            Content = dictPages[PageNumEnum.Second];
        }
        /// <summary>Обновляет модель vm страницы</summary>
        /// <param name="porject">SelectedProject</param>
        private void RefreshPageValues(Project porject) {
            if (Content is ProjectBasePageVM basePageVM) {
                //ProjectBasePageVM vm = (ProjectBasePageVM)Content;
                basePageVM.Project = porject;
            }
        }
      

        /// <summary>Словарь для экземпляров VM страниц</summary>
        private readonly Dictionary<PageNumEnum, INavigationPage> dictPages = new Dictionary<PageNumEnum, INavigationPage>();

        private INavigationPage _content;
        /// <summary>VM для текущей страницы</summary>
        public INavigationPage Content {
            get => _content;
            set {
                _content = value;
                RefreshPageValues(SelectedProject);
                OnPropertyChanged();
            }
        }

        /// <summary>Метод переключающий страницы</summary>
        /// <param name="parameter">Должно быть допустимое значение перечисления</param>
        private void OnGoPage(object parameter) {
            if (CanGoPage(parameter)) {
                Content = dictPages[(PageNumEnum)parameter];
            }
        }
        /// <summary>>Метод проверяющий возможность переключения на страницы</summary>
        /// <param name="parameter">Должно быть допустимое значение перечисления</param>
        /// <returns></returns>
        private bool CanGoPage(object parameter)
            => parameter is PageNumEnum page && dictPages.ContainsKey(page);

        private ICommand _toSwitchPageCommand;
        public ICommand SwitchPageCommand => _toSwitchPageCommand ?? ( _toSwitchPageCommand = new RelayCommand(OnGoPage, CanGoPage) );
    }
}
