using System.Collections.Generic;

namespace WPM
{
    /// <summary> 
    /// Used to parse landmarks from the world map globe
    /// </summary>
    public interface ILandmarkParser
    {
        /// <summary> 
        /// Gets all landmarks in a given cell
        /// </summary>
        /// <param name="cell"> The cell being parsed for landmarks </param>
        /// <returns> A list of landmarks located in the given cell</returns>
        List<Landmark> GetLandmarksInCell(Cell cell);

        /// <summary> 
        /// Gets all the landmarks within a given range of a given cell
        /// </summary>
        /// <param name="startCell"> The center cell from which the range is measured </param>
        /// <param name="range"> The range out from the center cell from which landmarks are parsed </param>
        /// <returns>  An array of lists, with ListX containing the landmarks reachable within X number 
        /// of cells away from the target cell</returns>
        List<Landmark>[] GetLandmarksInRange(Cell startCell, int range);
    }
}