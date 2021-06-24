namespace WPM
{
    /// <summary> 
    /// Used to determine what destination a tourist wants to be taken too
    /// </summary>
    public interface IDestinationSetter
    {
        /// <summary> 
        /// Sets the destination of a given tourist
        /// </summary>
        /// <param name="tourist"> The tourist whose destination is being set</param>
        /// <param name="touristRegion"> The tourist region the destination is being picked from</param>
        bool SetDestination(IInventoryTourist tourist, TouristRegion touristRegion);
    }
}