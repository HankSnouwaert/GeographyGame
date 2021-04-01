using System.Collections.Generic;

namespace WPM
{
    public interface IGlobeParser
    {
        List<int>[] GetCellsInRange(int startCell, int range = 0);
        List<int> GetCountriesFromProvinces(List<int> provinceIndexes);
        List<int> GetCountriesInCell(int cellIndex);
        List<int>[] GetCountriesInRange(int startCell, List<int>[] cellRange);
        List<Landmark> GetLandmarksInCell(int cellIndex);
        List<Landmark>[] GetLandmarksInRange(int startCell, List<int>[] cellRange);
        List<int> GetProvicesInCell(int cellIndex);
        List<int>[] GetProvincesInRange(int startCell, List<int>[] cellRange);
    }
}