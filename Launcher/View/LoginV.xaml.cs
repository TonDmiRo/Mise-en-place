using System;
using System.Windows;

namespace Launcher.View {
    /// <summary>
    /// Логика взаимодействия для Login.xaml
    /// </summary>
    public partial class LoginV : Window, IDisposable {
        public LoginV() {
            InitializeComponent();
        }
        public void Dispose() {
            DataContext = null;
        }
    }
}
