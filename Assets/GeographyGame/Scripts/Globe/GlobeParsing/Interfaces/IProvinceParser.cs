using System.Collections.Generic;

namespace WPM
{
    public interface IProvinceParser
    {
        List<Province> GetProvicesInCell(Cell cell);
        List<Province>[] GetProvincesInRange(Cell startCell, List<Cell>[] cellRange);
    }
}