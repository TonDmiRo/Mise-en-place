namespace Launcher {
    public interface ISaveCollection {
        void SetSerializer(ISerializer serializer);
        void SerializeCollection(string collectionOwner);
        void SerializeCollection(string collectionOwner, ISerializer serializer);
    }
}
