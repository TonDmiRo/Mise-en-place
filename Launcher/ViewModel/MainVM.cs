//#define EditMainV
using Launcher.Model;
using Launcher.View;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Launcher.ViewModel {
    /*To-Do:
     * TODO: Добавить кнопку копировать проект
     * TODO: // => <summary>
     * TODO: xaml: set uniformity in xaml code
     * TODO: заменить повторяющиеся атрибуты стилями
     * TODO: создать help
     * TODO: property("")
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
            // Конструктор без параметров используется только для работы с MainV
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
        private RelayCommand<object> saveUser;
        public RelayCommand<object> SaveUser {
            get {
                if (saveUser == null) {
                    saveUser = new RelayCommand<object>(execute);
                    void execute(object obj) {
                        _user.SaveUser();
                    }
                    return saveUser;
                }
                return saveUser;
            }
        }


        private RelayCommand<object> addProject;
        public RelayCommand<object> AddProject {
            get {
                if (addProject == null) {
                    addProject = new RelayCommand<object>(execute);
                    void execute(object obj) {
                        _user.ProjectCollection.AddProject(new Project("NewProject", "Задать цель"));
                        OnPropertyChanged(nameof(ProjectsCount));
                    }
                    return addProject;
                }
                return addProject;

            }

        }


        #region for UsefulMaterial
        private RelayCommand<object> addUsefulMaterial;
        public RelayCommand<object> AddUsefulMaterial {
            get {
                if (addUsefulMaterial == null) {
                    addUsefulMaterial = new RelayCommand<object>(execute);
                    void execute(object obj) {
                        AddUsefulM();
                        OnPropertyChanged(nameof(UsefulMaterialsCount));
                    }
                    void AddUsefulM() {
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
                    }

                    return addUsefulMaterial;
                }
                return addUsefulMaterial;

            }

        }


        private RelayCommand<Material> startUsefulMaterial;
        public RelayCommand<Material> StartUsefulMaterial {
            get {
                if (startUsefulMaterial == null) {
                    startUsefulMaterial = new RelayCommand<Material>(execute);
                    void execute(Material startM) {
                        try {
                            _user.UsefulMaterials.OpenUsefulMaterial(startM.MaterialTitle);
                        }
                        catch (Exception e) {
                            MessageBox.Show(e.Message);
                        }
                    }
                    return startUsefulMaterial;
                }
                return startUsefulMaterial;
            }
        }

        private RelayCommand<object> launcherUsefulMaterials;
        public RelayCommand<object> LauncherUsefulMaterials {
            get {
                if (launcherUsefulMaterials == null) {
                    launcherUsefulMaterials = new RelayCommand<object>(execute, canExecute);
                    void execute(object obj) {

                        OpenUsefulMaterials();
                    }
                    void OpenUsefulMaterials() {
                        bool oneWasOpen = false;
                        oneWasOpen = _user.UsefulMaterials.OpenMarkedUsefulMaterials();
                        //TODO: проверить если несколько ошибок
                        foreach (var item in _user.UsefulMaterials.Values) {
                            if (item.Exists != true) {
                                MessageBox.Show($"Путь к материалу:{item.MaterialTitle} поврежден!");
                            }
                        }




                        if (!oneWasOpen) { MessageBox.Show("Материалы не выбраны!"); }
                    }

                    bool canExecute(object obj) => UsefulMaterialsCount > 0;

                    return launcherUsefulMaterials;
                }
                return launcherUsefulMaterials;

            }

        }


        private RelayCommand<Material> removeUsefulMaterial;
        public RelayCommand<Material> RemoveUsefulMaterial {
            get {
                if (removeUsefulMaterial == null) {
                    removeUsefulMaterial = new RelayCommand<Material>(execute);
                    void execute(Material startM) {
                        _user.UsefulMaterials.Remove(startM.MaterialTitle);
                        OnPropertyChanged(nameof(UsefulMaterialsCount));
                    }
                    return removeUsefulMaterial;
                }
                return removeUsefulMaterial;
            }
        }
        #endregion

        #endregion
    }
}
