using Launcher.Model;
using System;
using System.Windows.Input;

namespace Launcher.ViewModel.Pages {
    internal class ProjectMaterialsPageForNav : BasePageVM {
        public string NewMaterialTitle {
            get => _title;
            set {
                _title = value;
                // OnPropertyChanged();
            }
        }

        public Material SelectedMaterial {
            get { return _selectedMaterial; }
            set {
                _selectedMaterial = value;
                NewMaterialTitle = ( value != null ) ? value.MaterialTitle : string.Empty;
                // OnPropertyChanged();

            }
        }

        #region Commands
        private ICommand _openMaterialCommand;
        public ICommand OpenMaterialCommand => _openMaterialCommand ?? ( _openMaterialCommand = new RelayCommand(OpenMaterial) );
        private void OpenMaterial(object parameter) {
            if (parameter is Material material) {
                material.OpenMaterial();
            }
        }

        private ICommand _removeSelectedMaterialCommand;
        public ICommand RemoveSelectedMaterialCommand => _removeSelectedMaterialCommand ?? ( _removeSelectedMaterialCommand = new RelayCommand(RemoveSelectedMaterial) );
        private void RemoveSelectedMaterial(object parameter) {
            Project.Materials.Remove(SelectedMaterial);
        }
        #endregion

        public ProjectMaterialsPageForNav() {
            Project = Project.EmptyProject;
            Project.ProjectTasks.Add(new Task("Первая задача"));

            Project.Materials.Add(new LocalMaterialCreator().CreateMaterial("Material1", "\\help.txt"));
            Project.Materials.Add(new LocalMaterialCreator().CreateMaterial("Material2", "\\help.txt"));
            Project.Materials.Add(new LocalMaterialCreator().CreateMaterial("Material3", "\\help.txt"));
        }

        public ProjectMaterialsPageForNav(Action<object> onGoPage, Func<object, bool> canGoPage) : base(onGoPage, canGoPage) { }

        #region private
        private string _title;
        private Material _selectedMaterial;
        #endregion
    }
}
