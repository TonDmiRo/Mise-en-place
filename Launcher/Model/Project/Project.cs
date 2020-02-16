using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Launcher.Model {
    [JsonObject(MemberSerialization.OptIn)]
    public class Project : INotifyPropertyChanged {
        #region ctor
        public Project() {
            Name = "NotFound";
            RepeatInterval = 0;
            TimeSpentOnProject = new TimeSpan(0, 0, 0, 0);

            _projectMaterials = new ObservableCollection<Material>();
            ProjectMaterials = new ReadOnlyObservableCollection<Material>(_projectMaterials);
            ProjectTasks = new ObservableCollection<Task>();
        }
        public Project(string name, string goal) : this() {
            Name = name;
            Goal = goal;
        }
        #endregion
        [JsonProperty("NameProject")]
        public string Name {
            get => _name;
            private set {
                _name = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty("GoalProject")]
        public string Goal { get; set; }

        #region Time
        [JsonProperty("TimeSpentOnProject")]
        public TimeSpan TimeSpentOnProject { get; private set; }
        public int RepeatInterval { get; set; }
        public DateTime NextLesson() { return _lastStartTime.Add(TimeSpan.FromDays(RepeatInterval)); }
        [JsonProperty("lastStartTime")]
        private DateTime _lastStartTime = DateTime.Now;
        private string _name;
        #endregion
        public void RenameProject(string Name) {
            this.Name = Name;
            TimeSpentOnProject = TimeSpan.Zero;
            _lastStartTime = DateTime.Now;
        }


        #region Collections

        #region Materials
        public void Add(Material item) {
            _projectMaterials.Add(item);
        }
        public bool Remove(Material item) {
            return _projectMaterials.Remove(item);
        }
        public void SetMaterial(int indexSpoiledM, Material serviceableM) {
            _projectMaterials[indexSpoiledM] = serviceableM;
        }
        public bool OpenMarkedMaterials() {
            if (CheckForMarked()) {
              
                foreach (var item in _projectMaterials) {
                    if (item.OpensAtLaunch) {
                        try {
                            item.OpenMaterial();
                        }
                        catch (Exception) {
                            item.BlockMaterial();
                        }
                    }
                }
                return true;
            }
            return false;
        }
        private bool CheckForMarked() {
            bool containsOneMarkedM = false;
            foreach (var item in _projectMaterials) {
                containsOneMarkedM = containsOneMarkedM | item.OpensAtLaunch;
                if (containsOneMarkedM) { break; }
            }
            return containsOneMarkedM;
        }


        public readonly ReadOnlyObservableCollection<Material> ProjectMaterials;
        [JsonProperty("ProjectMaterials")]
        private readonly ObservableCollection<Material> _projectMaterials;
        #endregion

        #region Collection Task
        [JsonProperty("ProjectTasks")]
        public readonly ObservableCollection<Task> ProjectTasks;
        #endregion

        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = "") {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null) {
                handler.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

       
        #endregion
    }
}