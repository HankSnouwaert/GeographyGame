namespace WPM
{
    /// <summary> 
    /// A landmark found on the world map globe
    /// </summary>
    public interface ILandmark : IMappableObject
    {
        /// <summary> 
        /// The mount point designating this landmarks location on the world map globe
        /// </summary>
        MountPoint MountPoint { get; set; }
    }
}