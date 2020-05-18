namespace Launcher.Model.BuilderForUser {
    internal abstract class UserBuilder {
        public abstract void SetUserInformation();
        public abstract void PrepareCustomFiles();
        public abstract void SetProjects();
        public abstract User GetUser();
    }
}
