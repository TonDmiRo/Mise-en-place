using Launcher.ViewModel;
using System;
using System.Windows;

namespace Launcher.View {
    /// <summary>
    /// Логика взаимодействия для URLCheck.xaml
    /// </summary>
    public partial class URLCheckV : Window, IDisposable {

        public URLCheckV(URLCheckVM viewModel) {
            DataContext = viewModel;
            viewModel.ChangeDialogResult += ChangeDialogResultMethod;

            InitializeComponent();
        }

        private void ChangeDialogResultMethod() {
            this.DialogResult = true;
            this.Close();
        }

        public void Dispose() {
            // Dispose of unmanaged resources.
            //Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
            //throw new NotImplementedException();
        }
    }
}
