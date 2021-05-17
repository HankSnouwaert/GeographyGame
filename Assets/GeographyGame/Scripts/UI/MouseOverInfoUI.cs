using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WPM
{
    public class MouseOverInfoUI : UIElement, IMouseOverInfoUI
    {
        //Public Variables
        public string MouseOverInfoString { get; set; }
        //Private Reference Interfaces
        IPlayerManager playerManager;
        WorldMapGlobe worldMapGlobe;
        //UI Elements
        private Component[] mouseOverInfoComponents;
        private Text mouseOverInfoText;
        //Error Checking
        private bool componentMissing = false;

        protected override void Awake()
        {
            base.Awake();
            if (gameObject.activeSelf)
            {
                mouseOverInfoComponents = UIObject.GetComponentsInChildren<Text>();
                if (mouseOverInfoComponents == null || mouseOverInfoComponents.Length > 1)
                {
                    componentMissing = true;
                }
                else
                {
                    mouseOverInfoText = mouseOverInfoComponents[0] as Text;
                }
            }
        }

        protected override void Start()
        {
            base.Start();

            if (componentMissing)
                errorHandler.ReportError("Error Finding Mouse Over Info UI components", ErrorState.close_window);

            worldMapGlobe = globeManager.WorldMapGlobe;
            if (worldMapGlobe == null)
                errorHandler.ReportError("World Map Globe missing", ErrorState.restart_scene);

            playerManager = gameManager.PlayerManager;
            if (playerManager == null)
                errorHandler.ReportError("Player Manager missing", ErrorState.restart_scene);

            CloseUI();
        }

        public void UpdateUI()
        {
            if (!uiManager.CursorOverUI && !gameManager.GamePaused && worldMapGlobe.countryHighlighted != null && worldMapGlobe.lastHighlightedCellIndex >= 0 )
            {
                UIObject.SetActive(true);
                Province province = worldMapGlobe.provinceHighlighted;
                Country country = worldMapGlobe.countryHighlighted;
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
                UIObject.SetActive(false);
            }
        }

        public void SetMouseOverInfoMessage(string textToSet)
        {
            UIObject.SetActive(true);
            mouseOverInfoText.text = textToSet;
        }

        public string CreateMouseOverInfoString(Province province, Country country, IMappableObject highlightedObject)
        {
            if (province != null && country != null)
            {
                string createdString;
                string nameType;
                if (country.name == "United States of America")
                    nameType = "State: ";
                else
                    nameType = "Province: ";
                string politicalProvince;
                string climate;
                try
                {
                    politicalProvince = province.attrib["PoliticalProvince"];  
                    climate = province.attrib["ClimateGroup"]; 
                }
                catch(System.Exception ex)
                {
                    errorHandler.CatchException(ex, ErrorState.close_window);
                    return null;
                }
                createdString = "Country: " + country.name + System.Environment.NewLine + nameType + politicalProvince;

                //Check to see if there's a highlighted object
                if (highlightedObject != null && highlightedObject != playerManager.PlayerCharacter)
                {
                    if (highlightedObject.ObjectName != null)
                    {
                        //Add the landmark to the string
                        createdString = createdString + System.Environment.NewLine + "Landmark: " + highlightedObject.ObjectName;
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
