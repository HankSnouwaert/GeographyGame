namespace WPM
{
    /// <summary> 
    /// Holds information and functionality relating to landmarks
    /// </summary>
    public interface ILandmarkManager
    {
        /// <summary> 
        /// Instantiates the cultural landmark for a given mount point
        /// </summary>
        /// <param name="mountPoint"> The mount point having its cultural landmark instantiated</param>
        bool InstantiateCulturalLandmark(MountPoint mountPoint);
    }
}