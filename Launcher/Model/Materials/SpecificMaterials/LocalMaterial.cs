using System.IO;

namespace Launcher.Model.SpecificMaterials {
    internal class LocalMaterial : Material {
        public LocalMaterial(string name, string path) : base(name, path) {

        }

        protected override void StartMaterial() {
            CheckMaterial();
            System.Diagnostics.Process.Start(PathToMaterial);
        }

        private void CheckMaterial() {
            // 1. Проверить путь;
            if (!IsPathValid()) {
                BlockMaterial();
                throw new DirectoryNotFoundException($"Путь:{PathToMaterial} учебного материала:{MaterialTitle} не найден.");
            }
            // 2. Если это файл проверить его.
            if (( Path.HasExtension(PathToMaterial) ) && ( !File.Exists(PathToMaterial) )) {
                BlockMaterial();
                throw new FileNotFoundException("Файл не найден!");
            }
        }
        private bool IsPathValid() {
            bool NotIsNullOrEmpty = !string.IsNullOrWhiteSpace(PathToMaterial);
            bool absentInvalidCharacters = PathToMaterial.IndexOfAny(Path.GetInvalidPathChars()) == -1;

            if (NotIsNullOrEmpty && absentInvalidCharacters) {
                string directory = Path.GetDirectoryName(PathToMaterial);
                if (Directory.Exists(directory)) {
                    return true;
                }
            }
            return false;
        }
    }
}
