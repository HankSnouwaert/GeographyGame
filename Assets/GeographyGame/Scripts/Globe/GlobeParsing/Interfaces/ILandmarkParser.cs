using System.Collections.Generic;

namespace WPM
{
    public interface ILandmarkParser
    {
        List<Landmark> GetLandmarksInCell(Cell cell);
        List<Landmark>[] GetLandmarksInRange(Cell startCell, List<Cell>[] cellRange);
    }
}