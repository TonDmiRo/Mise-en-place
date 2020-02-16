using System;

namespace Launcher.ViewModel {
    public class URLCheckVM : BaseVM {

        public event Action ChangeDialogResult;
        public bool ReadOnlyNewMaterial { get; private set; }
        public string URL {
            get => _uRL;
            set {
                if (!ReadOnlyNewMaterial) {
                    _uRL = value;
                   // OnPropertyChanged("URL");
                }
            }
        }


        private RelayCommand<object> setURL;
        public RelayCommand<object> SetURL {
            get {
                if (setURL == null) {
                    setURL = new RelayCommand<object>(execute, canExecute);
                    void execute(object obj) {
                        ReadOnlyNewMaterial = true;
                        ChangeDialogResult();// this.DialogResult = true; this.Close();
                    }
                    bool canExecute(object obj) {
                        bool isURLValid = ReadyToCreate();
                        return isURLValid;
                    }
                    bool ReadyToCreate() {
                        string m_title = URL;
                        bool URLhNotIsNull = ( !string.IsNullOrWhiteSpace(m_title) );
                        if (URLhNotIsNull && URLhNotIsNull) {
                            //TODO: нет проверок на отклик сайта
                            
                                return true;
                        }
                        return false;
                    }

                    return setURL;
                }
                return setURL;

            }
        }



        private string _uRL;
    }
}
