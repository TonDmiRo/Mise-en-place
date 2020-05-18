using Launcher.Model;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Launcher.ViewModel.Pages {
    internal class ProjectTasksPageVM : ProjectBasePageVM {
        public ICollection<Task> ProjectTasks {
            get {
                _projectTasks = Project?.ProjectTasks ?? null;
                return _projectTasks;
            }
        }

        #region Commands
        private ICommand _addTaskCommand;


        public ICommand AddTaskCommand => _addTaskCommand ?? ( _addTaskCommand = new RelayCommand(AddTask, CanExecuteMethod) );
        private void AddTask(object parameter) { ProjectTasks.Add(new Task(string.Empty)); }
        private bool CanExecuteMethod(object parameter) => ProjectTasks != null;

        #endregion


        /// <summary>Безпараметрический конструктор</summary>
        public ProjectTasksPageVM() {
            Project = Project.EmptyProject;
            ProjectTasks.Add(new Task("Первая задача"));
        }
        /// <summary>Конструктор</summary>
        /// <param name="onGoPage">Модель</param>
        /// <param name="onGoPage">Метод для перехода на страницу</param>
        /// <param name="canGoPage">Метод проверяющий возможность перехода на страниц</param>
        public ProjectTasksPageVM(Action<object> onGoPage, Func<object, bool> canGoPage) : base(onGoPage, canGoPage) { }


        #region private
        private ICollection<Task> _projectTasks;
        #endregion
    }
}
