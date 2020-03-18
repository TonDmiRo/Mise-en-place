using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Launcher.Model {
    [JsonObject(MemberSerialization.OptIn)]
    public class User : INotifyPropertyChanged {
        public User() {
            UsefulMaterials = new UsefulMaterialDictionary();
            ProjectCollection = new ProjectCollection();
            _launcherLaunchDate = DateTime.Now;

            UserSerializer = new DefaultSerializer();
        }
        public User(string name) : this() {
            /// конструктор для создании нового пользователя
            Name = name;
            UsageTimeTotal = TimeSpan.Zero;
            ///новый пользователь не вызывает метод SetTimeSpanMagnifier 
            magnifierForUsageTimeTotal = new TimeSpanMagnifier(UsageTimeTotal, ChangedUsageTimeTotal);

        }

        [JsonProperty]
        public string Name { get; private set; }
        [JsonProperty]
        public TimeSpan UsageTimeTotal { get; private set; }
        public ISerializer UserSerializer { get; set; }
        //поля для учета времени
        private readonly DateTime _launcherLaunchDate;
        public ProjectCollection ProjectCollection { get; private set; }
        public UsefulMaterialDictionary UsefulMaterials { get; private set; }

        #region Magnifier
        private TimeSpanMagnifier magnifierForUsageTimeTotal;
        private void ChangedUsageTimeTotal(object sender, TimeSpanHasChangedEventArgs e) {
            UsageTimeTotal = e.IncreasedTimeSpan;
        }
        [OnDeserialized]
        private void SetTimeSpanMagnifier(StreamingContext context) {
            magnifierForUsageTimeTotal = new TimeSpanMagnifier(UsageTimeTotal, ChangedUsageTimeTotal);
        }
        #endregion

        #region User Serializer
        public void SaveUser() {
            if (UserSerializer != null) {
                SerializeUser();
                ProjectCollection.SerializeCollection(Name, UserSerializer);
                UsefulMaterials.SerializeCollection(Name, UserSerializer);
            }
        }
        private void SerializeUser() {
            UserSerializer.Serialize(Name, this);
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
