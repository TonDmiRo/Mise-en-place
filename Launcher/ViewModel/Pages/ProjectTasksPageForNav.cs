using Launcher.Model;
using System;

namespace Launcher.ViewModel.Pages {
    internal class ProjectTasksPageForNav : BasePageVM {
        /// <summary>Безпараметрический конструктор</summary>
        public ProjectTasksPageForNav() {
            Project = Project.EmptyProject;
            Project.ProjectTasks.Add(new Task("Первая задача"));
        }
        /// <summary>Конструктор</summary>
        /// <param name="onGoPage">Модель</param>
        /// <param name="onGoPage">Метод для перехода на страницу</param>
        /// <param name="canGoPage">Метод проверяющий возможность перехода на страниц</param>
        public ProjectTasksPageForNav(Action<object> onGoPage, Func<object, bool> canGoPage) : base(onGoPage, canGoPage) {
        }
    }
}
