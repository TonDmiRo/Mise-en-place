using Launcher.Model.SpecificMaterials;
using System;

namespace Launcher.Model {
    public class WebMaterialCreator : MaterialCreator {
        public static bool PathIsValid(string PathToMaterial) {
            if (string.IsNullOrWhiteSpace(PathToMaterial)) { return false; }
            //TODO: реализовать проверки
            return true;
        }
        protected override Material Create(string title, string path) {
            if (CanCreate(title, path)) {
                return new WebMaterial(title, path);
            }
            throw new ArgumentException("Сannot create material");
        }

        private bool CanCreate(string title, string path) {
            CheckWhiteSpace(title, path);
            //TODO: реализовать проверки
            return true;
        }
    }
}
