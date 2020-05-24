using Launcher.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace Launcher.View {
    /// <summary>
    /// Interaction logic for MainV1.xaml
    /// </summary>
    public partial class MainV : Window {
        private readonly GridLength GridLengthForSidebar;
        private readonly GridLength GridLengthForProjectContent;

        public MainV() {
            InitializeComponent();
            GridLengthForSidebar = new GridLength(3, GridUnitType.Star);
            GridLengthForProjectContent = new GridLength(7, GridUnitType.Star);

            this.SizeChanged += OnWindowSizeChanged;
        }
        public MainV(MainVM main) : this() {
            DataContext = main;
        }
        private bool widthReachedMinSize; // интервал всего 140 
        private void OnWindowSizeChanged(object sender, SizeChangedEventArgs e) {
            if (e.NewSize.Width > 640) {
                if (widthReachedMinSize) {
                    ShowAllGrid();
                   
                    ColForProjectsSidebar.Width = GridLengthForSidebar;
                    ColForProjectContent.Width = GridLengthForProjectContent;
                    widthReachedMinSize = false;

                    BackButton.Visibility = Visibility.Collapsed;
                }
            }
            else {
                widthReachedMinSize = true;
                if (ListProjects.SelectedItem == null) {
                    ShowOnlyOneGrid(ProjectsSidebar);
                    ShowOnlyOneColumn(ColForProjectsSidebar);
                    
                }
                else {
                    ShowOnlyOneGrid(ProjectContent);
                    ShowOnlyOneColumn(ColForProjectContent);

                    BackButton.Visibility = Visibility.Visible;
                }
            }
        }


        private void ShowOnlyOneColumn(ColumnDefinition column) {
            column.Width = new GridLength(1, GridUnitType.Star);

            foreach (var item in MainGrid.ColumnDefinitions) {
                if (item != column) { item.Width = GridLength.Auto; }
            }
        }
        private void ShowOnlyOneGrid(Grid grid) {
            grid.Visibility = Visibility.Visible;
            foreach (UIElement item in MainGrid.Children) {
                if (item != grid) { item.Visibility = Visibility.Collapsed; }
            }
        }
        private void ShowAllGrid() {
            foreach (UIElement item in MainGrid.Children) {
                item.Visibility = Visibility.Visible;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e) {
            ListProjects.SelectedItem = null;
            this.Width ++;
            this.Width --;
        }
    }
}
