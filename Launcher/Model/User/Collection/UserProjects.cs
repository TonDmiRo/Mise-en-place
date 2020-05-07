using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;

namespace Launcher.Model {
    public class UserProjects : ReadOnlyObservableCollection<Project>, ISaveCollection {
        internal void Add(Project project) {
            if (SetofNames.Add(project.ProjectName)) {
                _userProjects.Add(project);
            }
            else {
                throw new Exception("Проект с таким именем уже существует!");
            }
        }
        internal bool Remove(Project project) {
            SetofNames.Remove(project.ProjectName);
            return _userProjects.Remove(project);
        }
        internal void Rename(int projectIndex, string newProjectName) {
            if (Contains(newProjectName)) { throw new Exception("Проект с таким именем уже существует в коллекции!"); }
            Project projectToReplace = new Project(newProjectName);

            // Copy task
            foreach (var task in _userProjects[projectIndex].ProjectTasks) {
                projectToReplace.ProjectTasks.Add(task);
            }
            // Copy materials
            foreach (var material in _userProjects[projectIndex].Materials) {
                projectToReplace.Materials.Add(material);
            }

            projectToReplace.ProjectGoal = _userProjects[projectIndex].ProjectGoal;
            projectToReplace.RepeatInterval = _userProjects[projectIndex].RepeatInterval;

            SetofNames.Remove(_userProjects[projectIndex].ProjectName);
            SetofNames.Add(newProjectName);

            _userProjects[projectIndex] = projectToReplace;
        }
        internal bool Contains(string projectName) {
            if (SetofNames.Contains(projectName)) { return true; }
            return false;
        }

        internal UserProjects(ObservableCollection<Project> projects) : base(projects) {
            _userProjects = projects;
            SetofNames = new HashSet<string>();
            SetSerializer(new DefaultSerializer());
        }
        internal void AddPropertyChangedEventHandler(NotifyCollectionChangedEventHandler PropCollectionChanged) {
            base.CollectionChanged += PropCollectionChanged;
        }

        #region ISaveCollection
        public void SerializeCollection(string collectionOwner, ISerializer serializer) {
            SetSerializer(serializer);
            SerializeCollection(collectionOwner);
        }
        public void SerializeCollection(string collectionOwner) {
            string projectsDirectory = Path.Combine(System.Configuration.ConfigurationManager.AppSettings["FilesPath"], collectionOwner, "Projects");

            foreach (var project in _userProjects) {
                string projectPath = Path.Combine(projectsDirectory, Path.ChangeExtension(project.ProjectName, "json"));
                serializer.Serialize(projectPath, project);
            }

        }


        public void SetSerializer(ISerializer serializer) {
            this.serializer = serializer;
        }
        private ISerializer serializer;
        #endregion

        private readonly HashSet<string> SetofNames;
        private readonly ObservableCollection<Project> _userProjects;
    }
}
