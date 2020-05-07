using Newtonsoft.Json;
using System;
using System.IO;

namespace Launcher {
    public class DefaultSerializer : ISerializer {
        public void Serialize(string objectPath, object value) {
            CheckPath(objectPath);

            using (FileStream fs = new FileStream(objectPath, FileMode.OpenOrCreate))
            using (StreamWriter file = new StreamWriter(fs)) {
                JsonSerializer serializer = new JsonSerializer {
                    Formatting = Formatting.Indented,
                    TypeNameHandling = TypeNameHandling.Auto
                };
                serializer.Serialize(file, value);
            }
        }
        /// <summary>
        /// Оставляю проверку на тебя
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectPath">Например Users//username//Projects//ProjectName.json</param>
        /// <returns></returns>
        public T Deserialize<T>(string objectPath) {
            T obj;
            CheckPath(objectPath);
            ChekFile(objectPath);

            using (FileStream fs = new FileStream(objectPath, FileMode.Open))
            using (StreamReader file = new StreamReader(fs)) {
                JsonSerializer serializer = new JsonSerializer {
                    Formatting = Formatting.Indented,
                    TypeNameHandling = TypeNameHandling.Auto
                };

                obj = (T)serializer.Deserialize(file, typeof(T));
            }
            return obj;
        }
        public object Deserialize(string objectPath) {
            object obj;
            CheckPath(objectPath);
            ChekFile(objectPath);

            using (FileStream fs = new FileStream(objectPath, FileMode.Open))
            using (StreamReader file = new StreamReader(fs)) {
                string json = file.ReadToEnd();
                obj = JsonConvert.DeserializeObject(json, new JsonSerializerSettings {
                    Formatting = Formatting.Indented,
                    TypeNameHandling = TypeNameHandling.Auto,
                });
                return obj;
            }
        }

        private void ChekFile(string objectPath) {
            string dir = Path.GetDirectoryName(objectPath);
            string[] filePaths = Directory.GetFiles(dir);
            if (filePaths.Length == 0) { throw new FileNotFoundException("Поместите ваши файлы по этому адресу: " + objectPath); }
        }

        public void CheckPath(string objectPath) {
            // Проверка для записи
            string dir = Path.GetDirectoryName(objectPath);
            if (!Directory.Exists(dir)) { Directory.CreateDirectory(dir); }
        }


    }
}
