using Launcher.Model;
using Launcher.View;
using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;

namespace Launcher.ViewModel {
    internal class ProjectEditorVM : BaseVM {
        UserProjects Projects;
        public ProjectEditorVM(UserProjects projects) {
            Projects = projects;
        }

        #region  Receiver
        /*
         * Заменить на паттерн Command
         * Для отдельного класса не хватает только коллекции с которой он будет работать 
         * при создании экземпляра передавать ProjectCollection
         * =new Receiver(ProjectCollection collection);
         */
        private void Receiver(object sender, ProjectEventArgs e) {
            switch (e.Command) {
                case CommandProject.Start:
                    StartProject(e);
                    break;
                case CommandProject.Rename:
                    RenameProject(sender, e);
                    break;
                case CommandProject.Delete:
                    RemoveProject(e);
                    break;
                default:
                    break;
            }
        }

        #region Methods for receiver
        private void StartProject(ProjectEventArgs e) {
            CheckExistenceOfMaterials(e.Project.Materials);

           // bool oneWasOpen = e.Project.Materials.OpenMarkedMaterials();
            //if (!oneWasOpen) { MessageBox.Show("Материалы не выбраны!"); }

            TimeSpan time = OpenDoningV();
            if (time.TotalMinutes > 25) {
                e.Project.IncreaseTimeSpentOnProjectTime(time);
            }
            else {
                MessageBox.Show("Работали над проектом < 25 минут. Постарайтесь не отвлекаться!");
            }
        }

        private TimeSpan OpenDoningV() {
            using (DoingVM viewModel = new DoingVM()) {
                using (DoingV doingV = new DoingV(viewModel)) {
                    //TODO: не работает
                    doingV.ShowDialog();
                    return viewModel.ElapsedTime;
                }
            }
        }
      

        private void RenameProject(object sender, ProjectEventArgs e) {
            //TODO: Исправить
            if (sender is ProjectVM projectVM) {
                int projectIndex = Projects.IndexOf(e.Project);
                Projects.Rename(projectIndex, projectVM.NewName);
            }
            ///работает не правильно
            ///команда должна отрабатываться здесь 
            ///для этого необходимо ещё передовать строку

            //MessageBox.Show($"Переименование проекта: {e.Project.Name} на новое имя выполнено.");
        }
        private void RemoveProject(ProjectEventArgs e) {
            Projects.Remove(e.Project);
            //MessageBox.Show($"{e.Project.Name} удален.");
        }
        #endregion

        #endregion
        private void CheckExistenceOfMaterials(ReadOnlyObservableCollection<Material> materials) {
            StringBuilder damagedMaterials = new StringBuilder();
            foreach (var item in materials) {
                if (item.Exists != true) {
                    item.BlockMaterial();
                    damagedMaterials.AppendLine(item.MaterialTitle);
                }
            }
            if (damagedMaterials.Length > 0) {
                MessageBox.Show("Список поврежденных материалов:\n" + damagedMaterials);
            }
        }

    }
}
