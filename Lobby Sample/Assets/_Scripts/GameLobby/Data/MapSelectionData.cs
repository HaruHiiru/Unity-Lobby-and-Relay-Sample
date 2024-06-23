using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.Core.Data
{
    [CreateAssetMenu(menuName = "Data/ModeSelectionData", fileName = "ModeSelectionData")]

    public class ModeSelectionData : ScriptableObject
    {
        public List<MapInfo> Maps;
    }

}

[Serializable]
public struct MapInfo
{
    public String mode;
    public String sceneName;
}