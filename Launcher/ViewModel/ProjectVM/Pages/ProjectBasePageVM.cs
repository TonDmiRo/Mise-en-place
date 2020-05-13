using Launcher.Model;
using System;

namespace Launcher.ViewModel.Pages {
    internal class ProjectBasePageVM : BasePageVM {
        public ProjectBasePageVM() {

        }
        public ProjectBasePageVM(Action<object> onGoPage, Func<object, bool> canGoPage) : base(onGoPage, canGoPage) { }

        public Project Project {
            get => _project;
            set {
                _project = value;
                OnPropertyChanged();
                ProjectIsNotNull = value is Project;
            }
        }
        public bool ProjectIsNotNull {
            get => _projectIsNotNull;
            private set {
                _projectIsNotNull = value;
                OnPropertyChanged();
            }
        }


        private Project _project;
        private bool _projectIsNotNull;
    }
}
