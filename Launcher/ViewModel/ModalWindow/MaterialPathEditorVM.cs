using Launcher.Model;
using Launcher.Model.SpecificMaterials;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Input;
using WinForms = System.Windows.Forms;

namespace Launcher.ViewModel.ModalWindow {
    public class MaterialPathEditorVM : BaseVM {
        public Material SelectedMaterial { get; set; }
        public string NewPathToMaterial {
            get => _newPathToMaterial;
            set {
                _newPathToMaterial = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(PathIsValid));
            }
        }

        public bool PathIsValid => CheckPath(NewPathToMaterial);
        private bool CheckPath(string path) {
            if (SelectedMaterial is LocalMaterial) {
                return LocalMaterialCreator.PathIsValid(path);
            }

            if (SelectedMaterial is WebMaterial) {
                return WebMaterialCreator.PathIsValid(path);
            }
            return false;
        }

        #region Commands
        private ICommand _getFilePathCommand;
        public ICommand GetFilePathCommand => _getFilePathCommand ??
            ( _getFilePathCommand = new RelayCommand(ChooseFile) );
        private void ChooseFile(object parameter) {
            fileBrowser = new OpenFileDialog();

            if (fileBrowser.ShowDialog() == true) {
                NewPathToMaterial = fileBrowser.FileName;
            }
        }


        private ICommand _getFolderCommand;
        public ICommand GetFolderPathCommand => _getFolderCommand ??
            ( _getFolderCommand = new RelayCommand(ChooseFolder) );
        private void ChooseFolder(object parameter) {
            folderBrowser = new WinForms.FolderBrowserDialog();
            WinForms.DialogResult result = folderBrowser.ShowDialog();

            if (result == WinForms.DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowser.SelectedPath)) {
                NewPathToMaterial = folderBrowser.SelectedPath;
            }
        }


        private ICommand _pastFromClipboardCommand;
        public ICommand PastFromClipboardCommand => _pastFromClipboardCommand ??
            ( _pastFromClipboardCommand = new RelayCommand(PastFromClipboard) );
        private void PastFromClipboard(object parameter) {
            NewPathToMaterial = Clipboard.GetText();
        }


        private ICommand _applyChangeCommand;
        public ICommand ApplyChangeCommand => _applyChangeCommand ??
            ( _applyChangeCommand = new RelayCommand(ApplyChange, CanApplyChange) );
        private void ApplyChange(object parameter) {
            if (SelectedMaterial.ChangePathToMaterial(NewPathToMaterial)) {
                MessageBox.Show("Путь изменен!");
                Window window = (Window)parameter;
                window.DialogResult = true;
                window.Close();
            }
            else {
                MessageBox.Show("Путь не может быть изменен.");
            }
        }
        private bool CanApplyChange(object parameter) {
            bool canApplyChange = ( SelectedMaterial.MaterialTitle != NewPathToMaterial ) && ( PathIsValid );
            return canApplyChange;
        }
        #endregion

        #region private
        private string _newPathToMaterial;

        private OpenFileDialog fileBrowser;
        private WinForms.FolderBrowserDialog folderBrowser;

        public string WindowTitle => "Изменение пути к материалу " + SelectedMaterial?.MaterialTitle ?? "...";
        public MaterialPathEditorVM() {
            SelectedMaterial = new LocalMaterialCreator().CreateMaterial("Test", System.IO.Directory.GetCurrentDirectory());
        }
        #endregion

        #region MyDispose
        private bool isDisposed = false;
        public override void Dispose() {
            if (isDisposed) { return; }

            _applyChangeCommand = null;
            _getFilePathCommand = null;
            _getFolderCommand = null;
            _pastFromClipboardCommand = null;

            fileBrowser = null;
            if (folderBrowser != null) {
                folderBrowser.Dispose();
                folderBrowser = null;
            }
            SelectedMaterial = null;
            isDisposed = true;
            base.Dispose();
        }
        #endregion
    }
}
