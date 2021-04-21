using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WPM
{
    public class NavigationUI : UIElement, INavigationUI, IDropOffUI
    {
        public string NavigationText { get; set; }
        private Button dropOffButton;
        private GameObject dropOffButtonObject;
        private Button[] buttonComponents;
        private Text[] textComponents;
        private Text displayText;
        private IGlobeManager globeManager;
        private WorldMapGlobe worldMapGlobe;
        // Start is called before the first frame update
        public override void Awake()
        {
            base.Awake();
            buttonComponents = uiObject.GetComponentsInChildren<Button>();
            dropOffButton = buttonComponents[0];
            dropOffButtonObject = dropOffButton.gameObject;
            textComponents = uiObject.GetComponentsInChildren<Text>();
            displayText = textComponents[0];
            UIOpen = true;
        }

        public override void Start()
        {
            base.Start();
            globeManager = FindObjectOfType<InterfaceFactory>().GlobeManager;
            worldMapGlobe = globeManager.WorldMapGlobe;
        }

        public void UpdateNavigationDisplay(List<Province> provinces, List<Country> countries, List<IMappableObject> nearbyObjects)
        {
            NavigationText = CreateNavigationInfoString(provinces, countries, nearbyObjects);
            SetDisplayText(NavigationText);
        }

        public string CreateNavigationInfoString(List<Province> provinces, List<Country> countries, List<IMappableObject> nearbyObjects)
        {
            string createdString = "";
            if (countries != null)
            {
                if(countries.Count > 0)
                {
                    if (countries.Count == 1)
                    {
                        createdString += "Country: ";
                        createdString += countries[0].name + System.Environment.NewLine;
                    }
                    else
                    {
                        createdString += "Countries: ";
                        foreach (Country country in countries)
                        {
                            createdString += country.name + System.Environment.NewLine + ",";
                        }
                    }
                }
            }
            if (provinces != null)
            {
                if(provinces.Count > 0)
                {
                    string nameType;
                    if(provinces.Count == 1)
                    {
                        string politicalProvince = provinces[0].attrib["PoliticalProvince"];
                        string climate = provinces[0].attrib["ClimateGroup"];
                        if (worldMapGlobe.countries[provinces[0].countryIndex].name == "United States of America")
                            nameType = "State: ";
                        else
                            nameType = "Province: ";
                        createdString += nameType + provinces[0].name + System.Environment.NewLine + "PoliticalProvince: " + politicalProvince +
                            System.Environment.NewLine + "Climate: " + climate;
                    }
                    else
                    {
                        foreach(Province province in provinces)
                        {
                            string politicalProvince = province.attrib["PoliticalProvince"];
                            string climate = province.attrib["ClimateGroup"];
                            if (worldMapGlobe.countries[province.countryIndex].name == "United States of America")
                                nameType = "State: ";
                            else
                                nameType = "Province: ";
                            createdString += nameType + province.name + System.Environment.NewLine + "PoliticalProvince: " + politicalProvince +
                            System.Environment.NewLine + "Climate: " + climate + System.Environment.NewLine;
                        }
                    }   
                }
                
                //Check to see if there's a highlighted object
                if (nearbyObjects != null)
                {
                    if (!nearbyObjects.Contains(gameManager.PlayerManager.Player))
                    {
                        foreach (MappableObject nearbyObject in nearbyObjects)
                        {
                            //Add the landmark to the string
                            createdString = createdString + System.Environment.NewLine + "Landmarks: " + nearbyObject.ObjectName;
                        }
                    }
                }
                //Display the string
                return createdString;
            }
            else
            {
                return null;
            }
        }

        public void SetDisplayText(string displayString)
        {
            displayText.text = displayString;
        }

        public void SetDropOffDelegate(DropOffDelegate dropOffDelegate)
        {
            dropOffButton.onClick.AddListener(delegate { dropOffDelegate(); });
        }

        public void ClearDropOffDelegate()
        {
            dropOffButton.onClick.RemoveAllListeners();
        }

        public void ToggleOptionForDropOff(bool active)
        {
            dropOffButtonObject.SetActive(active);
        }
    }
}
