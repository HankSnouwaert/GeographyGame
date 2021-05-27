using System.Collections.Generic;

namespace WPM
{
    /// <summary>
    /// Responsible for pathfinding across world map globe
    /// </summary>
    public interface IPathfinder
    {
        /// <summary>
        /// Cells that can be checked when pathfinding
        /// </summary>
        List<Cell>[] CellsInRange { get; set; }

        /// <summary>
        /// Cell indicies of the current path
        /// </summary>
        List<int> PathIndices { get; set; }

        /// <summary>
        /// The max range for pathfinding
        /// </summary>
        int TravelRange { get; }

        /// <summary>
        /// Clears the travel costs of all current CellsInRange
        /// </summary>
        void ClearCellCosts();

        /// <summary>
        /// Finds a path between startCellIndex and endCellIndex
        /// </summary>
        /// <param name="startCellIndex">Start cell index.</param>
        /// <param name="endCellIndex">End cell index.</param>
        ///<returns> The cell indicies of the path that was found </returns>
        List<int> FindPath(int startCellIndex, int endCellIndex);

        /// <summary>
        /// Colors a given path of cells
        /// </summary>
        /// <param name="cellIndices"> Cell indices of the path.</param>
        /// <param name="startCellIndex"> Starting cell of the path.</param>
        ///<returns> The cell indicies of the path that was found </returns>
        void ColorPath(List<int> cellIndices, int startCellIndex);

        /// <summary>
        /// Sets the travel costs of all current CellsInRange
        /// </summary>
        void SetCellCosts();

        /// <summary>
        /// Called when pathfinding is completed
        /// </summary>
        void FinishedPathFinding();
    }
}