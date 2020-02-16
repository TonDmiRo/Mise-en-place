using Launcher.Model;
using Launcher.Model.BuilderForUser;
using Launcher.View;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Launcher.ViewModel {
    public class MainVM : BaseVM {
        private string _username;
        public MainVM() {
            URLCheckVM viewModel = new URLCheckVM();
            using (URLCheckV window = new URLCheckV(viewModel)) {
                var result = window.ShowDialog();
                if (result == true) {
                    _username = viewModel.URL;
                }
            }

            Administrator admin;
            try {
                JsonUserBuilder builder = new JsonUserBuilder(_username);
                admin = new Administrator(builder);
                admin.Construct();
                _user = builder.GetUser();
            }
            catch (FileNotFoundException e) {
                MessageBox.Show(e.Message + " Поместите json файлы в папку Users");

                NewUserBuilder newBuilder = new NewUserBuilder(_username);
                admin = new Administrator(newBuilder);
                admin.Construct();
                _user = newBuilder.GetUser();
            }
            catch (Exception e) {
                MessageBox.Show(e.Message);
            }


            StartTimer();

            _projectVM = new ProjectVM(Receiver);
            CurrentPage = new ProjectV(_projectVM);
            Projects = _user.ProjectCollection.Projects;
        }


        #region  Receiver
        /// Для отдельного класса не хватает только коллекции с которой он будет работать 
        /// 
        /// при создании экземпляра должен получить ProjectCollection
        /// 
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

        private void ChangeProject(object sender) {
            OnPropertyChanged("ProjectIsCurrentlyChanging");
        }

        //Методы для команд
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
            OnPropertyChanged("ProjectsCount");
            //MessageBox.Show($"{e.Project.Name} удален.");
        }
        #endregion

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
            //может это хороший вариант?

            OnPropertyChanged("UsageTimeTotal");
        }
        #endregion

        #region CheckFile
        private void CheckFiles(string username) {
            FilesCheckVM viewModel = new FilesCheckVM(username);
            using (FilesCheckV window = new FilesCheckV(viewModel)) {
                var result = window.ShowDialog();
                if (result == true) {

                }
            }
        }
        #endregion

        #region public prop
        //Время для таймера
        public TimeSpan UsageTimeTotal => _user.UsageTimeTotal;
        //коллекции
        public ReadOnlyObservableCollection<Project> Projects { get; set; }
        public int ProjectsCount => Projects.Count;
        //Блокировка для List
        public bool ProjectIsCurrentlyChanging => _projectVM.ProjectIsCurrentlyChanging;
        public ReadOnlyObservableCollection<Material> UsefulMaterialValues => _user.UsefulMaterials.Values;

        public int UsefulMaterialsCount => UsefulMaterialValues.Count;

        //Первая из страниц взаимодействующая с проектом
        public Page CurrentPage {
            /// <summary>
            /// нужно для смены центральной части 
            /// пока существует только страница проекта
            /// можно добавить
            /// настройку программы
            /// редактирование проекта
            /// создание проекта
            /// 
            /// везде нужна кнопка назад для неё нужен navigatorPage
            /// </summary>
            get => _currentPage;
            set {
                _currentPage = value;
                OnPropertyChanged("CurrentPage");
            }
        }
        public Project SelectedProject {
            get => _projectVM.CurrentProject;
            set {

                try {
                    _projectVM.CurrentProject = value;
                }

                ///насколько оправдан _selectedProject? может его полностью заменить?
                ///останавливает то что это свойство привязано к SelectedItem 

                catch (Exception e) {

                    MessageBox.Show(e.Message);

                    /// TODO: Не знаю как решить
                    /// Проблема: визуальная часть не соответствует программной 
                    /// я блокирую изменение выбранного элемента, но ListBox меняет визуальную часть
                    /// проверил SelectedValue = null; не работает
                    /// провери SelectedIndex = -1; не работает 
                    /// 
                    /// isEnabled ListBox прировнять к ProjectNameIsCurrentlyChanging


                }
            }
        }
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
                        OnPropertyChanged("ProjectsCount");
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
                        OnPropertyChanged("UsefulMaterialsCount");
                    }
                    void AddUsefulM() {
                        MaterialVM viewModel = new MaterialVM();
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
                        OnPropertyChanged("UsefulMaterialsCount");
                    }
                    return removeUsefulMaterial;
                }
                return removeUsefulMaterial;
            }
        }
        #endregion

        #endregion

        #region private
        private readonly User _user;
        private ProjectVM _projectVM;
        private Page _currentPage;
        #endregion
    }
}
