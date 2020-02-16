using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher.Model.SpecificMaterials {
    public class WebMaterial : Material {
        public WebMaterial(string name, string path) : base(name, path) {
        }

        protected override void StartMaterial() {
            CheckMaterial();
            System.Diagnostics.Process.Start(PathToMaterial);
          
        }

        private void CheckMaterial() {
            //TODO: Сделать проверки с блокировкой как в локальной материале
            //throw new NotImplementedException();
        }
    }
}
