using Launcher.Model.SpecificMaterials;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher.Model {
    class CreatorLocalMaterial : CreatorMaterial {
        protected override Material Create(string title, string path) {
            if (CanCreate(title, path)) {
                return new LocalMaterial(title, path);
            }
            throw new ArgumentException("Сannot create material");
        }
        private bool CanCreate(string title, string path) {

            bool titleAndPathNotIsNull = ( !string.IsNullOrWhiteSpace(title) ) && ( !string.IsNullOrWhiteSpace(path) );
            if (!titleAndPathNotIsNull) {
                //TODO: эта проверка есть в базовом классе 
                throw new ArgumentException($"Title:{title} or Path:{path} is null or white space!");
            }

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
