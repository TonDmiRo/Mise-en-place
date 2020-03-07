using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;

namespace Launcher.Model.BuilderForUser {
    internal class JsonUserBuilder : UserBuilder {
        public override User GetUser() {
            return user;
        }
        public JsonUserBuilder(string username) {
            this.username = username;
            json = new DefaultSerializer();
           // user = new User();

            pathToUsers = ConfigurationManager.AppSettings["FilesPath"];
            afterUsernameForProjects = ConfigurationManager.AppSettings["ProjectsJson"];
            afterUsernameForUsefulM = ConfigurationManager.AppSettings["UsefulMaterialsJson"];
        }
        public override void SetUserInformation() {
            CheckFiles();

            user = json.Deserialize<User>(username);
            //дополнительные свойства
        }
        public override void SetProjects() {
            ObservableCollection<Project> projectsFromFile = json.Deserialize<ObservableCollection<Project>>(username + afterUsernameForProjects);
            foreach (var item in projectsFromFile) {
                user.ProjectCollection.AddProject(item);
            }
        }
        public override void SetMaterials() {
            Dictionary<string, Material> usefulMaterialsFromFile = json.Deserialize<Dictionary<string, Material>>(username + afterUsernameForUsefulM);
            foreach (var item in usefulMaterialsFromFile) {
                user.UsefulMaterials.Add(item.Value);
            }
        }


        private void CheckFiles() {
            if (!File.Exists($"{pathToUsers}{username}.json")) {
                throw new FileNotFoundException($"{username}.json not found!");
            }

            if (!File.Exists($"{pathToUsers}{username}{afterUsernameForProjects}")) {
                throw new FileNotFoundException($"{username}{afterUsernameForProjects} not found!");
            }

            if (!File.Exists($"{pathToUsers}{username}{afterUsernameForUsefulM}")) {
                throw new FileNotFoundException($"{username}{afterUsernameForUsefulM} not found!");
            }
        }
        private readonly string username;
        private ISerializer json;
        private User user;

        private readonly string pathToUsers;
        private readonly string afterUsernameForProjects;
        private readonly string afterUsernameForUsefulM;
    }
}
