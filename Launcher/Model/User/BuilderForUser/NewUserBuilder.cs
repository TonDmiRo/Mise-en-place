using System;
using System.Reflection;

namespace Launcher.Model.BuilderForUser {
    internal class NewUserBuilder : UserBuilder {

        private readonly User _user;
        private readonly string myDirectory;
        public NewUserBuilder(string username) {
            _user = new User(username);
            myDirectory = Environment.CurrentDirectory;

        }
        public override void SetUserInformation() {
            // не используется
        }

        public override void PrepareCustomFiles() {
            // не используется
        }

        public override void SetProjects() {
            Project newProject = new Project("Первый проект");

            LocalMaterialCreator creator = new LocalMaterialCreator();
            newProject.Materials.Add(creator.CreateMaterial("Материал проекта", myDirectory + "\\help.txt"));

            newProject.ProjectTasks.Add(new Task("Первая задача"));

            _user.Projects.Add(newProject);
        }
        
        public override User GetUser() {
            return _user;
        }

      
    }


}
