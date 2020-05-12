using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Launcher.Model {
    public class ProjectMaterials : ReadOnlyObservableCollection<Material> {
        public ProjectMaterials(ObservableCollection<Material> materials) : base(materials) {
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
        public void OpenMarkedMaterials() {
            if (MarkedMaterialsExist()) {
                foreach (var item in _projectMaterials) {
                    if (item.OpensAtLaunch) {
                        try { item.OpenMaterial(); }
                        catch (Exception) {
                            item.BlockMaterial();
                            // будет слишком много исключений если их перебрасывать throw e;
                            // TODO: если Exists false необходимо зафиксировать причину. string причина?
                            // блокировка без дальнейших инструкций для пользователя
                        }
                    }
                }
            }
        }
        public bool MarkedMaterialsExist() {
            bool containsOneMarkedM = false;
            foreach (var item in _projectMaterials) {
                containsOneMarkedM = containsOneMarkedM | item.OpensAtLaunch;
                if (containsOneMarkedM) { break; }
            }
            return containsOneMarkedM;
        }

        public IList<string> GetTitleOfDamagedMaterials() {
            List<string> names = new List<string>();
            foreach (var item in _projectMaterials) {
                if (item.Exists == false) {
                    item.BlockMaterial();
                    names.Add(item.MaterialTitle);
                }
            }
            return names;
        }

        private readonly ObservableCollection<Material> _projectMaterials;
    }
}
