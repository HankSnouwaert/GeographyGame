using System.Collections.Generic;

namespace WPM
{
    /// <summary>
    /// UI that displays information relating to the player's current location
    /// </summary>
    public interface INavigationUI
    {
        /// <summary>
        ///  The text describing the player's current location
        /// </summary>
        string NavigationText { get; set; }

        /// <summary>
        /// Sets the text displayed by this UI
        /// </summary>
        void SetDisplayText(string displayString);

        /// <summary>
        /// Update's the UI with information relating to the player's current position
        /// </summary>
        /// <param name="provinces"> The provinces that the player is currently located in </param>
        /// <param name="countries"> The countries that the player is currently located in </param>
        /// <param name="nearbyObjects"> The mappable objects that are currently within range of the player </param>
        void UpdateNavigationDisplay(List<Province> provinces, List<Country> countries, List<IMappableObject> nearbyObjects);
    }
}