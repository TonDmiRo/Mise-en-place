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
        public event Action ChangeDialogResult;
        public bool ReadOnlyNewMaterial { get; private set; }
        public string MaterialTitle {
            get => _title;
            set {
                if (!ReadOnlyNewMaterial) {
                    _title = value;
                    OnPropertyChanged("MaterialTitle");
                }
            }
        }
        public string PathToMaterial {
            get => _path;
            set {
                if (!ReadOnlyNewMaterial) {
                    _path = value;
                    OnPropertyChanged("PathToMaterial");
                }
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
                        ChangeDialogResult();// this.DialogResult = true; this.Close();
                    }
                    bool canExecute(object obj) {
                        bool isPathAndNameValid = ReadyToCreate();
                        return isPathAndNameValid;
                    }
                    bool ReadyToCreate() {
                        /// Необходимость локальных переменных
                        /// условие проверяется в цикле
                        /// пользователь может изменить одно из свойств в средине проверки
                        string m_title = MaterialTitle;
                        string m_path = PathToMaterial;

                        bool titleAndPathNotIsNull = ( !string.IsNullOrWhiteSpace(m_title) ) && ( !string.IsNullOrWhiteSpace(m_path) );
                        bool absentInvalidCharacters = ( m_path + m_title ).IndexOfAny(Path.GetInvalidPathChars()) == -1;

                        if (titleAndPathNotIsNull && absentInvalidCharacters) {

                            if (MaterialType==MaterialType.Local && Directory.Exists(Path.GetDirectoryName(m_path))) {
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
                //break;
                case MaterialType.Local:
                    material = new LocalMaterialCreator().CreateMaterial(MaterialTitle, PathToMaterial);
                    break;
                case MaterialType.Web:
                    //TODO: сделать web_button
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
                    getPath = new RelayCommand<object>(execute);
                    void execute(object obj) {
                        PathToMaterial = SelectedPath();

                    }
                    string SelectedPath() {
                        WinForms.FolderBrowserDialog folderBrowser = new WinForms.FolderBrowserDialog();

                        WinForms.DialogResult result = folderBrowser.ShowDialog();
                        if (result == WinForms.DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowser.SelectedPath)) {
                            MessageBox.Show(folderBrowser.SelectedPath);
                            MaterialType = MaterialType.Local;
                            return folderBrowser.SelectedPath;
                        }
                        MaterialType = MaterialType.InvalidType;
                        return string.Empty;
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
                    getFile = new RelayCommand<object>(execute);
                    void execute(object obj) {
                        PathToMaterial = SelectedFile();
                    }
                    string SelectedFile() {
                        OpenFileDialog folderBrowser = new OpenFileDialog();

                        if (folderBrowser.ShowDialog() == true) {
                            MaterialType = MaterialType.Local;
                            return folderBrowser.FileName;
                        }
                        MaterialType = MaterialType.InvalidType;
                        return string.Empty;
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
                    getURL = new RelayCommand<object>(execute);
                    void execute(object obj) {
                        SelectedURL();
                    }

                    void SelectedURL() {
                        URLCheckVM viewModel = new URLCheckVM();
                        using (View.URLCheckV window = new View.URLCheckV(viewModel)) {
                            var result = window.ShowDialog();
                            if (result == true) {
                                PathToMaterial = viewModel.URL;
                                MaterialType = MaterialType.Web;
                            }

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
    }
}
