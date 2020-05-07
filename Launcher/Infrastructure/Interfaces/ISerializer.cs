namespace Launcher {
    public interface ISerializer {
        //FileName => relative path
        void Serialize(string objectPath, object value);
        T Deserialize<T>(string objectPath);
        object Deserialize(string objectPath);
        void CheckPath(string objectPath);
    }
}
