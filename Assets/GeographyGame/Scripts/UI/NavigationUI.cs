using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WPM
{
    public class NavigationUI : UIElement, INavigationUI
    {
        //Public Variables
        public string NavigationText { get; set; }
        //Internal Reference Interfaces
        IPlayerManager playerManager;
        private WorldMapGlobe worldMapGlobe;
        //UI Elements
        private Text[] textComponents;
        private Text displayText;
        //Error Checking
        private bool componentMissing = false;
        
        protected override void Awake()
        {
            base.Awake();
            textComponents = UIObject.GetComponentsInChildren<Text>();
            if (textComponents == null || textComponents.Length > 1)
            {
                componentMissing = true;
            }
            else
            {
                displayText = textComponents[0];
            }
            UIOpen = true;
        }

        protected override void Start()
        {
            base.Start();
            if (gameObject.activeSelf)
            {
                if (componentMissing)
                    errorHandler.ReportError("Error getting Navigation UI Elements", ErrorState.close_window);

                worldMapGlobe = globeManager.WorldMapGlobe;
                if (worldMapGlobe == null)
                    errorHandler.ReportError("World Map Globe missing", ErrorState.restart_scene);

                playerManager = gameManager.PlayerManager;
                if (playerManager == null)
                    errorHandler.ReportError("Player Manager missing", ErrorState.restart_scene);
            }
        }

        public void UpdateNavigationDisplay(List<Province> provinces, List<Country> countries, List<IMappableObject> nearbyObjects)
        {
            NavigationText = CreateNavigationInfoString(provinces, countries, nearbyObjects);
            SetDisplayText(NavigationText);
        }

        private string CreateNavigationInfoString(List<Province> provinces, List<Country> countries, List<IMappableObject> nearbyObjects)
        {
            string createdString = "";
            createdString = AddCountriesToDisplayString(createdString, countries);
            createdString = AddProvincesToDisplayString(createdString, provinces);
            createdString = AddLandmarksToDisplayString(createdString, nearbyObjects);
            return createdString;
        }

        public void SetDisplayText(string displayString)
        {
            displayText.text = displayString;
        }

        private string AddCountriesToDisplayString(string displayString, List<Country> countries)
        {
            if (countries != null)
            {
                if (countries.Count > 0)
                {
                    if (countries.Count == 1)
                        displayString += "Country: ";
                    else
                        displayString += "Countries: ";
                    foreach (Country country in countries)
                    {
                        displayString += country.name + System.Environment.NewLine + ",";
                    }
                    displayString = displayString.TrimEnd(',');
                }
            }
            return displayString;
        }

        private string AddProvincesToDisplayString(string displayString, List<Province> provinces)
        {
            if (provinces != null)
            {
                if (provinces.Count > 0)
                {
                    string nameType;
                    string politicalProvince;
                    string climate;
                    Country country;
                    if (provinces.Count == 1)
                    {
                        try
                        {
                            politicalProvince = provinces[0].attrib["PoliticalProvince"];
                            climate = provinces[0].attrib["ClimateGroup"];
                            country = worldMapGlobe.countries[provinces[0].countryIndex];
                        }
                        catch
                        {
                            errorHandler.ReportError("Failed to retrieve province property", ErrorState.close_application);
                            return displayString;
                        }
                        if (country.name == "United States of America")
                            nameType = "State: ";
                        else
                            nameType = "Province: ";
                        displayString += nameType + provinces[0].name + System.Environment.NewLine;// + "PoliticalProvince: " + politicalProvince +
                            //System.Environment.NewLine + "Climate: " + climate;
                    }
                    else
                    {
                        country = worldMapGlobe.countries[provinces[0].countryIndex];
                        if (country.name == "United States of America")
                            nameType = "States: ";
                        else
                            nameType = "Provinces: ";
                        displayString += nameType;
                        foreach (Province province in provinces)
                        {
                            try
                            {
                                politicalProvince = provinces[0].attrib["PoliticalProvince"];
                                climate = provinces[0].attrib["ClimateGroup"];
                                country = worldMapGlobe.countries[provinces[0].countryIndex];
                            }
                            catch
                            {
                                errorHandler.ReportError("Failed to retrieve province attributes", ErrorState.close_application);
                                return displayString;
                            }
                            if (country.name == "United States of America")
                                nameType = "State: ";
                            else
                                nameType = "Province: ";
                            displayString += province.name + "," + System.Environment.NewLine;// + "PoliticalProvince: " + politicalProvince +
                            //System.Environment.NewLine + "Climate: " + climate + System.Environment.NewLine;
                        }
                        displayString = displayString.TrimEnd('\r', '\n');
                        displayString = displayString.TrimEnd(',');
                    }
                }
            }
            return displayString;
        }

        private string AddLandmarksToDisplayString(string displayString, List<IMappableObject> mappableObjects)
        {
            if (mappableObjects != null)
            {
                foreach (IMappableObject mappableObject in mappableObjects)
                {
                    if(mappableObject != playerManager.PlayerCharacter)
                        displayString = displayString + "Landmarks: " + mappableObject.ObjectName + System.Environment.NewLine;
                }
            }
            return displayString;
        }
    }
}
