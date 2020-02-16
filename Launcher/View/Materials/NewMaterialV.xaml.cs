using Launcher.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Launcher.View {
    /// <summary>
    /// Логика взаимодействия для AddMaterialV.xaml
    /// </summary>
    public partial class NewMaterialV : Window, IDisposable {
        public NewMaterialV(MaterialVM vm) {

            DataContext = vm;
            vm.ChangeDialogResult += ChangeDialogResultMethod;
            InitializeComponent();
        }

        // событие которое опевестит вью о закрытии и изменении занчения DialogResult
        private void ChangeDialogResultMethod() {
            this.DialogResult = true;
            this.Close();
        }
        public void Dispose() {
            // throw new NotImplementedException();
        }
        private void MaterialTitle_tb_GotFocus(object sender, RoutedEventArgs e) {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            tb.GotFocus -= MaterialTitle_tb_GotFocus;
        }
    }
}
