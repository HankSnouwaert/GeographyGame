using System.Collections.Generic;

namespace WPM
{
    public interface ILandmarkParser
    {
        List<Landmark> GetLandmarksInCell(int cellIndex);
        List<Landmark>[] GetLandmarksInRange(int startCell, List<int>[] cellRange);
    }
}