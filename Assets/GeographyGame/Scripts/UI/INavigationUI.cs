using System.Collections.Generic;

namespace WPM
{
    public interface INavigationUI
    {
        string NavigationText { get; set; }

        string CreateNavigationInfoString(List<Province> provinces, List<Country> countries, List<MappableObject> nearbyObjects);
        void SetDisplayText(string displayString);
        void UpdateNavigationDisplay(List<Province> provinces, List<Country> countries, List<MappableObject> nearbyObjects);
    }
}