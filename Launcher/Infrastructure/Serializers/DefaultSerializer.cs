using Newtonsoft.Json;
using System.IO;

namespace Launcher {
    public class DefaultSerializer : ISerializer {

        public void Serialize(string FileName, object value) {
            string path = GetPath(FileName);

            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            using (StreamWriter file = new StreamWriter(fs)) {
                JsonSerializer serializer = new JsonSerializer {
                    Formatting = Formatting.Indented,
                    TypeNameHandling = TypeNameHandling.Auto
                };
                serializer.Serialize(file, value);
            }
        }
        public T Deserialize<T>(string FileName) {
            T obj;
            string path = GetPath(FileName);

            using (FileStream fs = new FileStream(path, FileMode.Open))
            using (StreamReader file = new StreamReader(fs)) {
                JsonSerializer serializer = new JsonSerializer {
                    Formatting = Formatting.Indented,
                    TypeNameHandling = TypeNameHandling.Auto
                };

                obj = (T)serializer.Deserialize(file, typeof(T));
            }
            return obj;
        }
        public object Deserialize(string FileName) {
            object obj;
            string path = GetPath(FileName);

            using (FileStream fs = new FileStream(path, FileMode.Open))
            using (StreamReader file = new StreamReader(fs)) {
                string json = file.ReadToEnd();
                obj = JsonConvert.DeserializeObject(json, new JsonSerializerSettings {
                    Formatting = Formatting.Indented,
                    TypeNameHandling = TypeNameHandling.Auto,
                });
                return obj;
            }
        }

        public string GetPath(string fileName) {
            if (!Directory.Exists("Users")) {
                Directory.CreateDirectory("Users");
            }

            string path = Path.Combine("Users", ( Path.ChangeExtension(fileName, "json")));
            return path;
        }
    }
}
