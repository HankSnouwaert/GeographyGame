using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WPM
{
    public class MouseOverInfoUI : UIElement, IMouseOverInfoUI
    {
        public string MouseOverInfoString { get; set; }
        private Component[] mouseOverInfoComponents;
        private Text mouseOverInfoText;

        protected override void Awake()
        {
            base.Awake();
            mouseOverInfoComponents = uiObject.GetComponentsInChildren<Text>();
            mouseOverInfoText = mouseOverInfoComponents[0] as Text;
        }

        protected override void Start()
        {
            base.Start();
            CloseUI();
        }

        public void UpdateUI()
        {
            if (!uiManager.CursorOverUI && !gameManager.GamePaused && globeManager.WorldMapGlobe.countryHighlighted != null && globeManager.WorldMapGlobe.lastHighlightedCellIndex >= 0 )
            {
                uiObject.SetActive(true);
                Province province = globeManager.WorldMapGlobe.provinceHighlighted;
                Country country = globeManager.WorldMapGlobe.countryHighlighted;
                MappableObject highlightedObject = null;
                if (gameManager.HighlightedObject is MappableObject)
                {
                    highlightedObject = (MappableObject)gameManager.HighlightedObject;
                }

                MouseOverInfoString = CreateMouseOverInfoString(province, country, highlightedObject);

                if (MouseOverInfoString != null)
                {
                    SetMouseOverInfoMessage(MouseOverInfoString);
                }
            }
            else
            {
                uiObject.SetActive(false);
            }
        }

        public void SetMouseOverInfoMessage(string textToSet)//THIS MAKES NO SENSE
        {
            uiObject.SetActive(true);
            mouseOverInfoText.text = MouseOverInfoString;
        }

        public string CreateMouseOverInfoString(Province province, Country country, IMappableObject highlightedObject)
        {
            if (province != null)
            {
                string createdString;
                string nameType;
                if (country.name == "United States of America")
                    nameType = "State: ";
                else
                    nameType = "Province: ";
                string politicalProvince = province.attrib["PoliticalProvince"];
                string climate = province.attrib["ClimateGroup"];
                createdString = "Country: " + country.name + System.Environment.NewLine + nameType + politicalProvince;

                //Check to see if there's a highlighted object
                if (highlightedObject != null && highlightedObject != gameManager.PlayerManager.PlayerCharacter)
                {
                    if (highlightedObject.ObjectName != null)
                    {
                        //Add the landmark to the string
                        createdString = createdString + System.Environment.NewLine + "Landmark: " + gameManager.HighlightedObject.ObjectName;
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
    }
}
