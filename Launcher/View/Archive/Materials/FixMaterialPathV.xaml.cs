using Launcher.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Launcher.View {
    /// <summary>
    /// Логика взаимодействия для FixMaterialPath.xaml
    /// </summary>
    public partial class FixMaterialPathV : Window, IDisposable {
        public FixMaterialPathV(MaterialVM vm) {
            DataContext = vm;
            InitializeComponent();
        }

        public void Dispose() {
           // throw new NotImplementedException();
        }

        private void ChangeDialogResultMethod() {
            this.DialogResult = true;
            this.Close();
        }

    }
}
