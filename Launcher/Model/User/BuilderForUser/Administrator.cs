namespace Launcher.Model.BuilderForUser {
    internal class Administrator {
        private UserBuilder builder;
        public Administrator(UserBuilder builder) {
            this.builder = builder;
        }
        public void Construct() {
            builder.SetUserInformation();
            builder.PrepareCustomFiles();
            builder.SetProjects();
        }
    }
}
