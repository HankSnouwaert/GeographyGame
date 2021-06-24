namespace WPM
{
    /// <summary> 
    /// Interface responsible for checking if a tourist is being dropped off in a correct map location
    /// </summary>
    public interface ITouristDropper
    {
        /// <summary> 
        /// Check if a given location is a correct drop off location given a desired destination
        /// </summary>
        /// <param name="dropOffCell"> The drop off cell</param>
        /// <param name="destinationType"> The desired destination type </param>
        /// <param name="provinceDestination"> The desired province destination.  Null if the destination is not a province </param>
        /// <param name="landmarkDestination"> The desired landmark destination.  Null if the destination is not a landmark </param>
        ///<param name="countryDestination"> The desired country destination.  Null if the destination is not a country </param>
        bool AttemptDropOff(Cell dropOffCell, DestinationType destinationType, Province provinceDestination, Landmark landmarkDestination, Country countryDestination);
    }
}