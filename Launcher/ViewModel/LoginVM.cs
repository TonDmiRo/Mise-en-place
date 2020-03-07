﻿using Launcher.Model;
using Launcher.Model.BuilderForUser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Configuration;

namespace Launcher.ViewModel {
    internal class LoginVM : BaseVM {
        
        
        public string Username { get; set; }
        public LoginVM() {
            pathToUsers= ConfigurationManager.AppSettings["FilesPath"];
            afterUsernameForProjects = ConfigurationManager.AppSettings["ProjectsJson"];
            afterUsernameForUsefulM = ConfigurationManager.AppSettings["UsefulMaterialsJson"];

            _usernames = GetListUsername();
        }

        private RelayCommand<object> startupLauncher;
        public RelayCommand<object> StartupLauncher {
            get {
                if (startupLauncher == null) {
                    startupLauncher = new RelayCommand<object>(execute);
                    void execute(object obj) {
                        if (_usernames.Contains(Username)) {
                            if (StartupMainWindow()) {
                                Window window = (Window)obj;
                                window.Close();
                            }
                        }
                        else {
                            MessageBox.Show("Пользователь не существует!");
                        }
                    }
                    bool StartupMainWindow() {
                        try {
                            User user = GetUser();

                            MainVM viewModel = new MainVM(user);
                            MainV mainV = new MainV(viewModel);

                            mainV.Show();
                            return true;
                        }
                        catch (Exception e) {
                            MessageBox.Show(e.Message);
                            return false;
                        }
                    }

                    return startupLauncher;
                }
                return startupLauncher;

            }
        }

        private RelayCommand<object> createUserCommand;
        public RelayCommand<object> CreateUserCommand {
            get {
                if (createUserCommand == null) {
                    createUserCommand = new RelayCommand<object>(execute, canExecute);
                    void execute(object obj) {
                        CreateUser();
                        MessageBox.Show("Пользователь создан. Нажмите войти.");

                    }
                    bool canExecute(object obj) {
                        return CheckUsername();
                    }
                    bool CheckUsername() {
                        string str = Username;
                        if (string.IsNullOrWhiteSpace(str))
                            return false;

                        int indexOfSubstring = str.IndexOf("'");
                        if (indexOfSubstring >= 0)
                            return false;

                        if (_usernames.Contains(str))
                            return false;

                        return true;
                    }

                    return createUserCommand;
                }
                return createUserCommand;

            }
        }

        
        private User GetUser() {
            Administrator admin;
            UserBuilder builder = new JsonUserBuilder(Username);
            admin = new Administrator(builder);
            admin.Construct();
            User user= builder.GetUser();
            return user;
        }

        private void CreateUser() {
            Administrator admin;
            UserBuilder newBuilder = new NewUserBuilder(Username);
            admin = new Administrator(newBuilder);
            admin.Construct();
            newBuilder.GetUser().SaveUser();
            _usernames.Add(Username);
            // return newBuilder.GetUser();
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