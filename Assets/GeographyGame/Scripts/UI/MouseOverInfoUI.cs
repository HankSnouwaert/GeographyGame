using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WPM
{
    public class MouseOverInfoUI : UIElement, IMouseOverInfoUI
    {
        public string MouseOverInfoString { get; set; }
        private Component[] hexInfoComponents;
        private Text hexInfoText;

        public override void Awake()
        {
            base.Awake();
            uiObject.SetActive(false);
            // Transform textObject = uiObject.transform.GetChild(0);
            hexInfoComponents = uiObject.GetComponentsInChildren<Text>();
            hexInfoText = hexInfoComponents[0] as Text;
        }

        public void UpdateUI()
        {
            if (!uiManager.CursorOverUI && !gameManager.GamePaused && gameManager.worldGlobeMap.countryHighlighted != null && gameManager.worldGlobeMap.lastHighlightedCellIndex >= 0 )
            {
                uiObject.SetActive(true);
                Province province = gameManager.worldGlobeMap.provinceHighlighted;
                Country country = gameManager.worldGlobeMap.countryHighlighted;
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

        public void SetMouseOverInfoMessage(string textToSet)
        {
            uiObject.SetActive(true);
            hexInfoText.text = MouseOverInfoString;
        }

        public string CreateMouseOverInfoString(Province province, Country country, MappableObject highlightedObject)
        {
            if (province != null)
            {
                string createdString;
                //Create the string of the hex's info
                string nameType;
                if (country.name == "United States of America")
                    nameType = "State: ";
                else
                    nameType = "Province: ";
                string politicalProvince = province.attrib["PoliticalProvince"];
                string climate = province.attrib["ClimateGroup"];
                createdString = "Country: " + country.name + System.Environment.NewLine + nameType + politicalProvince;

                //Check to see if there's a highlighted object
                if (highlightedObject != null && highlightedObject != gameManager.player)
                {
                    if (highlightedObject.objectName != null)
                    {
                        //Add the landmark to the string
                        createdString = createdString + System.Environment.NewLine + "Landmark: " + gameManager.HighlightedObject.objectName;
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
