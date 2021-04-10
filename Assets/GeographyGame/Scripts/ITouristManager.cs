using System.Collections.Generic;

namespace WPM
{
    public interface ITouristManager
    {
        TouristRegion CurrentRegion { get; set; }
        List<int> RecentProvinceDestinations { get; set; }
        List<string> RecentLandmarkDestinations { get; set; }
        List<int> RecentCountryDestinations { get; set; }
        int TouristSpawnRate { get; set; }
        int TrackingTime { get; set; }
        void GenerateTourist();
        void NextTurn(int turns);
    }
}