using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Launcher.Model {
    public class UsefulMaterialDictionary : ISaveCollection {

        public UsefulMaterialDictionary() {
            UsefulMaterials = new Dictionary<string, Material>();
        }

        private Dictionary<string, Material> UsefulMaterials {
            get {
                return _usefulMaterials;
            }
            set {
                _usefulMaterials = value;
                _values = new ObservableCollection<Material>(_usefulMaterials.Values);
                Values = new ReadOnlyObservableCollection<Material>(_values);
            }
        }
        private Dictionary<string, Material> _usefulMaterials;

        public ReadOnlyObservableCollection<Material> Values { get; private set; }
        private ObservableCollection<Material> _values;


        public int Count => UsefulMaterials.Count;
        public bool ContainsKey(string key) {
            return UsefulMaterials.ContainsKey(key);
        }
        public bool Add(Material material) {
            //Добавляются только новые материалы
            if (!ContainsKey(material.MaterialTitle)) {
                UsefulMaterials.Add(material.MaterialTitle, material);
                _values.Add(material);
                return true;
            }

            throw new ArgumentException("Элемент с таким ключом уже существует в словаре!");
        }
        public bool Remove(string key) {
            _values.Remove(UsefulMaterials[key]);
            return UsefulMaterials.Remove(key);
        }

        public bool OpenUsefulMaterial(string key) {
            if (ContainsKey(key)) {
                try {
                    UsefulMaterials[key].OpenMaterial();
                }
                catch (Exception e) {
                    UsefulMaterials[key].BlockMaterial();
                    throw e;
                }
                return true;
            }
            return false;
        }

        public bool OpenMarkedUsefulMaterials() {
            if (CheckForMarked()) {
                foreach (var item in _usefulMaterials.Values) {
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
            foreach (var item in _usefulMaterials) {
                containsOneMarkedM = containsOneMarkedM | item.Value.OpensAtLaunch;
                if (containsOneMarkedM) { break; }
            }
            return containsOneMarkedM;
        }

        #region IDictionary<TKey, TValue>
        private void Clear() {
            UsefulMaterials.Clear();
        }
        private bool TryGetValue(string key, out Material value) {
            return UsefulMaterials.TryGetValue(key, out value);
        }

        private bool Contains(KeyValuePair<string, Material> item) {
            return ( (IDictionary<string, Material>)UsefulMaterials ).Contains(item);
        }
        #endregion

        #region Serialize

        private ISerializer serializer;
        public void SetSerializer(ISerializer serializer) {
            this.serializer = serializer;
        }
        public void SerializeCollection(string collectionOwner) {
            serializer.Serialize(collectionOwner + "'s_usefulMaterials", UsefulMaterials);
        }
        public void SerializeCollection(string collectionOwner, ISerializer serializer) {
            SetSerializer(serializer);
            SerializeCollection(collectionOwner);
            //   serializer.Serialize(collectionOwner + "'s_usefulMaterials", UsefulMaterials);
        }
        #endregion

    }
}
