using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ZooWorld.Settings
{
    [CreateAssetMenu(fileName = "AnimalSettingsTable", menuName = "ZooWorld/Settings/AnimalSettingsTable")]
    public sealed class AnimalSettingsTable : ScriptableObject
    {
        [field: SerializeField]
        public List<AnimalSettings> Animals { get; private set; }
    }
}