using Launcher.Model;
using Launcher.Model.BuilderForUser;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace Launcher.ViewModel {
    internal class LoginVM : BaseVM {
        public string Username { get; set; }
        public LoginVM() {
            pathToUsers = ConfigurationManager.AppSettings["FilesPath"];
            afterUsernameForProjects = ConfigurationManager.AppSettings["ProjectsJson"];
            afterUsernameForUsefulM = ConfigurationManager.AppSettings["UsefulMaterialsJson"];

            _usernames = GetListUsername();
        }

        private ICommand signInLauncher;
        public ICommand SignInLauncher => signInLauncher ?? ( signInLauncher = new RelayCommand(LaunchLauncher, CanLaunch) );
        private void LaunchLauncher(object parameter) {
            if (_usernames.Contains(Username)) {
                try {
                    User user = GetUser();
                    MainVM viewModel = new MainVM(user);
                    MainV mainV = new MainV(viewModel);
                    mainV.Show();

                    View.LoginV window = (View.LoginV)parameter;
                    window.Close();
                    window.Dispose();
                }
                catch (Exception e) {
                    MessageBox.Show(e.Message);
                }
            }
            else { MessageBox.Show("Пользователь не существует!"); }
        }
        private User GetUser() {
            Administrator admin;
            UserBuilder builder = new JsonUserBuilder(Username);
            admin = new Administrator(builder);
            admin.Construct();
            User user = builder.GetUser();
            return user;
        }
        private bool CanLaunch(object parameter) {
            string str = Username;
            if (string.IsNullOrWhiteSpace(str)) { return false; }
            return true;
        }


        private ICommand createUserCommand;
        public ICommand CreateUserCommand => createUserCommand ?? ( createUserCommand = new RelayCommand(CreateUser, CanCreateUser) );
        private void CreateUser(object parameter) {
            Administrator admin;
            UserBuilder newBuilder = new NewUserBuilder(Username);
            admin = new Administrator(newBuilder);
            admin.Construct();
            newBuilder.GetUser().SaveUser();
            _usernames.Add(Username);

            MessageBox.Show("Пользователь создан. Нажмите войти.");
        }
        private bool CanCreateUser(object parameter) {
            string str = Username;
            if (string.IsNullOrWhiteSpace(str)) { return false; }

            int indexOfSubstring = str.IndexOf("'");
            if (indexOfSubstring >= 0) { return false; }

            if (_usernames.Contains(str)) { return false; }


            return true;
        }


        private List<string> GetListUsername() {
            //TODO: создать модель для проверки файлов
            List<string> usernames = new List<string>();
            if (CheckDirectory()) {
                DirectoryInfo Users = new DirectoryInfo(pathToUsers);
                FileInfo[] usersFiles = Users.GetFiles();
                foreach (var file in usersFiles) {
                    string fileName = file.Name;

                    bool fileOwnedByUser = false;
                    string username = String.Empty;
                    int indexOfSubstring = fileName.IndexOf(afterUsernameForProjects);
                    if (indexOfSubstring > 0) {
                        fileOwnedByUser = true;
                        username = fileName.Remove(indexOfSubstring);
                    }
                    else {
                        indexOfSubstring = fileName.IndexOf(afterUsernameForUsefulM);
                        if (indexOfSubstring > 0) {
                            fileOwnedByUser = true;
                            username = fileName.Remove(indexOfSubstring);
                        }
                    }

                    if (fileOwnedByUser == false) {
                        username = Path.GetFileNameWithoutExtension(fileName);
                    }

                    if (( username != String.Empty ) && ( !usernames.Contains(username) )) {
                        usernames.Add(username);
                    }

                }
            }
            return usernames;
        }
        private bool CheckDirectory() {
            if (Directory.Exists(pathToUsers)) {
                return true;
            }
            else {
                Directory.CreateDirectory(pathToUsers);
                return false;
            }
        }

        private readonly string pathToUsers;
        private readonly string afterUsernameForProjects;
        private readonly string afterUsernameForUsefulM;

        private readonly List<string> _usernames;
    }
}