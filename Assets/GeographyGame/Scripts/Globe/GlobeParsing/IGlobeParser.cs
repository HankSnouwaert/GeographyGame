using System.Collections.Generic;

namespace WPM
{
    public interface IGlobeParser
    {
        IProvinceParser ProvinceParser { get; set; }
        ICountryParser CountryParser { get; set; }
        ILandmarkParser LandmarkParser { get; set; }
        List<int>[] GetCellsInRange(int startCell, int range = 0);
    }
}