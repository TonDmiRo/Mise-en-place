using Launcher.Model;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Input;
using WinForms = System.Windows.Forms;

namespace Launcher.ViewModel.ModalWindow {

    public enum MaterialType {
        InvalidType,
        Local,
        ///video:local
        ///book:local
        Web
    }

    internal class MaterialCreationVM : BaseVM {

        private bool windowDialogResult;
        public Material GetNewMaterial() {
            if (windowDialogResult) { return newMaterial; }
            return null;
        }
        private Material newMaterial;


        public MaterialType SelectedType {
            get => _selectedType;
            set {
                _selectedType = value;
                OnPropertyChanged();
            }
        }
        public bool PathIsValid => CheckPath(PathToMaterial);
        private bool CheckPath(string path) {
            if (SelectedType == MaterialType.Local) {
                return LocalMaterialCreator.PathIsValid(path);
            }
            else if (SelectedType == MaterialType.Web) {
                return WebMaterialCreator.PathIsValid(path);
            }

            return false;
        }


        public string MaterialTitle {
            get { return _materialTitle; }
            set {
                _materialTitle = value;
                OnPropertyChanged();
            }
        }
        public string PathToMaterial {
            get => _pathToMaterial;
            set {
                _pathToMaterial = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(PathIsValid));
            }
        }


        #region Commands
        private ICommand _getFilePathCommand;
        public ICommand GetFilePathCommand => _getFilePathCommand ??
            ( _getFilePathCommand = new RelayCommand(ChooseFile) );
        private void ChooseFile(object parameter) {
            fileBrowser = new OpenFileDialog();

            if (fileBrowser.ShowDialog() == true) {
                PathToMaterial = fileBrowser.FileName;
            }
        }


        private ICommand _getFolderCommand;
        public ICommand GetFolderPathCommand => _getFolderCommand ??
            ( _getFolderCommand = new RelayCommand(ChooseFolder) );
        private void ChooseFolder(object parameter) {
            folderBrowser = new WinForms.FolderBrowserDialog();
            WinForms.DialogResult result = folderBrowser.ShowDialog();

            if (result == WinForms.DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowser.SelectedPath)) {
                PathToMaterial = folderBrowser.SelectedPath;
            }
        }


        private ICommand _pastFromClipboardCommand;
        public ICommand PastFromClipboardCommand => _pastFromClipboardCommand ??
            ( _pastFromClipboardCommand = new RelayCommand(PastFromClipboard) );
        private void PastFromClipboard(object parameter) {
            PathToMaterial = Clipboard.GetText();
        }


        private ICommand _completeCreationCommand;
        public ICommand CompleteCreationCommand => _completeCreationCommand ??
            ( _completeCreationCommand = new RelayCommand(CompleteCreation, CanCompleteCreation) );
        private void CompleteCreation(object parameter) {
            try {
                CreateMaterial();

                Window window = (Window)parameter;
                windowDialogResult = true;
                window.DialogResult = windowDialogResult;
                window.Close();
            }
            catch (Exception e) {
                MessageBox.Show("Не удалось создать! "+e.Message);
            }
        }
        private void CreateMaterial() {
            switch (SelectedType) {
                case MaterialType.InvalidType:
                    newMaterial = null;
                    throw new ArgumentNullException("Материал был поврежден при создании.");
                // break;
                case MaterialType.Local:
                    newMaterial = new LocalMaterialCreator().CreateMaterial(MaterialTitle, PathToMaterial);
                    break;
                case MaterialType.Web:
                    newMaterial = new WebMaterialCreator().CreateMaterial(MaterialTitle, PathToMaterial);
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }
        private bool CanCompleteCreation(object parameter) {
            /// Необходимость локальных переменных m_title m_path
            /// условие проверяется в цикле
            /// пользователь может изменить одно из свойств в середине проверки
            string m_title = MaterialTitle;
            string m_path = PathToMaterial;

            bool canCompleteCreation = ( !string.IsNullOrWhiteSpace(m_title) ) && ( CheckPath(m_path) );
            return ( !string.IsNullOrWhiteSpace(m_title) ) && ( CheckPath(m_path) );
        }
        #endregion


        public MaterialCreationVM() {
            MaterialTitle = "NewMaterial";
            PathToMaterial = @"E:\Sources\9 - Библиотека\Справочник\Совершенный код. Стив Макконнелл.pdf";
            SelectedType = MaterialType.Local;
        }
        #region private

        private OpenFileDialog fileBrowser;
        private WinForms.FolderBrowserDialog folderBrowser;

        private string _materialTitle;
        private string _pathToMaterial;
        private MaterialType _selectedType;
        #endregion


        #region MyDispose
        private bool isDisposed = false;
        public override void Dispose() {
            if (isDisposed) { return; }

            _completeCreationCommand = null;
            _getFilePathCommand = null;
            _getFolderCommand = null;
            _pastFromClipboardCommand = null;

            fileBrowser = null;
            if (folderBrowser != null) {
                folderBrowser.Dispose();
                folderBrowser = null;
            }
            isDisposed = true;
            base.Dispose();
        }
        #endregion
    }


}
