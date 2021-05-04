using System.Collections.Generic;

namespace WPM
{
    /// <summary> 
    /// Used to parse provinces from the world map globe
    /// </summary>
    public interface IProvinceParser
    {
        /// <summary> 
        /// Gets all the provinces with territory in a given cell
        /// </summary>
        /// <param name="cell"> The cell being parsed fro provinces </param>
        /// <returns> A list of provinces with territory that overlaps with the given cell</returns>
        List<Province> GetProvicesInCell(Cell cell);

        /// <summary> 
        /// Gets all the provinces within a given range of a given cell
        /// </summary>
        /// <param name="startCell"> The center cell from which the range is measured </param>
        /// <param name="range"> The range out from the center cell from which provinces are parsed </param>
        /// <returns>  An array of lists, with ListX containing the provinces reachable within X number 
        /// of cells away from the target cell</returns>
        List<Province>[] GetProvincesInRange(Cell startCell, int range);
    }
}