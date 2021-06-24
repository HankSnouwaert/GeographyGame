using System.Collections.Generic;

namespace WPM
{
    /// <summary> 
    /// Used to instantiante and track information related to tourist generation
    /// </summary>
    public interface ITouristManager
    {
        /// <summary> 
        /// The tourist region tourist destinations are currently being picked from
        /// </summary>
        TouristRegion CurrentRegion { get; }
        
        /// <summary> 
        /// Provinces destinations that have recently been requested by tourists
        /// </summary>
        List<int> RecentProvinceDestinations { get; set; }

        /// <summary> 
        /// Landmark destinations that have recently been requested by tourists
        /// </summary>
        List<string> RecentLandmarkDestinations { get; set; }

        /// <summary> 
        /// Country destinations that have recently been requested by tourists
        /// </summary>
        List<int> RecentCountryDestinations { get; set; }

        /// <summary> 
        /// The number of turns between tourist instantiations
        /// </summary>
        int TouristSpawnRate { get; }

        /// <summary> 
        /// The number of rounds a requested tourist destination is remembered
        /// </summary>
        int TrackingTime { get; }

        /// <summary> 
        /// Generate a new tourist
        /// </summary>
        void GenerateTourist();
    }
}