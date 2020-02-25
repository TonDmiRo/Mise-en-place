using System;
using System.IO;

namespace Launcher.Model {
    public abstract class MaterialCreator {
        public Material CreateMaterial(string title, string path) {
            if (CanCreate(title, path)) {
                return Create(title, path);
            }
            throw new ArgumentException("Сannot create material");
        }

        protected abstract Material Create(string title, string path);

        private bool CanCreate(string title, string path) {
            //TODO: Надо?
            bool titleAndPathNotIsNull = ( !string.IsNullOrWhiteSpace(title) ) && ( !string.IsNullOrWhiteSpace(path) );
            if (!titleAndPathNotIsNull) {
                throw new ArgumentException($"Title:{title} or Path:{path} is null or white space!");
            }
            return true;
        }
    }
}
