using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Customization
{
    [Serializable]
    public class CustomizableEntityItem
    {
        public string PointKey;
        public GameObject GameObject;
        public Component Component;
        public List<Material> Materials = new();

        public Material MainMaterial => Materials.FirstOrDefault();
    }

    public interface ICustomizableEntityProvider
    {
        IReadOnlyList<CustomizableEntityItem> Items { get; }
    }

    public class CustomizableEntityProvider : MonoBehaviour, ICustomizableEntityProvider
    {
        [SerializeField]
        private List<CustomizableEntityItem> _items;

        public IReadOnlyList<CustomizableEntityItem> Items => _items;
    }
}