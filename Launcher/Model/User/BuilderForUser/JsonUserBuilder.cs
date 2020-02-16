using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Launcher.Model.BuilderForUser {
    internal class JsonUserBuilder : UserBuilder {
        private User user;
        private readonly string username;
        private ISerializer json;

        public JsonUserBuilder(string username) {
            this.username = username;
            json = new DefaultSerializer();
            user = new User();
        }
        public override void SetUserInformation() {
            if (File.Exists($"{username}.json")) {
                user = json.Deserialize<User>(username);
            }
            throw new FileNotFoundException($"{username}.json not found!");
            //дополнительные свойства
        }
        public override void SetProjects() {
            string projects = user.Name + "'s_projects";
            if (File.Exists($"{projects}.json")) {
                ObservableCollection<Project> projectsFromFile = json.Deserialize<ObservableCollection<Project>>(projects);
                foreach (var item in projectsFromFile) {
                    user.ProjectCollection.AddProject(item);
                }
            }
            throw new FileNotFoundException($"{projects}.json not found!");
        }
        public override void SetMaterials() {
            string projects = user.Name + "'s_usefulMaterials";
            if (File.Exists($"{projects}.json")) {
                Dictionary<string, Material> usefulMaterialsFromFile = json.Deserialize<Dictionary<string, Material>>(projects);
                foreach (var item in usefulMaterialsFromFile) {
                    user.UsefulMaterials.Add(item.Value);
                }
            }
            throw new FileNotFoundException($"{projects}.json not found!");
        }
        public override User GetUser() {
            return user;
        }
    }
}
