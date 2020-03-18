using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;

namespace Launcher.Model {
    public class ProjectCollection : ISaveCollection {
        public ProjectCollection() {
            serializer = new DefaultSerializer();

            _projects = new ObservableCollection<Project>();
            Projects = new ReadOnlyObservableCollection<Project>(_projects);
        }
        public void AddProject(Project item) {
            _projects.Add(item);
        }
        public bool RemoveProject(Project item) {
            return _projects.Remove(item);
        }
        
        public readonly ReadOnlyObservableCollection<Project> Projects;

        private readonly ObservableCollection<Project> _projects;

        #region Serialize
        public void SetSerializer(ISerializer serializer) {
            this.serializer = serializer;
        }
        public void SerializeCollection(string collectionOwner) {
            serializer.Serialize(collectionOwner + "'s_projects", _projects);
        }
        public void SerializeCollection(string collectionOwner, ISerializer serializer) {
            SetSerializer(serializer);
            SerializeCollection(collectionOwner);
        }
        private ISerializer serializer;
        #endregion
    }
}
