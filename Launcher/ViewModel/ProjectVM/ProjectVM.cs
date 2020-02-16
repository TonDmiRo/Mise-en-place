using Launcher.Model;
using Launcher.View;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace Launcher.ViewModel {

    public class ProjectVM : BaseVM {
        private event EventHandler<ProjectEventArgs> ClickTheButton;
        public ProjectVM(EventHandler<ProjectEventArgs> Receiver) {
            _emptyСourse = new Project("Nonexistent project", "Choose a project");
            CurrentProject = _emptyСourse;
            ClickTheButton += Receiver;
        }
        public Project CurrentProject {
            /// Все данные для этой VM берутся из проекта.
            get => _project;
            set {
                /// При изменении проекта необходимо оповещать о незавершенном изменении
                if (ProjectIsCurrentlyChanging) { throw new InvalidOperationException($"Завершите изменение проекта: {CurrentProject.Name}!"); }

                if (value != null && value != _emptyСourse) {
                    _project = value;
                    ProjectIsNotEmpty = true;
                }
                else {
                    _project = _emptyСourse;
                    ProjectIsNotEmpty = false;
                }

                OnPropertyChanged("");
            }
        }

        ///используется для тригера 
        public bool ProjectIsCurrentlyChanging {
            get => _projectIsCurrentlyChanging;
            set {
                _projectIsCurrentlyChanging = value;
                NewName = ( value == true ) ? Name : string.Empty;
            }
        }

        /// необходим для полной блокировки страницы
        public bool ProjectIsNotEmpty { get; set; }

        #region public
        public string Name { get => CurrentProject.Name; }
        public string NewName {
            get => _newName;
            set {
                _newName = value;
                OnPropertyChanged();
            }
        }
        public string Goal {
            get => CurrentProject.Goal;
            set => CurrentProject.Goal = value;
        }


        public string TimeSpentOnProject {
            get {
                double hours = CurrentProject.TimeSpentOnProject.TotalHours;
                var formatted = string.Format("{0}h ", hours);
                return formatted;
            }
        }
        public string NextLesson {
            get {
                TimeSpan timeBeforeLesson = ( CurrentProject.NextLesson() - DateTime.Now );
                bool timeHasCome = timeBeforeLesson < TimeSpan.Zero;
                return timeHasCome ? "NOW" : CurrentProject.NextLesson().ToString("dd/MM/yyyy");
            }
        }

        public ReadOnlyObservableCollection<Material> ProjectMaterials => CurrentProject.ProjectMaterials;
        public ObservableCollection<Task> ProjectTasks => CurrentProject.ProjectTasks;
        #endregion

        #region Commands
        private RelayCommand<object> launcherTheProject;
        public RelayCommand<object> LauncherTheProject {
            get {
                if (launcherTheProject == null) {
                    launcherTheProject = new RelayCommand<object>(execute, canExecute);
                    void execute(object obj) {
                        ClickTheButton(this, new ProjectEventArgs(CommandProject.Start, CurrentProject));
                        OpenMaterials();
                    }
                    void OpenMaterials() {
                        bool oneWasOpen = false;
                        oneWasOpen = _project.OpenMarkedMaterials();
                        foreach (var item in _project.ProjectMaterials) {
                            if (item.Exists != true) {
                                MessageBox.Show($"Путь к материалу:{item.MaterialTitle} поврежден!");
                            }


                            if (!oneWasOpen) { MessageBox.Show("Материалы не выбраны!"); }
                        }
                    }

                    bool canExecute(object obj) => _project != null;

                    return launcherTheProject;
                }
                return launcherTheProject;

            }

        }

        




        #region project editing 
        private RelayCommand<object> changeProject;
        public RelayCommand<object> ChangeProject {
            get {
                if (changeProject == null) {
                    changeProject = new RelayCommand<object>(execute);
                    void execute(object obj) {
                        ClickTheButton(this, new ProjectEventArgs(CommandProject.Change, CurrentProject));
                    }
                    return changeProject;
                }
                return changeProject;

            }

        }


        private RelayCommand<object> renameProject;
        public RelayCommand<object> RenameProject {
            get {
                if (renameProject == null) {
                    renameProject = new RelayCommand<object>(execute, canExecute);
                    void execute(object obj) {
                        MessageBoxResult result = MessageBox.Show($"Время изучения проекта обнулится. Изменить имя {CurrentProject.Name} на {NewName}?.", "Rename", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes) {

                            ClickTheButton(this, new ProjectEventArgs(CommandProject.Rename, CurrentProject));
                            OnPropertyChanged("Name");
                        }
                    }
                    bool canExecute(object obj) {
                        if (NewName == Name) {
                            return false;
                        }
                        bool NameNotIsNull = ( !string.IsNullOrWhiteSpace(NewName) ) && ( !string.IsNullOrWhiteSpace(NewName) );

                        return NameNotIsNull;
                    }


                    return renameProject;
                }
                return renameProject;

            }

        }


        private RelayCommand<object> removeProject;
        public RelayCommand<object> RemoveProject {
            get {
                if (removeProject == null) {
                    removeProject = new RelayCommand<object>(execute);
                    void execute(object obj) {
                        MessageBoxResult result = MessageBox.Show($"Вы хотите удалить {CurrentProject.Name}?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes) {
                            ProjectIsCurrentlyChanging = false;
                            ClickTheButton(this, new ProjectEventArgs(CommandProject.Delete, CurrentProject));
                            ClickTheButton(this, new ProjectEventArgs(CommandProject.Change, CurrentProject));
                        }
                    }
                    return removeProject;
                }
                return removeProject;

            }

        }
        #endregion

        #region для элементов проекта
        private RelayCommand<object> addMaterial;
        public RelayCommand<object> AddMaterial {
            get {
                if (addMaterial == null) {
                    addMaterial = new RelayCommand<object>(execute);
                    void execute(object obj) {
                        AddM();//получить элемент или что такое 
                    }

                    void AddM() {
                        MaterialVM viewModel = new MaterialVM();
                        using (NewMaterialV window = new NewMaterialV(viewModel)) {
                            var result = window.ShowDialog();
                            if (result == true) {

                                CurrentProject.Add(viewModel.GetMaterial());
                            }

                        }
                    }
                    return addMaterial;
                }
                return addMaterial;

            }

        }


        private RelayCommand<Material> fixMaterial;
        public RelayCommand<Material> FixMaterial {
            get {
                if (fixMaterial == null) {
                    fixMaterial = new RelayCommand<Material>(execute);
                    void execute(Material spoiledM) {
                        FixM(spoiledM);
                    }

                    void FixM(Material material) {
                        MaterialVM viewModel = new MaterialVM(material.MaterialTitle);
                        using (FixMaterialPathV window = new FixMaterialPathV(viewModel)) {
                            var result = window.ShowDialog();
                            if (result == true) {
                                Material serviceableMaterial = viewModel.GetMaterial();
                                int indexSpoiledM = CurrentProject.ProjectMaterials.IndexOf(material);
                                CurrentProject.SetMaterial(indexSpoiledM, serviceableMaterial);
                            }

                        }
                    }
                    return fixMaterial;
                }
                return fixMaterial;

            }

        }


        private RelayCommand<Material> removeProjectMaterial;
        public RelayCommand<Material> RemoveProjectMaterial {
            get {
                if (removeProjectMaterial == null) {
                    removeProjectMaterial = new RelayCommand<Material>(execute);
                    void execute(Material deleteM) {
                        CurrentProject.Remove(deleteM);
                    }
                    return removeProjectMaterial;
                }
                return removeProjectMaterial;
            }
        }


        private RelayCommand<object> addTask;
        public RelayCommand<object> AddTask {
            get {
                if (addTask == null) {
                    addTask = new RelayCommand<object>(execute);
                    void execute(object obj) {
                        AddProjectTask();//получить элемент или что такое 
                    }

                    void AddProjectTask() {
                        ProjectTasks.Add(new Task(string.Empty));
                    }
                    return addTask;
                }
                return addTask;

            }

        }
        #endregion

        #endregion

        #region private
        private Project _project;
        private string _newName;
        private bool _projectIsCurrentlyChanging;
        private readonly Project _emptyСourse;
        #endregion
    }

    public enum CommandProject {
        Start,
        Change,
        Rename,
        Delete
    }
    public class ProjectEventArgs : EventArgs {
        public ProjectEventArgs(CommandProject command, Project project) {
            Command = command;
            Project = project;
        }

        public CommandProject Command { get; private set; }
        public Project Project { get; private set; }
    }
}
