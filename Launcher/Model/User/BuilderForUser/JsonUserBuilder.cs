using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace Launcher.Model.BuilderForUser {

    internal class JsonUserBuilder : UserBuilder {
        public override User GetUser() {
            return _user;
        }
        public JsonUserBuilder(string username) {
            this.username = username;
            userPath = Path.Combine(ConfigurationManager.AppSettings["FilesPath"], username, Path.ChangeExtension(username, extension));
            projectsPath = Path.Combine(ConfigurationManager.AppSettings["FilesPath"], username, "Projects");
            pathToArchive = Path.Combine(ConfigurationManager.AppSettings["FilesPath"], username, "Archive");

            json = new DefaultSerializer();

            _preparedProjects = new List<Project>();
            afterUsernameForProjects = ConfigurationManager.AppSettings["ProjectsJson"];
        }
        public override void SetUserInformation() {
            CheckUser();
            _user = json.Deserialize<User>(userPath);

        }
        private void CheckUser() {
            if (!File.Exists(userPath)) {
                throw new Exception($"{username}.json not found!");
            }
        }

        private readonly List<Project> _preparedProjects;
        private bool possiblySetFiles = false;
        public override void PrepareCustomFiles() {
            if (!possiblySetFiles) {
                CheckProjectsExistence();
                PrepareListForInstallation(_preparedProjects);
            }
        }

        #region Methods for PrepareCustomFiles
        private bool filesWereNotChecked = true;
        private void CheckProjectsExistence() {
            if (!Directory.Exists(projectsPath)) {
                Directory.CreateDirectory(projectsPath);
                throw new FileNotFoundException("Поместите ваши проекты по этому адресу: " + projectsPath);
            }

            if (filesWereNotChecked) {
                string[] projectsFile = Directory.GetFiles(projectsPath);
                if (projectsFile.Length == 0) {
                    filesWereNotChecked = false;
                    throw new FileNotFoundException("Проекты не найдены!");
                }
            }
        }

        private void PrepareListForInstallation(List<Project> list) {
            List<string> whitelist = _user.ListProjects.ToList();
            string[] projectsFromDirectory = Directory.GetFiles(projectsPath);
            List<Project> filteredProjects = GetFilteredProjects(whitelist, projectsFromDirectory);

            foreach (var project in filteredProjects) {
                if (!list.Contains(project)) {
                    list.Add(project);
                }
            }

            if (list.Count != 0) {
                // список не полный, но пользователь может продолжить
                possiblySetFiles = true;
            }
            else {
                // создаем первый проект
                list.Add(new Project("NewProject"));
            }

            if (whitelist.Count != 0) {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (var item in whitelist) {
                    stringBuilder.AppendLine(item);
                }
                stringBuilder.AppendLine("Поместите ваши проекты по этому адресу: " + projectsPath);
                throw new FileNotFoundException("Отсутствующие проекты:\n" + stringBuilder);
            }
        }

        private List<Project> GetFilteredProjects(List<string> whitelist, string[] projectsFromDirectory) {
            if (!Directory.Exists(pathToArchive)) { Directory.CreateDirectory(pathToArchive); }

            List<Project> filteredProjects = new List<Project>();
            foreach (var item in projectsFromDirectory) {
                string nameFile = Path.GetFileNameWithoutExtension(item);
                if (whitelist.Contains(nameFile)) {
                    filteredProjects.Add(json.Deserialize<Project>(item));
                    whitelist.Remove(nameFile);
                }
                else {
                    // переносим в архив
                    if (File.Exists(item)) {File.Move(item, Path.Combine(pathToArchive, Path.GetFileName(item)));}
                }
            }
            return filteredProjects;
        }
        #endregion

        public override void SetProjects() {

            foreach (var project in _preparedProjects) {
                _user.Projects.Add(project);
            }

            //ObservableCollection<Project> projectsFromFile = json.Deserialize<ObservableCollection<Project>>(Path.Combine(ConfigurationManager.AppSettings["FilesPath"], username + afterUsernameForProjects));
            //foreach (var item in projectsFromFile) {
            //    _user.ProjectCollection.AddProject(item);
            //}
        }


        private readonly string username;
        private readonly string userPath;
        private readonly string projectsPath;
        private readonly string pathToArchive;

        private User _user;
        private const string extension = ".json";
        private ISerializer json;

        #region ForExeption
        // TODO: бул переменая предупредил что фалов нет
        #endregion
        // TODO: delete
        private readonly string afterUsernameForProjects;
        private readonly string afterUsernameForUsefulM;
    }


}
