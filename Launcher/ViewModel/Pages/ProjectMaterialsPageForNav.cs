using Launcher.Model;
using System;

namespace Launcher.ViewModel.Pages {
    internal class ProjectMaterialsPageForNav : BasePageVM {
        private string _title;

        public string NewMaterialTitle {
            get => _title;
            set {
                _title = value;
                OnPropertyChanged();
            }
        }

        public ProjectMaterialsPageForNav() {
            Project = Project.EmptyProject;
            Project.ProjectTasks.Add(new Task("Первая задача"));

            Project.Materials.Add(new LocalMaterialCreator().CreateMaterial("Material1", "\\help.txt"));
            Project.Materials.Add(new LocalMaterialCreator().CreateMaterial("Material2", "\\help.txt"));
            Project.Materials.Add(new LocalMaterialCreator().CreateMaterial("Material3", "\\help.txt"));
        }


        public ProjectMaterialsPageForNav(Action<object> onGoPage, Func<object, bool> canGoPage) : base(onGoPage, canGoPage) { }
    }
}
