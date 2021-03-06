﻿namespace WPM
{
    /// <summary> 
    /// Interface used to access extended functionality for interacting with world map globe
    /// </summary>
    public interface IGlobeManager
    {
        /// <summary> 
        /// The game's world map globe
        /// </summary>
        WorldMapGlobe WorldMapGlobe { get; }
        /// <summary> 
        /// Contains funcitonality for the cursor interacting with map cells
        /// </summary>
        ICellCursorInterface CellCursorInterface { get; }
        /// <summary> 
        /// Used to initialize the globe when the scen starts
        /// </summary>
        IGlobeEditor GlobeEditor { get; }
        /// <summary> 
        /// Used to retreive specific information from specific locations on the globe
        /// </summary>
        IGlobeParser GlobeParser { get; }

        /// <summary> 
        /// Used to store information and functionality related to objects on the globe
        /// </summary>
        IMappablesManager MappablesManager { get; }
    }
}