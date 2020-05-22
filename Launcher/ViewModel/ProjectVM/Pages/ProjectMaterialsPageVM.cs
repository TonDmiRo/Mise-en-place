using Launcher.Model;
using Launcher.View.ModalWindow;
using Launcher.ViewModel.ModalWindow;
using System;
using System.Windows;
using System.Windows.Input;

namespace Launcher.ViewModel.Pages {
    internal class ProjectMaterialsPageVM : ProjectBasePageVM {
        private ProjectMaterials Materials {
            get {
                _projectMaterials = Project?.Materials ?? null;
                return _projectMaterials;
            }
        }

        public string NewMaterialTitle { get; set; }

        public Material SelectedMaterial {
            get { return _selectedMaterial; }
            set {
                _selectedMaterial = value;
                NewMaterialTitle = value?.MaterialTitle;
            }
        }

        #region Commands
        private ICommand _openMaterialCommand;
        public ICommand OpenMaterialCommand => _openMaterialCommand ??
            ( _openMaterialCommand = new RelayCommand(OpenMaterial) );
        private void OpenMaterial(object parameter) {
            if (parameter is Material material) {
                material.OpenMaterial();
            }
        }


        private ICommand _addMaterialCommand;
        public ICommand AddMaterialCommand => _addMaterialCommand ??
            ( _addMaterialCommand = new RelayCommand(AddMaterial) );
        private void AddMaterial(object parameter) {
            using (MaterialCreationVM vm = new MaterialCreationVM()) {
                using (MaterialCreationV materialCreationV = new MaterialCreationV() { DataContext = vm }) {
                    materialCreationV.ShowDialog();

                    Material newMaterial = vm.GetNewMaterial();
                    if (newMaterial != null) {
                        Materials.Add(newMaterial);
                        MessageBox.Show("Материал успешно создан.");
                    }
                }
            }
        }


        private ICommand _renameSelectedMaterialCommand;
        public ICommand RenameSelectedMaterialCommand => _renameSelectedMaterialCommand ??
            ( _renameSelectedMaterialCommand = new RelayCommand(RenameSelectedMaterial, CanRenameSelectedMaterial) );
        private void RenameSelectedMaterial(object parameter) { SelectedMaterial.MaterialTitle = NewMaterialTitle; }
        private bool CanRenameSelectedMaterial(object parameter) {
            if (SelectedMaterial != null) {
                if (string.IsNullOrWhiteSpace(NewMaterialTitle)) { return false; }
                bool titlesDiffer = !NewMaterialTitle.Equals(SelectedMaterial.MaterialTitle);
                return titlesDiffer;
            }
            return false;
        }


        private ICommand _changePathOfSelectedMaterialCommand;
        public ICommand ChangePathOfSelectedMaterialCommand => _changePathOfSelectedMaterialCommand ??
           ( _changePathOfSelectedMaterialCommand = new RelayCommand(ChangePathOfSelectedMaterial) );
        private void ChangePathOfSelectedMaterial(object parameter) {
            using (MaterialPathEditorVM vm = new MaterialPathEditorVM { SelectedMaterial = SelectedMaterial }) {
                using (MaterialPathEditorV materialPathEditorV = new MaterialPathEditorV() { DataContext = vm }) {
                    materialPathEditorV.ShowDialog();
                }
            }
        }


        private ICommand _removeSelectedMaterialCommand;
        public ICommand RemoveSelectedMaterialCommand => _removeSelectedMaterialCommand ??
            ( _removeSelectedMaterialCommand = new RelayCommand(RemoveSelectedMaterial) );
        private void RemoveSelectedMaterial(object parameter) {
            Materials.Remove(SelectedMaterial);
        }
        #endregion

        public ProjectMaterialsPageVM() {
            Project = Project.EmptyProject;

            Project.Materials.Add(new LocalMaterialCreator().CreateMaterial("Material1", "\\help.txt"));
            Project.Materials.Add(new LocalMaterialCreator().CreateMaterial("Material2", "\\help.txt"));
            Project.Materials.Add(new LocalMaterialCreator().CreateMaterial("Material3", "\\help.txt"));
        }

        public ProjectMaterialsPageVM(Action<object> onGoPage, Func<object, bool> canGoPage) : base(onGoPage, canGoPage) { }

        #region private
        private Material _selectedMaterial;
        private ProjectMaterials _projectMaterials;
        #endregion
    }
}
