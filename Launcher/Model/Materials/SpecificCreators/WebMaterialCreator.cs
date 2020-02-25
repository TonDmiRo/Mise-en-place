using Launcher.Model.SpecificMaterials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher.Model {
    public class WebMaterialCreator : MaterialCreator {
        protected override Material Create(string title, string path) {
            if (CanCreate(title, path)) {
                return new WebMaterial(title, path);
            }
            throw new ArgumentException("Сannot create material");
        }

        private bool CanCreate(string title, string path) {
            //TODO: реализовать проверка на отклик сайта
#warning нет проверок
            return true;
        }
    }
}
