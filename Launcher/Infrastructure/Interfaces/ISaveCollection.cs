namespace Launcher {
    public interface ISaveCollection {
        // TODO: DELETE
        void SetSerializer(ISerializer serializer);
        void SerializeCollection(string collectionOwner);
        void SerializeCollection(string collectionOwner, ISerializer serializer);
    }
}
