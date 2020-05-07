using Launcher.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace Launcher.View {
    /// <summary>
    /// Interaction logic for MainV1.xaml
    /// </summary>
    public partial class MainV : Window {
        private readonly GridLength GridLengthForSidebar;
        private readonly GridLength GridLengthForOnlyListProjects;

        private readonly GridLength GridLengthForGridProject;
        public MainV() {
            InitializeComponent();


            GridLengthForSidebar = new GridLength(3, GridUnitType.Star);
            GridLengthForOnlyListProjects = new GridLength(1, GridUnitType.Star);

            GridLengthForGridProject = new GridLength(7, GridUnitType.Star);

            this.SizeChanged += OnWindowSizeChanged;
        }
        public MainV(MainVM main) : this() {
            DataContext = main;
        }

        private void OnWindowSizeChanged(object sender, SizeChangedEventArgs e) {

            if (e.NewSize.Width > 640) {
                GridProject.Visibility = Visibility.Visible;
                
                MainBody.ColumnDefinitions[0].Width = GridLengthForSidebar;
                MainBody.ColumnDefinitions[2].Width = GridLengthForGridProject;

                //todo изменить стиль
            }
            else {
                GridProject.Visibility = Visibility.Collapsed;

                MainBody.ColumnDefinitions[0].Width = GridLengthForOnlyListProjects;
                MainBody.ColumnDefinitions[2].Width = GridLength.Auto;

                //todo вернуть стиль
            }


        }
    }
}
