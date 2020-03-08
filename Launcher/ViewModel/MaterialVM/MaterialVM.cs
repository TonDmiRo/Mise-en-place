using Launcher.Model;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
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
        public MaterialVM() {
        }
        public MaterialVM(string title) {
            MaterialTitle = title;
        }
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

        #region Commands
        private RelayCommand<object> setMaterialValues;
        public RelayCommand<object> SetMaterialValues {
            get {
                if (setMaterialValues == null) {
                    setMaterialValues = new RelayCommand<object>(execute, canExecute);
                    void execute(object obj) {
                        ReadOnlyNewMaterial = true;
                        Window window = (Window)obj;
                        window.DialogResult = true;
                        window.Close();
                    }
                    bool canExecute(object obj) {
                        bool isPathAndNameValid = ReadyToCreate();
                        return isPathAndNameValid;
                    }
                    bool ReadyToCreate() {
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

                    return setMaterialValues;
                }
                return setMaterialValues;

            }
        }
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

        private RelayCommand<object> getPath;
        public RelayCommand<object> GetPath {
            get {
                if (getPath == null) {
                    getPath = new RelayCommand<object>(execute, canExecute);
                    void execute(object obj) {
                        PathToMaterial = SelectedPath();

                    }
                    string SelectedPath() {
                        folderBrowserPath = new WinForms.FolderBrowserDialog();
                        WinForms.DialogResult result = folderBrowserPath.ShowDialog();
                        if (result == WinForms.DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserPath.SelectedPath)) {
                            MessageBox.Show(folderBrowserPath.SelectedPath);
                            MaterialType = MaterialType.Local;
                            return folderBrowserPath.SelectedPath;
                        }
                        MaterialType = MaterialType.InvalidType;
                        return string.Empty;
                    }

                    bool canExecute(object obj) {
                        return AutomaticPathEntry;
                    }
                    return getPath;
                }
                return getPath;
            }
        }

        private RelayCommand<object> getFile;
        public RelayCommand<object> GetFile {
            get {
                if (getFile == null) {
                    getFile = new RelayCommand<object>(execute, canExecute);
                    void execute(object obj) {
                        PathToMaterial = SelectedFile();
                    }
                    string SelectedFile() {
                        folderBrowserFile = new OpenFileDialog();
                        if (folderBrowserFile.ShowDialog() == true) {
                            MaterialType = MaterialType.Local;
                            return folderBrowserFile.FileName;
                        }
                        MaterialType = MaterialType.InvalidType;
                        return string.Empty;
                    }
                    bool canExecute(object obj) {
                        return AutomaticPathEntry;
                    }
                    return getFile;
                }
                return getFile;
            }
        }


        private RelayCommand<object> getURL;
        public RelayCommand<object> GetURL {
            get {
                if (getURL == null) {
                    getURL = new RelayCommand<object>(execute, canExecute);
                    void execute(object obj) {
                        SelectedURL();
                    }
                    void SelectedURL() {
                        if (AutomaticPathEntry == true) {
                            // "Указать URL"
                            AutomaticPathEntry = false;
                        }
                        else {
                            // Нажатие кнопки Применить
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
                    }

                    bool canExecute(object obj) {
                        bool isURLValid = ReadyToCreate();
                        return isURLValid;
                    }
                    bool ReadyToCreate() {
                        if (AutomaticPathEntry == true) {
                            return true;
                        }
                        else {
                            string m_title = PathEnteredByUser;
                            bool URLhNotIsNull = ( !string.IsNullOrWhiteSpace(m_title) );
                            if (URLhNotIsNull && URLhNotIsNull) {

                                return true;
                            }
                            return false;
                        }
                    }
                    return getURL;
                }
                return getURL;

            }

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
            if (isDisposed)
                return;
            setMaterialValues = null;
            getPath = null;
            getFile = null;
            getURL = null;
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
