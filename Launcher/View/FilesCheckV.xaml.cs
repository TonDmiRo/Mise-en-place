using Launcher.ViewModel;
using System;
using System.Windows;

namespace Launcher.View {
    /// <summary>
    /// Логика взаимодействия для FilesCheckV.xaml
    /// </summary>
    public partial class FilesCheckV : Window, IDisposable {
        public FilesCheckV(FilesCheckVM vm) {
            DataContext = vm;
            InitializeComponent();
        }
        public void Dispose() {
            // throw new NotImplementedException();
        }
    }
}
