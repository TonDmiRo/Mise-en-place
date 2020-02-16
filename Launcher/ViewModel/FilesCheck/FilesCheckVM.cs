using Launcher.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Launcher.ViewModel {
    //type of object to serialize
    public enum NameTypeInFile {
        User,
        Projects,
        UsefulMaterials
    }
    public class FilesCheckVM : BaseVM {
        public string User { get; set; }
        public string Projects { get; set; }
        public string UsefulMaterials { get; set; }
        public ObservableCollection<MyFile> MyFiles { get; set; }
        public FilesCheckVM() {
            MyFile user = new MyFile("Dimka", NameTypeInFile.User);
            MyFile projects = new MyFile("Dimka" + "'s_projects", NameTypeInFile.Projects);
            MyFile usefulMaterials = new MyFile("UsefulMaterials", NameTypeInFile.UsefulMaterials);

            MyFiles = new ObservableCollection<MyFile> {
                user,
                projects,
                usefulMaterials
            };

            foreach (var item in MyFiles) {
                item.Exists();
                item.Deserialize();
            }
        }



        public FilesCheckVM(string username) {
            MyFile user = new MyFile(username, NameTypeInFile.User);
            MyFile projects = new MyFile(username + "'s_projects", NameTypeInFile.Projects);
            MyFile usefulMaterials = new MyFile("UsefulMaterials", NameTypeInFile.UsefulMaterials);

            MyFiles = new ObservableCollection<MyFile> {
                user,
                projects,
                usefulMaterials
            };

            foreach (var item in MyFiles) {
                item.Exists();
                item.Deserialize();
            }
        }

      

    }
    public class MyFile {
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
