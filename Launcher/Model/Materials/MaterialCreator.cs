﻿using System;
using System.IO;

namespace Launcher.Model {
    public abstract class MaterialCreator {
        public Material CreateMaterial(string title, string path) {
                return Create(title, path);
        }
        protected abstract Material Create(string title, string path);
        protected void CheckWhiteSpace(string title, string path) {
            bool titleAndPathNotIsNull = ( !string.IsNullOrWhiteSpace(title) ) && ( !string.IsNullOrWhiteSpace(path) );
            if (!titleAndPathNotIsNull) {
                throw new ArgumentException($"Title:{title} or Path:{path} is null or white space!");
            }
        }
    }
}
