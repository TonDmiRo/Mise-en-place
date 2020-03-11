namespace Launcher.Model.SpecificMaterials {
    public class WebMaterial : Material {
        public WebMaterial(string name, string path) : base(name, path) {
        }
        protected override void CheckMaterial() {
            base.CheckMaterial();
            //TODO: Сделать проверки с блокировкой как в локальной материале
            //throw new NotImplementedException();
        }

        protected override void StartMaterial() {
            CheckMaterial();
            System.Diagnostics.Process.Start(PathToMaterial);

        }
    }
}
