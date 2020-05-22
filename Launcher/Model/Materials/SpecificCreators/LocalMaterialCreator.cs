using Launcher.Model.SpecificMaterials;
using System;
using System.IO;

namespace Launcher.Model {
    internal class LocalMaterialCreator : MaterialCreator {
        public static bool PathIsValid(string PathToMaterial) {
            if (string.IsNullOrWhiteSpace(PathToMaterial)) { return false; }
            if (PathToMaterial.IndexOfAny(Path.GetInvalidPathChars()) >= 0) { return false; }
            if (( !Path.HasExtension(PathToMaterial) && ( !Directory.Exists(PathToMaterial) ) )) { return false; }
            if (( Path.HasExtension(PathToMaterial) ) && ( !File.Exists(PathToMaterial) )) { return false; }
            return true;
        }

        protected override Material Create(string title, string path) {
            if (CanCreate(title, path)) {
                return new LocalMaterial(title, path);
            }
            throw new ArgumentException("Сannot create material");
        }
        private bool CanCreate(string title, string path) {
            CheckWhiteSpace(title, path);

            bool absentInvalidCharacters = ( title + path ).IndexOfAny(Path.GetInvalidPathChars()) == -1;
            if (!absentInvalidCharacters) {
                throw new ArgumentException($"Title:{title} or Path:{path} contains invalid characters!");
            }

            if (!Directory.Exists(Path.GetDirectoryName(path))) {
                throw new DirectoryNotFoundException($"The directory of this Path:{path} does not exist");
            }
            return true;
        }
    }


}
