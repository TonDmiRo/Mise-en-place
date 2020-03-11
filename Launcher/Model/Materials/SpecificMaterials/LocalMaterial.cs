using System;
using System.IO;

namespace Launcher.Model.SpecificMaterials {
    internal class LocalMaterial : Material {
        public LocalMaterial(string name, string path) : base(name, path) {
        }
        protected override void CheckMaterial() {
            base.CheckMaterial();
            if (PathToMaterial.IndexOfAny(Path.GetInvalidPathChars()) >= 0) {
                BlockMaterial();
                throw new ArgumentException($"Путь:{PathToMaterial} учебного материала:{MaterialTitle} содержит недопустимые символы.");
            }
            string directory = Path.GetDirectoryName(PathToMaterial);
            if (!Directory.Exists(directory)) {
                BlockMaterial();
                throw new DirectoryNotFoundException($"Путь:{PathToMaterial} учебного материала:{MaterialTitle} не найден.");
            }

            // 2. Если это файл проверить его.
            if (( Path.HasExtension(PathToMaterial) ) && ( !File.Exists(PathToMaterial) )) {
                BlockMaterial();
                throw new FileNotFoundException("Файл не найден!");
            }
        }

        protected override void StartMaterial() {
            System.Diagnostics.Process.Start(PathToMaterial);
        }
    }
}
