using System.Collections.Generic;

namespace WPM
{
    /// <summary> 
    /// Interface attached to objects that can move around the world map globe
    /// It is not responsible for pathfinding
    /// </summary>
    public interface IGeoPosAnimator
    {
        /// <summary> 
        /// Used by the animator to determine if the object should continue making movement
        /// progress towards its destination
        /// </summary>
        bool Auto { get; set; }

        /// <summary> 
        /// Flag indicating that the object should stop moving
        /// </summary>
        bool Stop { get; set; }

        /// <summary> 
        /// Flag indicating whether the object is moving
        /// </summary>
        bool Moving { get; set; } 

        /// <summary> 
        /// Determines the path the object will take while moving, given the existing latlan list
        /// </summary>
        void ComputePath();
        
        /// <summary> 
        /// Generates a list of internal Latlon points corresponding to a given set of path indices
        /// </summary>
        /// <param name="pathIndices"> The path being used to generate the Latlon list</param>
        void GenerateLatLon(List<int> pathIndices);

        /// <summary> 
        /// Moves the game object to a point between the current two path indices
        /// </summary>
        /// <param name="progress"> A percentile input used determine what point between the two 
        /// indices the game object should be moved too</param>
        void MoveTo(float progress);
    }
}