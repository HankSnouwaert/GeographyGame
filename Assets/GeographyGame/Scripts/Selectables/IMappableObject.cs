using System.Collections.Generic;
using UnityEngine;

namespace WPM
{
    public interface IMappableObject : ISelectableObject
    {
        void UpdateLocation(int newCellIndex);
        int CellLocation { get; set; }
        Vector3 VectorLocation { get; set; }
        Vector2[] Latlon { get; set; }
        List<Country> CountriesOccupied { get; set; } 
        List<Province> ProvincesOccupied { get; set; } 
        List<string> PoliticalProvincesOccupied { get; set; } 
        List<string> ClimatesOccupied { get; set; }
    }
}