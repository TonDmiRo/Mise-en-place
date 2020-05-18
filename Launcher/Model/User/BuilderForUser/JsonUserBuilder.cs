using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace Launcher.Model.BuilderForUser {

    internal class JsonUserBuilder : UserBuilder {
        private const string extension = ".json";
        private ISerializer json;

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
        }
        public override void SetUserInformation() {
            CheckUser();
            _user = json.Deserialize<User>(userPath);
        }
        private void CheckUser() {
            if (!File.Exists(userPath)) {
                throw new FileNotFoundException($"{username}.json not found!");
            }
        }

        public override void PrepareCustomFiles() {
            PrepareDirectories();
            PrepareListOfProjects();
        }

        #region Methods for PrepareCustomFiles
        private void PrepareDirectories() {
            if (!Directory.Exists(projectsPath)) {
                Directory.CreateDirectory(projectsPath);
                if (_user.ListProjects.Count > 0) {
                    throw new DirectoryNotFoundException("Поместите ваши проекты по этому адресу: \n" + projectsPath);
                }
            }
            if (!Directory.Exists(pathToArchive)) {
                Directory.CreateDirectory(pathToArchive);
            }
        }
        private void PrepareListOfProjects() {
            _preparedProjects.Clear();

            List<string> namesOfRequiredProjects = _user.ListProjects.ToList();
            string[] projectsFromDirectory = Directory.GetFiles(projectsPath);
            // добавить файлы, соответствующие именам из списка _user.ListProjects
            foreach (var item in projectsFromDirectory) {
                string projectNameFromDirectory = Path.GetFileNameWithoutExtension(item);

                if (namesOfRequiredProjects.Contains(projectNameFromDirectory)) {
                    _preparedProjects.Add(json.Deserialize<Project>(item));
                    namesOfRequiredProjects.Remove(projectNameFromDirectory);
                }
                // переносим в архив
                else {if (File.Exists(item)) { File.Move(item, Path.Combine(pathToArchive, Path.GetFileName(item))); }}
            }

            // вызвать исключение, если не удалось добавить все файлы из списка _user.ListProjects
            if (namesOfRequiredProjects.Count != 0) {
                StringBuilder stringBuilder = new StringBuilder();

                stringBuilder.AppendLine("Отсутствующие проекты:");
                foreach (var item in namesOfRequiredProjects) {
                    stringBuilder.AppendLine(item);
                }
                stringBuilder.AppendLine();

                stringBuilder.AppendLine("Поместите ваши проекты по этому адресу: " + projectsPath);
                throw new ProjectsNotFoundException(stringBuilder.ToString());
            }
        }
        #endregion

        public override void SetProjects() {
            foreach (var project in _preparedProjects) {
                _user.Projects.Add(project);
            }
        }

        private User _user;

        private readonly string username;
        private readonly string userPath;
        private readonly string projectsPath;
        private readonly string pathToArchive;

        private readonly List<Project> _preparedProjects;
    }

    internal class ProjectsNotFoundException : FileNotFoundException {
        public ProjectsNotFoundException(string message)
            : base(message) {
        }
    }
}
