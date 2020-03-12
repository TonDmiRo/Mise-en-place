//#define EditMainV
using Launcher.Model;
using Launcher.View;
using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Launcher.ViewModel {
    /*To-Do:
     * TODO: Добавить кнопку копировать проект
     * TODO: // => <summary>
     * TODO: xaml: set uniformity in xaml code
     * TODO: заменить повторяющиеся атрибуты стилями
     * TODO: создать help
     * TODO: property("")
     * TODO: Проверить открытие битых файлов
    */
    public class MainVM : BaseVM {
        /// <summary>
        /// Время проведенное в программе
        /// </summary>
        public TimeSpan UsageTimeTotal => _user.UsageTimeTotal;
        /// <summary>
        /// Позволяет динамически изменять центральную часть программы.
        /// </summary>
        public Page CurrentPage {
            /* Варианты замены:
             * страница проекта
             * настройка пользователя
             * 
             * Улучшения:
             * TODO: Реализовать navigatorPage
             */
            get => _currentPage;
            set {
                _currentPage = value;
                OnPropertyChanged();
            }
        }

#if EditMainV 
        public MainVM() {
            // Конструктор без параметров используется только для редактирования MainV
            // TODO: Убрать комментарий <Window.DataContext>
            Administrator admin;
            NewUserBuilder newUserBuilder = new NewUserBuilder("Test");
            admin = new Administrator(newUserBuilder);
            admin.Construct();
            _user = newUserBuilder.GetUser();

            StartTimer();
            InitializeProjectPage();

            Projects = _user.ProjectCollection.Projects;
        }
#endif
        public MainVM(User user) {
            _user = user; // model

            StartTimer();
            InitializeProjectPage();
            Projects = _user.ProjectCollection.Projects;
        }

        #region Timer
        private DispatcherTimer timer;
        private void StartTimer() {
            timer = new DispatcherTimer {
                //Interval = TimeSpan.FromMinutes(1)
                Interval = TimeSpan.FromSeconds(60)
            };
            timer.Tick += new EventHandler(Tick);
            timer.Start();
        }
        private void Tick(object sender, EventArgs e) {
            ///может это хороший вариант?
            /// можно подписаться на tick у пользователя 

            OnPropertyChanged(nameof(UsageTimeTotal));
        }
        #endregion

        #region Collection
        /// <summary>
        /// Основная коллекция. 
        /// Элементы коллекции являются моделью для ProjectVM.
        /// </summary>
        public ReadOnlyObservableCollection<Project> Projects { get; set; }
        /// <summary>
        /// Элемент основной коллекции. Передает значение ProjectVM.
        /// </summary>
        public Project SelectedProject {
            get => _projectVM.CurrentProject;
            set {
                try {
                    _projectVM.CurrentProject = value;
                }
                catch (Exception e) {
                    MessageBox.Show(e.Message);
                }
            }
        }

        public int ProjectsCount => Projects.Count;
        /// <summary>
        /// Свойство для блокировки ListProjects.
        /// </summary>
        public bool ProjectIsCurrentlyChanging => _projectVM.ProjectIsCurrentlyChanging;


        public ReadOnlyObservableCollection<Material> UsefulMaterialValues => _user.UsefulMaterials.Values;
        public int UsefulMaterialsCount => UsefulMaterialValues.Count;
        #endregion

        #region private
        private readonly User _user;
        private ProjectVM _projectVM;
        private Page _currentPage;

        private void InitializeProjectPage() {
            _projectVM = new ProjectVM(Receiver);
            CurrentPage = new ProjectV(_projectVM);
        }
        private void InitializeUserSettingsPage() {
            // TODO: страница настройки
            /*
             * сохраняем ProjectV 
             * открываем настройки
             * блокируем List
             * сохраняем изменения
             * возвращаем ProjectV
             */
        }
        #endregion

        #region  Receiver
        /*
         * Заменить на паттерн Command
         * Для отдельного класса не хватает только коллекции с которой он будет работать 
         * при создании экземпляра передавать ProjectCollection
         * =new Receiver(ProjectCollection collection);
         */
        private void Receiver(object sender, ProjectEventArgs e) {
            switch (e.Command) {
                case CommandProject.Start:
                    StartProject();
                    break;
                case CommandProject.Rename:
                    RenameProject(sender, e);
                    break;
                case CommandProject.Delete:
                    RemoveProject(e);
                    break;
                case CommandProject.Change:
                    ChangeProject(sender);
                    break;
                default:
                    break;
            }
        }

        #region Methods for receiver
        private void ChangeProject(object sender) {
            OnPropertyChanged(nameof(ProjectIsCurrentlyChanging));
        }
        private void StartProject() {
            //TODO: новая страница с таймером
            ///Должен открываться таймер для этого проекта
            ///и уже там кнопка старта
            MessageBox.Show($"Открытие таймера для проекта: {SelectedProject.Name}.");
        }
        private void RenameProject(object sender, ProjectEventArgs e) {
            if (sender is ProjectVM projectVM) {
                bool s = projectVM == _projectVM;
                e.Project.RenameProject(projectVM.NewName); // == CurrentProject

            }

            ///работает не правильно
            ///команда должна отрабатываться здесь 
            ///для этого необходимо ещё передовать строку

            //MessageBox.Show($"Переименование проекта: {e.Project.Name} на новое имя выполнено.");
        }
        private void RemoveProject(ProjectEventArgs e) {
            _user.ProjectCollection.RemoveProject(e.Project);
            OnPropertyChanged(nameof(ProjectsCount));
            //MessageBox.Show($"{e.Project.Name} удален.");
        }
        #endregion

        #endregion

        #region Commands
        private ICommand _saveUserCommand;
        public ICommand SaveUserCommand => _saveUserCommand ?? ( _saveUserCommand = new RelayCommand(SaveUser) );
        private void SaveUser(object parameter) {
            _user.SaveUser();
        }

        private ICommand _addProjectCommand;
        public ICommand AddProjectCommand => _addProjectCommand ?? ( _addProjectCommand = new RelayCommand(AddProject) );
        private void AddProject(object parameter) {
            _user.ProjectCollection.AddProject(new Project("NewProject", "Задать цель"));
            OnPropertyChanged(nameof(ProjectsCount));
        }


        #region for UsefulMaterial
        private ICommand _addUsefulMCommand;
        public ICommand AddUsefulMCommand => _addUsefulMCommand ?? ( _addUsefulMCommand = new RelayCommand(AddUsefulMaterial) );
        private void AddUsefulMaterial(object parameter) {
            using (MaterialVM viewModel = new MaterialVM()) {
                using (NewMaterialV window = new NewMaterialV(viewModel)) {
                    var result = window.ShowDialog();
                    if (result == true) {
                        try {
                            _user.UsefulMaterials.Add(viewModel.GetMaterial());
                        }
                        catch (Exception e) {
                            MessageBox.Show(e.Message);
                        }
                    }
                }
            }

            OnPropertyChanged(nameof(UsefulMaterialsCount));
        }

        private ICommand _startUsefulMCommand;
        public ICommand StartUsefulMCommand => _startUsefulMCommand ?? ( _startUsefulMCommand = new RelayCommand(StartUsefulMaterial) );
        private void StartUsefulMaterial(object startM) {
            try {
                if (startM is Material usefulMaterial)
                    _user.UsefulMaterials.OpenUsefulMaterial(usefulMaterial.MaterialTitle);
            }
            catch (Exception e) {
                MessageBox.Show(e.Message);
            }
        }

        private ICommand _launchUMaterialsCommand;
        public ICommand LaunchUMaterialsCommand => _launchUMaterialsCommand ?? ( _launchUMaterialsCommand = new RelayCommand(LaunchUsefulMaterials, (object obj) => UsefulMaterialsCount > 0) );
        private void LaunchUsefulMaterials(object parameter) {
            VerifyExistence();
            bool oneWasOpen = _user.UsefulMaterials.OpenMarkedUsefulMaterials();
            if (!oneWasOpen) { MessageBox.Show("Материалы не выбраны!"); }
        }
        private void VerifyExistence() {
            StringBuilder damagedMaterials = new StringBuilder();
            foreach (var item in _user.UsefulMaterials.Values) {
                if (item.Exists != true) {
                    item.BlockMaterial();
                    damagedMaterials.AppendLine(item.MaterialTitle);
                }
            }
            if (damagedMaterials.Length > 0) {
                MessageBox.Show("Список поврежденных материалов:\n" + damagedMaterials);
            }
        }

        private ICommand _removeUsefulMCommand;
        public ICommand RemoveUsefulMCommand => _removeUsefulMCommand ?? ( _removeUsefulMCommand = new RelayCommand(RemoveUsefulMaterial) );
        private void RemoveUsefulMaterial(object material) {
            if (material is Material materialToRemove) {
                _user.UsefulMaterials.Remove(materialToRemove.MaterialTitle);
                OnPropertyChanged(nameof(UsefulMaterialsCount));
            }
        }
        #endregion
        #endregion
    }
}
