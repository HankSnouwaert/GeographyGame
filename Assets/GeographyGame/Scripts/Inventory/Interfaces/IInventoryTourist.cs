namespace WPM
{
    /// <summary> 
    /// Tourist that appears in the players inventory
    /// </summary>
    public interface IInventoryTourist : IInventoryItem
    {
        /// <summary> 
        /// Name of the tourist desired destination
        /// </summary>
        string DestinationName { get; set; }
        
        /// <summary> 
        /// The province the tourist wants to be taken too.  Null if the tourist's destination is not a province
        /// </summary>
        Province ProvinceDestination { get; set; }
        
        /// <summary> 
        /// The landmark the tourist wants to be taken too.  Null if the tourist's destination is not a landmark
        /// </summary>
        Landmark LandmarkDestination { get; set; }
        
        /// <summary> 
        /// The country the tourist wants to be taken too.  Null if the tourist's destination is not a country
        /// </summary>
        Country CountryDestination { get; set; }
        
        /// <summary> 
        /// The type of destination the tourist wants to be taken too
        /// </summary>
        DestinationType DestinationType { get; set; }
        
        /// <summary> 
        /// Attempt to drop the tourist off at the player's location
        /// </summary>
        void AttemptDropOff();

        /// <summary> 
        /// Cause the tourist's pop up U.I. to display
        /// </summary>
        /// <param name="persistant"> Whether the pop up stays up if the player interacts with other objects</param>
        void SetPopUpRequest(bool persistant);
    }
}