using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Launcher.Model {
    [JsonObject(MemberSerialization.OptIn)]
    public class User : INotifyPropertyChanged {
        public User() {
            Projects = new UserProjects(new ObservableCollection<Project>());
            _listProjects = new List<string>();
            ListProjects = new ReadOnlyCollection<string>(_listProjects);
            _launcherLaunchDate = DateTime.Now;
            UserSerializer = new DefaultSerializer();
        }
        public User(string name) : this() {
            /// конструктор для создании нового пользователя
            Name = name;
            TotalUsageTime = TimeSpan.Zero;
            ///новый пользователь не вызывает метод SetTimeSpanMagnifier 
            magnifierForUsageTimeTotal = new TimeSpanMagnifier(TotalUsageTime, ChangedUsageTimeTotal);

        }

        public void SetHandlers(PropertyChangedEventHandler PropUserChanged, NotifyCollectionChangedEventHandler NotifyCollectionChanged) {
            PropertyChanged += PropUserChanged;
            Projects.AddPropertyChangedEventHandler(NotifyCollectionChanged);
        }

        [JsonProperty]
        public string Name { get; private set; }
        [JsonProperty]
        public TimeSpan TotalUsageTime { get; private set; }
        [JsonProperty]
        public TimeSpan ProductiveTime { get; private set; }

        public ISerializer UserSerializer { get; set; }
        //поля для учета времени
        private readonly DateTime _launcherLaunchDate;

        public UserProjects Projects { get; private set; }

        public ReadOnlyCollection<string> ListProjects { get; private set; }
        [JsonProperty]
        private readonly List<string> _listProjects;

        #region Magnifier
        private TimeSpanMagnifier magnifierForUsageTimeTotal;
        private void ChangedUsageTimeTotal(object sender, TimeSpanHasChangedEventArgs e) {
            TotalUsageTime = e.IncreasedTimeSpan;
            OnPropertyChanged(nameof(TotalUsageTime));
        }
        [OnDeserialized]
        private void SetTimeSpanMagnifier(StreamingContext context) {
            magnifierForUsageTimeTotal = new TimeSpanMagnifier(TotalUsageTime, ChangedUsageTimeTotal);
        }
        #endregion

        #region User Serializer
        public void SaveUser() {
            if (UserSerializer != null) {
                SerializeUser();
                Projects.SerializeCollection(Name, UserSerializer);
            }
            else {
                throw new Exception("UserSerializer не задан!");
            }
        }
        private void SerializeUser() {
            UpdateListProjects();
            string userPath = Path.Combine(System.Configuration.ConfigurationManager.AppSettings["FilesPath"], Name, Path.ChangeExtension(Name,"json"));
            UserSerializer.Serialize(userPath, this);
        }
        private void UpdateListProjects() {
            _listProjects.Clear();

            var list = Projects.Select(p => p.ProjectName);
            _listProjects.AddRange(list);
            /// TODO: добавлять проекты отсутствующие в списке в архив
            /// или это делать на этапе проверки папки?
        }
        #endregion

        #region INotifyPropertyChanged
        //TODO: delete
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = "") {
            string sfad = propertyName;
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null) {
                handler.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
