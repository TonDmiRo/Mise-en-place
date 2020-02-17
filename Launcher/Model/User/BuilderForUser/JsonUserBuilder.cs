using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Launcher.Model.BuilderForUser {
    internal class JsonUserBuilder:UserBuilder {
        const string _project = "'s_projects.json";
        const string _usefulM = "'s_usefulMaterials.json";
        private User user;
        private readonly string username;
        private ISerializer json;

        public JsonUserBuilder(string username) {
            this.username = username;
            json = new DefaultSerializer();
            user = new User();
        }
        public override void SetUserInformation() {
            CheckFiles();

            user = json.Deserialize<User>(username);
            //дополнительные свойства
        }
        public override void SetProjects() {
            ObservableCollection<Project> projectsFromFile = json.Deserialize<ObservableCollection<Project>>(username + _project);
            foreach (var item in projectsFromFile) {
                user.ProjectCollection.AddProject(item);
            }
        }
        public override void SetMaterials() {
            Dictionary<string, Material> usefulMaterialsFromFile = json.Deserialize<Dictionary<string, Material>>(username + _usefulM);
            foreach (var item in usefulMaterialsFromFile) {
                user.UsefulMaterials.Add(item.Value);
            }
        }
        public override User GetUser() {
            return user;
        }

        private void CheckFiles() {
            string directory = "Users//";
            if (!File.Exists($"{directory}{username}.json")) {
                throw new FileNotFoundException($"{username}.json not found!");
            }

            if (!File.Exists($"{directory}{username}{_project}")) {
                throw new FileNotFoundException($"{username}{_project} not found!");
            }

            if (!File.Exists($"{directory}{username}{_usefulM}")) {
                throw new FileNotFoundException($"{username}{_usefulM} not found!");
            }

        }
    }
}
