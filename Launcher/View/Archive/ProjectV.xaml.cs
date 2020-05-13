using Launcher.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Launcher.View {
    /// <summary>
    /// Логика взаимодействия для ProjectView.xaml
    /// </summary>
    public partial class ProjectV : Page {
        public ProjectV(ProjectVM vm) {

           
            InitializeComponent();
            DataContext = vm;
        }
        private void ToChangeMaterials_Click(object sender, System.Windows.RoutedEventArgs e) {
            bool EditingEnabled = ( sender as ToggleButton ).IsChecked ?? false;

            int indexRemoveProjectMaterial = ProjectMaterials_DG.Columns.IndexOf(removeProjectMaterial_column);

            if (EditingEnabled) {
                ProjectMaterials_DG.Columns[indexRemoveProjectMaterial].Visibility = Visibility.Visible;
                panelForAddMaterials.Visibility = Visibility.Visible;
            }
            else {
                ProjectMaterials_DG.Columns[indexRemoveProjectMaterial].Visibility = Visibility.Hidden;
                panelForAddMaterials.Visibility = Visibility.Hidden;
            }
        }

        private void ToChangeTasks_Click(object sender, System.Windows.RoutedEventArgs e) {
            bool EditingEnabled = ( sender as ToggleButton ).IsChecked ?? false;

            int indexNameTask = Task_DG.Columns.IndexOf(NameTask_colum);
            int indexDeleteTask = Task_DG.Columns.IndexOf(deleteTask_column);

            if (EditingEnabled) {
                Task_DG.Columns[indexDeleteTask].Visibility = Visibility.Visible;
                panelForAddTasks.Visibility = Visibility.Visible;
            }
            else {
                Task_DG.Columns[indexDeleteTask].Visibility = Visibility.Hidden;
                panelForAddTasks.Visibility = Visibility.Hidden;
            }
            Task_DG.Columns[indexNameTask].IsReadOnly = !Task_DG.Columns[indexNameTask].IsReadOnly;
        }

       

     
    }
}