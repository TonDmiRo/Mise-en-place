using Launcher.Model;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using WinForms = System.Windows.Forms;

namespace Launcher.ViewModel {

    public enum MaterialType {
        InvalidType,
        Local,
        ///video:local
        ///book:local
        Web
    }
    public class MaterialVM : BaseVM {


        public Material GetMaterial() {
            Material material = CreateMaterial();
            return material;
        }
        private Material CreateMaterial() {
            Material material = null;
            switch (MaterialType) {
                case MaterialType.InvalidType:
                    material = null;
                    throw new ArgumentNullException("Вы создали пустой материал!");
                // break;
                case MaterialType.Local:
                    material = new LocalMaterialCreator().CreateMaterial(MaterialTitle, PathToMaterial);
                    break;
                case MaterialType.Web:
                    material = new WebMaterialCreator().CreateMaterial(MaterialTitle, PathToMaterial);
                    break;
                default:
                    throw new InvalidOperationException();
            }
            return material;
        }

        #region prop
        public bool ReadOnlyNewMaterial { get; private set; }
        public string MaterialTitle {
            get => _title;
            set {
                if (!ReadOnlyNewMaterial) {
                    _title = value;
                    OnPropertyChanged();
                }
            }
        }
        public string PathToMaterial {
            get => _path;
            set {
                if (!ReadOnlyNewMaterial) {
                    _path = value;
                    OnPropertyChanged();
                }
            }
        }
        public string PathEnteredByUser {
            get => _pathEnteredByUser;
            set {
                _pathEnteredByUser = value;
                OnPropertyChanged();
            }
        }
        public bool AutomaticPathEntry {
            get => _automaticPathEntry;
            set {
                _automaticPathEntry = value;
                OnPropertyChanged();
            }
        }
        public MaterialType MaterialType { get; private set; }
        #endregion

        public MaterialVM() {
        }
        public MaterialVM(string title) {
            MaterialTitle = title;
        }

        #region Commands
        private ICommand _finishEditingMCommand;
        public ICommand FinishEditingMCommand => _finishEditingMCommand ??
            ( _finishEditingMCommand = new RelayCommand(FinishEditingMaterial, СanСomplete) );
        private void FinishEditingMaterial(object parameter) {
            ReadOnlyNewMaterial = true;
            Window window = (Window)parameter;
            window.DialogResult = true;
            window.Close();
        }
        private bool СanСomplete(object parameter) {
            bool isPathAndTitleValid = ReadyToCreate();
            return isPathAndTitleValid;
        }
        private bool ReadyToCreate() {
            /// Необходимость локальных переменных m_title m_path
            /// условие проверяется в цикле
            /// пользователь может изменить одно из свойств в средине проверки
            string m_title = MaterialTitle;
            string m_path = PathToMaterial;

            bool titleAndPathNotIsNull = ( !string.IsNullOrWhiteSpace(m_title) ) && ( !string.IsNullOrWhiteSpace(m_path) );
            bool absentInvalidCharacters = ( m_path + m_title ).IndexOfAny(Path.GetInvalidPathChars()) == -1;

            if (titleAndPathNotIsNull && absentInvalidCharacters) {

                if (MaterialType == MaterialType.Local && Directory.Exists(Path.GetDirectoryName(m_path))) {
                    return true;
                }
                if (MaterialType == MaterialType.Web) {
                    return true;
                }
            }
            return false;
        }


        private ICommand _getFolderCommand;
        public ICommand GetFolderCommand => _getFolderCommand ?? ( _getFolderCommand = new RelayCommand(ChooseFolder, CanChoose) );
        private void ChooseFolder(object parameter) {
            folderBrowserPath = new WinForms.FolderBrowserDialog();
            WinForms.DialogResult result = folderBrowserPath.ShowDialog();

            if (result == WinForms.DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserPath.SelectedPath)) {
                MaterialType = MaterialType.Local;
                PathToMaterial = folderBrowserPath.SelectedPath;
            }
            else {
                MaterialType = MaterialType.InvalidType;
                PathToMaterial = string.Empty;
            }
        }

        private bool CanChoose(object parameter) {
            return AutomaticPathEntry;
        }

        private ICommand _getFileCommand;
        public ICommand GetFileCommand => _getFileCommand ?? ( _getFileCommand = new RelayCommand(ChooseFile, CanChoose) );
        private void ChooseFile(object parameter) {
            folderBrowserFile = new OpenFileDialog();

            if (folderBrowserFile.ShowDialog() == true) {
                MaterialType = MaterialType.Local;
                PathToMaterial = folderBrowserFile.FileName;
            }
            else {
                MaterialType = MaterialType.InvalidType;
                PathToMaterial = string.Empty;
            }
        }


        private ICommand _getURLCommand;
        public ICommand GetURLCommand => _getURLCommand ?? ( _getURLCommand = new RelayCommand(EnterURL, CanEnterURL) );
        private void EnterURL(object parameter) {
            //Нажатие кнопки Применить
            //TODO: Сделать проверку существования сайта
            if (true) {
                PathToMaterial = PathEnteredByUser;
                MaterialType = MaterialType.Web;
                AutomaticPathEntry = true;
            }
            else {
                MessageBox.Show("404 - Страница не найдена");
            }

            PathEnteredByUser = String.Empty;
        }
        private bool CanEnterURL(object parameter) {
            string m_title = PathEnteredByUser;
            if (!string.IsNullOrWhiteSpace(m_title)) {
                return true;
            }
            else {
                return false;
            }
        }


        private ICommand _enableURLInputCommand;
        public ICommand EnableURLInputCommand => _enableURLInputCommand ?? ( _enableURLInputCommand = new RelayCommand(EnableURLInput) );
        private void EnableURLInput(object parameter) {
            // "Указать URL"
            AutomaticPathEntry = false;
        }
        #endregion

        private string _path;
        private string _title;
        private bool _automaticPathEntry = true;
        private string _pathEnteredByUser;

        private WinForms.FolderBrowserDialog folderBrowserPath;
        private OpenFileDialog folderBrowserFile;
        private bool isDisposed = false;
        public override void Dispose() {
            if (isDisposed) { return; }

            _finishEditingMCommand = null;
            _getFolderCommand = null;
            _getFileCommand = null;
            _getURLCommand = null;
            _enableURLInputCommand = null;
            folderBrowserFile = null;
            if (folderBrowserPath != null) {
                folderBrowserPath.Dispose();
                folderBrowserPath = null;
            }
            isDisposed = true;
            base.Dispose();
        }
    }
}
