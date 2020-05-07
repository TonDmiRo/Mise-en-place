using Launcher.Model;
using Launcher.Model.BuilderForUser;
using Launcher.View;
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
           
            while (true) {
                try {
                    admin.Construct();
                    break;
                }
                catch (FileNotFoundException e) {
                    MessageBoxResult result = MessageBox.Show(e.Message + "\n Продолжить?", "Файлы не найдены!", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.No) {
                        throw new Exception("Не удалось загрузить пользователя.");
                    }
                }
            }



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
            List<string> usernames = new List<string>();
            if (CheckDirectory()) {
                DirectoryInfo Users = new DirectoryInfo(pathToUsers);
                DirectoryInfo[] userDirectories = Users.GetDirectories();
                foreach (var dir in userDirectories) {
                    if (( dir.Name != String.Empty ) && ( !usernames.Contains(dir.Name) )) {
                        usernames.Add(dir.Name);
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
        private readonly List<string> _usernames;
    }
}