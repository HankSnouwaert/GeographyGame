using System.Collections.Generic;

namespace WPM
{
    public interface ICountryParser
    {
        List<int> GetCountriesFromProvinces(List<int> provinceIndexes);
        List<int> GetCountriesInCell(int cellIndex);
        List<int>[] GetCountriesInRange(int startCell, List<int>[] cellRange);
    }
}