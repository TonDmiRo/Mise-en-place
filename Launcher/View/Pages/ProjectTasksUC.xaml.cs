using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Launcher.View.Pages
{
    /// <summary>
    /// Interaction logic for TasksUC.xaml
    /// </summary>
    public partial class ProjectTasksUC : UserControl
    {
        public ProjectTasksUC()
        {
            InitializeComponent();
        }

        private void ToChangeTasks_Click(object sender, System.Windows.RoutedEventArgs e) {
            bool EditingEnabled = ( sender as ToggleButton ).IsChecked ?? false;

            int indexNameTask = Task_DG.Columns.IndexOf(NameTask_colum);
            int indexDeleteTask = Task_DG.Columns.IndexOf(deleteTask_column);

            if (EditingEnabled) {
                Task_DG.Columns[indexDeleteTask].Visibility = Visibility.Visible;
            }
            else {
                Task_DG.Columns[indexDeleteTask].Visibility = Visibility.Hidden;
            }
            Task_DG.Columns[indexNameTask].IsReadOnly = !Task_DG.Columns[indexNameTask].IsReadOnly;
        }
    }
}
