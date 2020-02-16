using System;
using System.Reflection;

namespace Launcher.Model.BuilderForUser {
    internal class NewUserBuilder : UserBuilder {

        private readonly User user;
        private readonly string myDirectory;
        public NewUserBuilder(string username) {
            user = new User(username);
            myDirectory = Environment.CurrentDirectory;
            
        }
        public override void SetUserInformation() {
         

        }
        public override void SetProjects() {
            Project newProject = new Project("Первый проект","Первая цель");

            CreatorLocalMaterial creator = new CreatorLocalMaterial();
            newProject.Add(creator.CreateMaterial("Материал проекта", myDirectory + "\\help.txt"));

            newProject.ProjectTasks.Add(new Task("Первая задача"));


            user.ProjectCollection.AddProject(newProject);
        }

        public override void SetMaterials() {

            CreatorLocalMaterial creator = new CreatorLocalMaterial();
            user.UsefulMaterials.Add(creator.CreateMaterial("Полезный материал", myDirectory + "\\help.txt"));
            user.UsefulMaterials.Add(creator.CreateMaterial("Полезный материал1", myDirectory + "\\help.txt"));
            user.UsefulMaterials.Add(creator.CreateMaterial("Полезный материал2", myDirectory + "\\help.txt"));
            user.UsefulMaterials.Add(creator.CreateMaterial("Полезный материал3", myDirectory + "\\help.txt"));
        }
        public override User GetUser() {
            return user;
        }
    }


}
