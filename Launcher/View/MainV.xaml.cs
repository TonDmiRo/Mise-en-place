using Launcher.ViewModel;
using System.Windows;
namespace Launcher {
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>

    public partial class MainV : Window {
        //private const int standartWidth = 768;
        //private const int standartHeight = 640;
        //private bool standartSize = true;
        public MainV(MainVM viewModel) {
            InitializeComponent();
            DataContext = viewModel;
        }
        //private void ChangeSize() {
        //    this.MinWidth = ( standartSize == true ) ? 480 : standartWidth;
        //    this.Width = this.MinWidth;

        //    this.MinHeight = ( standartSize == true ) ? 460 : standartHeight;
        //    this.Height = this.MinHeight;
          
        //    standartSize = !standartSize;
        //}

       
    }
}
