using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Launcher.Model {
    public abstract class Material : INotifyPropertyChanged {
        protected Material(string name, string path) {
            MaterialTitle = name;
            PathToMaterial = path;

            Exists = true; // при создании есть проверка на существование Creator.CanCreate(string title,string path)
        }
        #region prop
        [JsonProperty("Title")]
        public string MaterialTitle { get; protected set; }
        [JsonProperty("Path")]
        public string PathToMaterial { get; protected set; }
        [JsonProperty("OpensAtLaunch")]
        public bool OpensAtLaunch {
            get => _opensAtLaunch;
            set {
                _opensAtLaunch = value;
                OnPropertyChanged();
            }
        }
        [JsonProperty("Exists")]
        public bool Exists {
            /// необходим для триггера ProjectMaterials_DG
            /// меняет цвет материала на красный если Exists= false
            get => _exists;
            set {
                _exists = value;
                OnPropertyChanged();
            }
        }

        // TODO: если Exists false необходимо зафиксировать причину. string причина?
        #endregion
        public void OpenMaterial() {
            CheckMaterial();
            StartMaterial();
        }
        protected abstract void StartMaterial();
        protected virtual void CheckMaterial() {
            if (string.IsNullOrWhiteSpace(PathToMaterial)) {
                BlockMaterial();
                throw new Exception("Путь не может быть пустым!");
            }
        }
       
        public void BlockMaterial() {
            OpensAtLaunch = false;
            Exists = false;
        }
        private void UnlockMaterial() {
            OpensAtLaunch = true;
            Exists = true;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = "") {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null) {
                handler.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
        private bool _opensAtLaunch;
        private bool _exists;
    }
}