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

        /// <summary> 
        /// The visual outline of the landmark's model
        /// </summary>
        Outline Outline { get; set; }
    }
}