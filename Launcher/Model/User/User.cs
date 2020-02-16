using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Launcher.Model {
    [JsonObject(MemberSerialization.OptIn)]
    public class User : INotifyPropertyChanged {
        public User() {
            Name = "BugUser";

            UsefulMaterials = new UsefulMaterialDictionary();
            ProjectCollection = new ProjectCollection();
            _launcherLaunchDate = DateTime.Now;

            UserSerializer = new DefaultSerializer();
        }
        public User(string name) : this() {
            /// пользовательский конструктор для создании нового пользователя
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
            UsageTimeTotal = e.IncreasedTS;
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
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = "") {
            string sfad = propertyName;
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null) {
                handler.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        public void Method() {
            /// Метод для подсчета полезного времени (КПВ коэффицент полезного времени)
            /// пройтись по всем проектам получив сумму
            /// КПВ = AllTheTimeOfUse/TimeSpentOnProjects;

            TimeSpan TimeSpentOnProjects = TimeSpan.Zero;
            Project ToFirst;
            TimeSpan timeProject = TimeSpan.Zero;
            foreach (var item in ProjectCollection.Projects) {
                TimeSpentOnProjects += item.TimeSpentOnProject;

                TimeSpan time = DateTime.Now - item.NextLesson();

                if (time > timeProject) {
                    /// с помощью флага определяется самый забытый проект
                    /// можно использовать для оповещеня
                    /// В первую очередь заняться ToFirst!!!
                    timeProject = time;
                    ToFirst = item;
                }
            }
        }
    }
}
