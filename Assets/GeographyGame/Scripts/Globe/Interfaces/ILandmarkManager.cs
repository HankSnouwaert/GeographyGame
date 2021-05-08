using System.Collections.Generic;

namespace WPM
{
    /// <summary> 
    /// Holds information and functionality relating to landmarks
    /// </summary>
    public interface ILandmarkManager
    {
        /// <summary> 
        /// Contains all cultural landmarks, referenced by their string ID
        /// </summary>
        Dictionary<string, Landmark> CulturalLandmarks { get; set; }

        /// <summary> 
        /// Contains all cultural landmarks, reference by their name
        /// </summary>
        Dictionary<string, Landmark> CulturalLandmarksByName { get; set; }

        /// <summary> 
        /// Instantiates the cultural landmark for a given mount point
        /// </summary>
        /// <param name="mountPoint"> The mount point having its cultural landmark instantiated</param>
        bool InstantiateCulturalLandmark(MountPoint mountPoint);
    }
}