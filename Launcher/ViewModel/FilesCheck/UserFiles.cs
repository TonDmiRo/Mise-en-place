using Launcher.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher.ViewModel.FilesCheck {
    class UserFiles {
        string Username { get; set; }
        private MyFile userInformation;
        private MyFile userProjects;
        private MyFile userUsefulMaterials;

       
        public UserFiles(string username) {
            Username = username;
            userInformation = new MyFile(Username, NameTypeInFile.User);
            userProjects = new MyFile(Username + "'s_projects", NameTypeInFile.Projects);
            userUsefulMaterials = new MyFile("usefulMaterials", NameTypeInFile.UsefulMaterials);
        }

        private class MyFile {
            public string FileName { get; }
            public string FileInfo { get; set; }
            public bool Exist { get; set; }
            public bool IsDeserializable { get; set; }


            private readonly NameTypeInFile type;
            public MyFile(string objectName, NameTypeInFile type) {
                FileName = objectName;
                this.type = type;
            }

            public object Deserialize() {
                object myObject = new object();
                try {
                    switch (type) {
                        case NameTypeInFile.User:
                            myObject = Deserialize<User>();
                            break;
                        case NameTypeInFile.Projects:
                            myObject = Deserialize<ObservableCollection<Project>>();
                            break;
                        case NameTypeInFile.UsefulMaterials:
                            myObject = Deserialize<Dictionary<string, Material>>();
                            break;

                        default:
                            throw new Exception("Тип не найден!");
                    }

                    FileInfo = $"{FileName} найден";
                    IsDeserializable = true;
                }

                catch (System.Exception e) {
                    FileInfo = $"{FileName} Невозможно десериализовать.\n Причина:{e.Message}";
                    IsDeserializable = false;
                }


                return myObject;
            }
            public T Deserialize<T>() {
                T myObject = new DefaultSerializer().Deserialize<T>(FileName);
                return myObject;
            }

            public void Exists() {
                Exist = File.Exists(Path.ChangeExtension(FileName, "json"));
            }

            public void Recreate() { }
            ///Нужен сериализатор Т
        }
    }
}
