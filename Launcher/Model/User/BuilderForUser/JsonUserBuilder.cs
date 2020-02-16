using System.Collections.Generic;
using System.Collections.ObjectModel;

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
            user = json.Deserialize<User>(username);
            //дополнительные свойства
        }
        public override void SetProjects() {

            ObservableCollection<Project> projectsFromFile = json.Deserialize<ObservableCollection<Project>>(user.Name + "'s_projects");

            foreach (var item in projectsFromFile) {
                user.ProjectCollection.AddProject(item);
            }

        }
        public override void SetMaterials() {
            Dictionary<string, Material> usefulMaterialsFromFile = json.Deserialize<Dictionary<string, Material>>(user.Name + "'s_usefulMaterials");
            foreach (var item in usefulMaterialsFromFile) {
                user.UsefulMaterials.Add(item.Value);
            }
        }
        public override User GetUser() {
            return user;
        }
    }
}
