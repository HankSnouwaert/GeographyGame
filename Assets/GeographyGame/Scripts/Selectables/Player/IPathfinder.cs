using System.Collections.Generic;

namespace WPM
{
    public interface IPathfinder
    {
        List<Cell>[] CellsInRange { get; set; }

        List<int> PathIndices { get; set; }

        IPlayerCharacter PlayerCharacter { get; }

        int TravelRange { get; }

        void ClearCellCosts();

        /// <summary>
        /// Draws a path between startCellIndex and endCellIndex
        /// </summary>
        /// <returns><c>true</c>, if path was found and drawn, <c>false</c> otherwise.</returns>
        /// <param name="startCellIndex">Start cell index.</param>
        /// <param name="endCellIndex">End cell index.</param>
        List<int> FindPath(int startCellIndex, int endCellIndex);

        void SetCellCosts();

        void FinishedPathFinding();
    }
}