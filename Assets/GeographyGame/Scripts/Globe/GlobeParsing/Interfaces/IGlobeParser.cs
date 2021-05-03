using System.Collections.Generic;

namespace WPM
{
    public interface IGlobeParser
    {
        IProvinceParser ProvinceParser { get; }
        ICountryParser CountryParser { get; }
        ILandmarkParser LandmarkParser { get; }
        List<Cell>[] GetCellsInRange(Cell startCell, int range = 0);
    }
}