using System.Windows;

namespace Launcher {
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application {
        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);
            ViewModel.LoginVM viewModel = new ViewModel.LoginVM();
            View.LoginV window = new View.LoginV() { DataContext = viewModel };
            window.Show();
        }
    }


}
