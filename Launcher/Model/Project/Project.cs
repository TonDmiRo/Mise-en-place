using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Launcher.Model {
    [JsonObject(MemberSerialization.OptIn)]
    public class Project : INotifyPropertyChanged {
        #region ctor
        protected Project() {
            RepeatInterval = 3;
            TimeSpentOnProject = new TimeSpan(0, 0, 0, 0);

            ProjectTasks = new ObservableCollection<Task>();
            _projectMaterials = new ObservableCollection<Material>();
            Materials = new ProjectMaterials(_projectMaterials);
        }
        public Project(string name) : this() {
            ProjectName = name;
        }
        #endregion

        [JsonProperty]
        public string ProjectName {
            get => _name;
            private set {
                _name = value;
                OnPropertyChanged();
            }
        }


        public string ProjectGoal {
            get { if (String.IsNullOrWhiteSpace(_projectGoal)) { _projectGoal = "Укажите цель проекта"; } return _projectGoal; }
            set => _projectGoal = value;
        }

        #region Time
        [JsonProperty]
        public TimeSpan TimeSpentOnProject { get; private set; }
        [JsonProperty("RepeatInterval")]
        public int RepeatInterval { get; set; }
        public DateTime NextLesson() { return _lastStartTime.Add(TimeSpan.FromDays(RepeatInterval)); }
        public void IncreaseTimeSpentOnProjectTime(TimeSpan time) {
            if (time < TimeSpan.FromHours(2)) {
                TimeSpentOnProject += time;
            }
            else {
                TimeSpentOnProject += TimeSpan.FromHours(1);
            }
            _lastStartTime = DateTime.Now;
        }
        #endregion

        #region Collections


        public ProjectMaterials Materials { get; private set; }
        [JsonProperty("ProjectMaterials")]
        private readonly ObservableCollection<Material> _projectMaterials;


        [OnDeserialized]
        private void SetTimeSpanMagnifier(StreamingContext context) {
            Materials = new ProjectMaterials(_projectMaterials);
        }

        [JsonProperty("ProjectTasks")]
        public ObservableCollection<Task> ProjectTasks { get; }
        #endregion

        #region private
        [JsonProperty("lastStartTime")]
        private DateTime _lastStartTime = DateTime.Now;
        private string _name;
        private static Project s_emptyProject;
        [JsonProperty("ProjectGoal")]
        private string _projectGoal;
        #endregion

        public static Project EmptyProject {
            get {
                return s_emptyProject ?? (
                    s_emptyProject = new Project() {
                        ProjectName = "",
                        ProjectGoal = "Select an existing project!",
                    } );

            }
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
        /* // IEquatable<Project>
        
        public bool Equals(Project other) {
            // https://ru.stackoverflow.com/questions/841939/%d0%97%d0%b0%d1%87%d0%b5%d0%bc-%d0%bc%d1%8b-%d1%80%d0%b5%d0%b0%d0%bb%d0%b8%d0%b7%d0%be%d0%b2%d1%8b%d0%b2%d0%b0%d0%b5%d0%bc-iequatablet-%d0%b5%d1%81%d0%bb%d0%b8-equals-%d0%b5%d1%81%d1%82%d1%8c-%d0%b2-object?answertab=oldest#tab-top


            if (this == other)
                return true;
            if (other == null)
                return false;
            if (ProjectName != other.ProjectName)
                return false;
            return true;
        }
        public override bool Equals(object other) => Equals(other as Project);
        public override int GetHashCode() {
            unchecked {
                // https://stackoverflow.com/a/263416/4340086
                int hash = 2166136261;
                hash = ( 16777619 * hash ) ^ ( ProjectName?.GetHashCode() ?? 0 );
                return hash;
            }
        }
        */
    }
}