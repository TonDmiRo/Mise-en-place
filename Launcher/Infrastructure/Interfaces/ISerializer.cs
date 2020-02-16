namespace Launcher {
    public interface ISerializer {
        void Serialize(string FileName, object value);
        T Deserialize<T>(string FileName);
        object Deserialize(string FileName);
        string GetPath(string FileName);
    }
}
