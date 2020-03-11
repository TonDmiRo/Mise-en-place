using Launcher.Model;
using Launcher.View;
using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Input;

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

                OnPropertyChanged(null);// (null) обновляет все свойства
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
        private ICommand _launchProjectCommand;
        public ICommand LaunchProjectCommand => _launchProjectCommand ?? ( _launchProjectCommand = new RelayCommand(LaunchProject, CanLaunchProject) );
        private void LaunchProject(object parameter) {
            ClickTheButton(this, new ProjectEventArgs(CommandProject.Start, CurrentProject));
            OpenMaterials();
        }
        private void OpenMaterials() {
            VerifyExistence();

            bool oneWasOpen = false;
            oneWasOpen = _project.OpenMarkedMaterials();
            if (!oneWasOpen) { MessageBox.Show("Материалы не выбраны!"); }
        }

        private void VerifyExistence() {
            StringBuilder damagedMaterials = new StringBuilder();
            foreach (var item in _project.ProjectMaterials) {
                if (item.Exists != true) {
                    item.BlockMaterial();
                    damagedMaterials.AppendLine(item.MaterialTitle);
                }
            }
            if (damagedMaterials.Length > 0) {
                MessageBox.Show("Список поврежденных материалов:\n" + damagedMaterials);
            }
        }


        private bool CanLaunchProject(object parameter) {
            return _project != null;
        }


        #region project editing 
        private ICommand _changeProjectCommand;
        public ICommand ChangeProjectCommand => _changeProjectCommand ?? ( _changeProjectCommand = new RelayCommand(ChangeProject) );
        private void ChangeProject(object parameter) {
            ClickTheButton(this, new ProjectEventArgs(CommandProject.Change, CurrentProject));
        }


        private ICommand _renameProjectCommand;
        public ICommand RenameProjectCommand => _renameProjectCommand ?? ( _renameProjectCommand = new RelayCommand(RenameProject, CanRenameProject) );
        private void RenameProject(object parameter) {
            string strForMessage = $"Время изучения проекта обнулится. Изменить имя {CurrentProject.Name} на {NewName}?";

            MessageBoxResult result = MessageBox.Show(strForMessage, "Rename", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes) {
                ClickTheButton(this, new ProjectEventArgs(CommandProject.Rename, CurrentProject));
                OnPropertyChanged(nameof(Name));
            }
        }
        private bool CanRenameProject(object parameter) {
            if (NewName == Name) { return false; }
            bool NameNotIsNull = ( !string.IsNullOrWhiteSpace(NewName) ) && ( !string.IsNullOrWhiteSpace(NewName) );
            return NameNotIsNull;
        }


        private ICommand _deleteProjectCommand;
        public ICommand DeleteProjectCommand => _deleteProjectCommand ?? ( _deleteProjectCommand = new RelayCommand(DeleteProject) );
        private void DeleteProject(object parameter) {
            string message = $"Вы хотите удалить {CurrentProject.Name}?";
            MessageBoxResult result = MessageBox.Show(message, "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes) {
                ProjectIsCurrentlyChanging = false;
                ClickTheButton(this, new ProjectEventArgs(CommandProject.Delete, CurrentProject));
                ClickTheButton(this, new ProjectEventArgs(CommandProject.Change, CurrentProject));
            }
        }
        #endregion

        #region для элементов проекта
        private ICommand _addMaterialCommand;
        public ICommand AddMaterialCommand => _addMaterialCommand ?? ( _addMaterialCommand = new RelayCommand(AddMaterial) );
        private void AddMaterial(object parameter) {
            using (MaterialVM viewModel = new MaterialVM()) {
                using (NewMaterialV window = new NewMaterialV(viewModel)) {
                    var result = window.ShowDialog();
                    if (result == true) {
                        CurrentProject.Add(viewModel.GetMaterial());
                    }
                }
            }
        }


        private ICommand _fixMaterialCommand;
        public ICommand FixMaterialCommand => _fixMaterialCommand ?? ( _fixMaterialCommand = new RelayCommand(FixMaterial) );
        private void FixMaterial(object parameter) {
            if (parameter is Material spoiledM) {
                try {
                    Material serviceableMaterial = GetServiceableMaterial(spoiledM.MaterialTitle);
                    int indexSpoiledM = CurrentProject.ProjectMaterials.IndexOf(spoiledM);
                    CurrentProject.SetMaterial(indexSpoiledM, serviceableMaterial);
                }
                catch (Exception e) {
                    MessageBox.Show(e.Message);
                }
            }
        }
        private Material GetServiceableMaterial(string materialTitle) {
            using (MaterialVM viewModel = new MaterialVM(materialTitle)) {
                using (FixMaterialPathV window = new FixMaterialPathV(viewModel)) {
                    var result = window.ShowDialog();
                    if (result == true) {
                        return viewModel.GetMaterial();
                    }
                }
            }
            throw new Exception("Материал не удалось исправить!");
        }


        private ICommand _removeProjectMCommand;
        public ICommand RemoveProjectMCommand => _removeProjectMCommand ?? ( _removeProjectMCommand = new RelayCommand(RemoveProjectMaterial) );
        private void RemoveProjectMaterial(object material) {
            if (material is Material deleteM) {
                CurrentProject.Remove(deleteM);
            }
        }


        private ICommand _addTaskCommand;
        public ICommand AddTaskCommand => _addTaskCommand ?? ( _addTaskCommand = new RelayCommand(AddTask) );
        private void AddTask(object parameter) {
            ProjectTasks.Add(new Task(string.Empty));
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
