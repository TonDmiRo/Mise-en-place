using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;

namespace Launcher.Model {
    public class ProjectMaterials: ReadOnlyObservableCollection<Material> {
        public ProjectMaterials(ObservableCollection<Material> materials) :base(materials) {
            _projectMaterials = materials;
        }
        public void Add(Material item) {
            _projectMaterials.Add(item);
        }
        public bool Remove(Material item) {
            return _projectMaterials.Remove(item);
        }
        public void SetMaterial(int indexSpoiledM, Material serviceableM) {
            _projectMaterials[indexSpoiledM] = serviceableM;
        }
        public bool OpenMarkedMaterials() {
            if (CheckForMarked()) {
                foreach (var item in _projectMaterials) {
                    if (item.OpensAtLaunch) {
                        try {
                            item.OpenMaterial();
                        }
                        catch (Exception) {
                            item.BlockMaterial();
                        }
                    }
                }
                return true;
            }
            return false;
        }
        private bool CheckForMarked() {
            bool containsOneMarkedM = false;
            foreach (var item in _projectMaterials) {
                containsOneMarkedM = containsOneMarkedM | item.OpensAtLaunch;
                if (containsOneMarkedM) { break; }
            }
            return containsOneMarkedM;
        }
       
        private readonly ObservableCollection<Material> _projectMaterials;
    }
}
