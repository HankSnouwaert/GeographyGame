using System.Collections.Generic;

namespace WPM
{
    public interface IProvinceParser
    {
        List<int> GetProvicesInCell(int cellIndex);
        List<int>[] GetProvincesInRange(int startCell, List<int>[] cellRange);
    }
}