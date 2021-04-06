using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPM
{
    public class GlobeInfo : MonoBehaviour, IGlobeInfo
    {
        public Dictionary<string, MappableObject> MappedObjects { get; set; } = new Dictionary<string, MappableObject>();

        public Dictionary<string, Landmark> CulturalLandmarks { get; set; } = new Dictionary<string, Landmark>();

        public Dictionary<string, Landmark> CulturalLandmarksByName { get; set; } = new Dictionary<string, Landmark>();

    }
}
