using System.Collections.Generic;

namespace WPM
{
    public interface ICountryParser
    {
        List<Country> GetCountriesFromProvinces(List<Province> provinces);
        List<Country> GetCountriesInCell(Cell cell);
        List<Country>[] GetCountriesInRange(Cell startCell, List<Cell>[] cellRange);
    }
}