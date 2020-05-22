using System;
using System.IO;

namespace Launcher.Model.SpecificMaterials {
    internal class LocalMaterial : Material {
        public override bool ChangePathToMaterial(string newPath) {
            if (LocalMaterialCreator.PathIsValid(newPath)) {
                PathToMaterial = newPath;
                return true;
            }
            return false;
        }
        protected override void StartMaterial() {
            System.Diagnostics.Process.Start(PathToMaterial);
        }

        protected override void CheckMaterial() {
            base.CheckMaterial();
            if (PathToMaterial.IndexOfAny(Path.GetInvalidPathChars()) >= 0) {
                BlockMaterial();
                throw new ArgumentException($"Путь:{PathToMaterial} учебного материала:{MaterialTitle} содержит недопустимые символы.");
            }

            if (( !Path.HasExtension(PathToMaterial) ) && ( !Directory.Exists(PathToMaterial) )) {
                BlockMaterial();
                throw new DirectoryNotFoundException($"Путь:{PathToMaterial} учебного материала:{MaterialTitle} не найден.");
            }

            // 2. Если это файл проверить его.
            if (( Path.HasExtension(PathToMaterial) ) && ( !File.Exists(PathToMaterial) )) {
                BlockMaterial();
                throw new FileNotFoundException("Файл не найден!");
            }
        }

        public LocalMaterial(string name, string path) : base(name, path) {
        }
    }
}
