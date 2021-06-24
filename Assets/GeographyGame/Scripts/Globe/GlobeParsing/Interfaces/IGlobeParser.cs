using System.Collections.Generic;

namespace WPM
{
    /// <summary> 
    /// Interface used to hold other parsing interfaces and to get cells from globe
    /// </summary>
    public interface IGlobeParser
    {
        /// <summary> 
        /// Interface used to parse provinces from globe
        /// </summary>
        IProvinceParser ProvinceParser { get; }
        
        /// <summary> 
        /// Interface used to parse countries from globe
        /// </summary>
        ICountryParser CountryParser { get; }

        /// <summary> 
        /// Interface used to parse landmarks from globe
        /// </summary>
        ILandmarkParser LandmarkParser { get; }

        /// <summary> 
        /// Gets all cells within range of a given cell
        /// </summary>
        /// <param name="startCell"> The cell used as a point of reference to get all other cells</param>
        /// <param name="range"> The range out from the start cell to parse other cells </param>
        /// <returns>  An array of lists, with List0 containing all cells within range and ListX containing 
        /// the cells X number of cells away from the target cell </returns>
        List<Cell>[] GetCellsInRange(Cell startCell, int range = 0);
    }
}