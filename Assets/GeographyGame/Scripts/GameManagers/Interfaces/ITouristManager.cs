using System.Collections.Generic;

namespace WPM
{
    public interface ITouristManager
    {
        TouristRegion CurrentRegion { get; }
        List<int> RecentProvinceDestinations { get; set; }
        List<string> RecentLandmarkDestinations { get; set; }
        List<int> RecentCountryDestinations { get; set; }
        int TouristSpawnRate { get; }
        int TrackingTime { get; }
        void GenerateTourist();
    }
}