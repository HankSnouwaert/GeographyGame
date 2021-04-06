using System.Collections.Generic;

namespace WPM
{
    public interface IGlobeInfo
    {
        Dictionary<string, Landmark> CulturalLandmarks { get; set; }
        Dictionary<string, Landmark> CulturalLandmarksByName { get; set; }
        Dictionary<string, MappableObject> MappedObjects { get; set; }
    }
}