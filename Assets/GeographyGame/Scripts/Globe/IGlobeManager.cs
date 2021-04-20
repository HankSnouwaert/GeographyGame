namespace WPM
{
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
        IGlobeInitializer GlobeInitializer { get; }
        /// <summary> 
        /// Contains information about the globe and its contents
        /// </summary>
        IGlobeInfo GlobeInfo { get; }
        /// <summary> 
        /// Used to retreive specific information from specific locations on the globe
        /// </summary>
        IGlobeParser GlobeParser { get; }
    }
}