using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ZooWorld.Settings
{
    [CreateAssetMenu(fileName = "AnimalCatalogSettings", menuName = "ZooWorld/Settings/Animal Catalog")]
    public sealed class AnimalSettingsTable : ScriptableObject
    {
        [field: SerializeField]
        [BoxGroup("Catalog")]
        public List<AnimalSettings> Definitions { get; private set; }
    }
}