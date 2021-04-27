using System.Collections.Generic;
using UnityEngine;

namespace WPM
{
    /// <summary> 
    /// Base interface for any object located on the world map globe
    /// </summary>
    public interface IMappableObject : ISelectableObject
    {
        /// <summary> 
        /// Update the objects map location given a new cell
        /// </summary>
        /// <param name="newCellIndex"> The index of the cell where 
        /// the object is now located</param>
        void UpdateLocation(int newCellIndex);
        
        /// <summary> 
        /// Cell of the world map globe that this object is centered on
        /// </summary>
        Cell CellLocation { get; set; }
        
        /// <summary> 
        /// The indec of the cell of the world map globe that this object is centered on
        /// </summary>
        int CellIndex { get; set; }

        /// <summary> 
        /// Vector coordinate of the world map globe that this object is centered on
        /// </summary>
        Vector3 VectorLocation { get; set; }

        /// <summary> 
        /// Latitude/Longitude coordinates of the world map globe that this object is centered on
        /// </summary>
        Vector2[] Latlon { get; set; }

        /// <summary> 
        /// Countries this object occupies
        /// </summary>
        List<Country> CountriesOccupied { get; set; }

        /// <summary> 
        /// Pronvices this object occupies
        /// </summary>
        List<Province> ProvincesOccupied { get; set; }

        /// <summary> 
        /// Political provinces this object occupies
        /// </summary>
        List<string> PoliticalProvincesOccupied { get; set; }

        /// <summary> 
        /// Climates this object occupies
        /// </summary>
        List<string> ClimatesOccupied { get; set; }
    }
}