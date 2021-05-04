using System.Collections.Generic;

namespace WPM
{
    /// <summary> 
    /// Used to parse countries from the world map globe
    /// </summary>
    public interface ICountryParser
    {
        /// <summary> 
        /// Gets all the coutries represented within a group of given provinces
        /// </summary>
        /// <param name="provinces"> The provinces from which the countries are being found </param>
        /// <returns> The countries represented amoungst the given provinces </returns>
        List<Country> GetCountriesFromProvinces(List<Province> provinces);

        /// <summary> 
        /// Gets all the countries with territory in a given cell
        /// </summary>
        /// <param name="cell"> The cell being parsed for countries </param>
        /// <returns> A list of contries with territory that overlaps with the given cell</returns>
        List<Country> GetCountriesInCell(Cell cell);

        /// <summary> 
        /// Gets all the countries within a given range of a given cell
        /// </summary>
        /// <param name="startCell"> The center cell from which the range is measured </param>
        /// <param name="range"> The range out from the center cell from which countries are parsed </param>
        /// <returns>  An array of lists, with ListX containing the countries reachable within X number 
        /// of cells away from the target cell</returns>
        List<Country>[] GetCountriesInRange(Cell startCell, int range);
    }
}