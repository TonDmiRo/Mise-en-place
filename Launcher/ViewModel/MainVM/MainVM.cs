﻿using Launcher.Model;
using Launcher.Model.BuilderForUser;
using Launcher.ViewModel.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Launcher.ViewModel {
    public sealed partial class MainVM : BaseVM {
        #region User prop
        /// <summary>
        /// Время проведенное в программе
        /// </summary>
        public TimeSpan UsageTimeTotal => _user.TotalUsageTime;
        #endregion

        #region Projects
        /// <summary>
        /// Основная коллекция. 
        /// Элементы коллекции являются vm для Pages.
        /// </summary>
        public UserProjects Projects { get; set; }
        /// <summary>
        /// Элемент основной коллекции. Передает значение в Page.
        /// </summary>
        public Project SelectedProject {
            get => _selectedProject;
            set {
                _selectedProject = value;
                OnPropertyChanged();
                RefreshPageValues(_selectedProject);
                ProjectIsNotNull = value is Project;
            }
        }
        /// <summary>
        /// Регулирует доступ к странице
        /// </summary>
        public bool ProjectIsNotNull {
            get => _projectIsNotNull;
            private set {
                _projectIsNotNull = value;
                OnPropertyChanged();
            }
        }

        //TODO: удалить 
        public int ProjectsCount => Projects.Count;
        #endregion

        #region Commands
        private ICommand _saveUserCommand;
        public ICommand SaveUserCommand => _saveUserCommand ?? ( _saveUserCommand = new RelayCommand(SaveUser) );
        private void SaveUser(object parameter) {
            _user.SaveUser();
        }


        // Команды для проекта:
        private ICommand _openPprojectEditorCommand;
        public ICommand OpenPprojectEditorCommand => _openPprojectEditorCommand ?? ( _openPprojectEditorCommand = new RelayCommand(OpenPprojectEditor) );
        private void OpenPprojectEditor(object parameter) {
            /// Открыть новое окно для редактирования 
            /// передаем коллекцию проектов
            /// в этом окне как в примере с авто редактируем не изменяемые значения

        }

        private ICommand _addProjectCommand;
        public ICommand AddProjectCommand => _addProjectCommand ?? ( _addProjectCommand = new RelayCommand(AddProject) );
        private void AddProject(object parameter) {
            string projectName = $"{ProjectsCount + 1}. NewProject";
            Projects.Add(new Project(projectName));
        }

        private ICommand _deleteProjectCommand;
        public ICommand DeleteProjectCommand => _deleteProjectCommand ?? ( _deleteProjectCommand = new RelayCommand(DeleteProject, (object parameter) => { return ProjectIsNotNull; }) );
        private void DeleteProject(object parameter) {
            string message = $"Вы хотите удалить {SelectedProject.ProjectName}?";
            MessageBoxResult result = MessageBox.Show(message, "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes) {
                Projects.Remove(SelectedProject);
            }
        }
        #endregion

        /// <summary>
        /// Этот конструктор необходим для DesignInstance. Используется только для редактирования
        /// </summary>
        public MainVM() {
            Administrator admin;
            NewUserBuilder newUserBuilder = new NewUserBuilder("");
            admin = new Administrator(newUserBuilder);
            admin.Construct();
            _user = newUserBuilder.GetUser();
            //получение пустого пользователя

            _user.SetHandlers(PropUserChanged, NotifyCollectionChanged);
            Projects = _user.Projects;
            InitializePages();
        }
        public MainVM(User user) {
            _user = user; // model
            _user.SetHandlers(PropUserChanged, NotifyCollectionChanged);
            Projects = _user.Projects;
            InitializePages();

        }

        #region Model handlers
        private void PropUserChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == "UsageTimeTotal") { OnPropertyChanged(nameof(UsageTimeTotal)); }
        }
        private void NotifyCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Remove) {
                OnPropertyChanged(nameof(ProjectsCount));
            }
        }
        #endregion
        // TODO: перенести
        private void CheckExistenceOfMaterials(ReadOnlyObservableCollection<Material> materials) {
            StringBuilder damagedMaterials = new StringBuilder();
            foreach (var item in materials) {
                if (item.Exists != true) {
                    item.BlockMaterial();
                    damagedMaterials.AppendLine(item.MaterialTitle);
                }
            }
            if (damagedMaterials.Length > 0) {
                MessageBox.Show("Список поврежденных материалов:\n" + damagedMaterials);
            }
        }

        #region private
        private readonly User _user;
        private Project _selectedProject;
        private bool _projectIsNotNull;
        #endregion
    }
}