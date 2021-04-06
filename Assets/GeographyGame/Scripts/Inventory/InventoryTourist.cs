using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace WPM
{
    public class InventoryTourist : InventoryItem
    {
        //private GameObject dialogPanel;
        //private GameObject dropOffButtonObject;
        private Text dialog;
        private IDropOffUI dropOffUI;
        private IUIManager uiManager;
        private GlobeManager globeManager;
        private IGlobeParser globeParser;
        private IGlobeInfo globeInfo;
        //private Button dropOffButton;
        private string destinationName;
        private int destinationIndex;
        //private Province[] possibleProvinces;
        private Province provinceDestination;
        private Landmark landmarkDestination;
        private Country countryDestination;
        private int destinationType;
        private const int PROVINCE = 0;
        private const int LANDMARK = 1;
        private const int COUNTRY = 2;
        readonly string savedText = null;


        private const int PROVINCE_MULTIPLIER = 1;
        private const int LANDMARK_MULTIPLIER = 10;
        private const int COUNTRY_MULTIPLIER = 1;
        private const int TOURIST_DROP_OFF_SCORE = 100;

        public override void Start()
        {
            base.Start();
            /*
            dialogPanel = uiManager.NavigationUIObject;
            Transform textObject = dialogPanel.transform.GetChild(0);
            dialog = textObject.gameObject.GetComponent(typeof(Text)) as Text;
            Transform dropOffButtonTransfrom = dialogPanel.transform.GetChild(1);
            dropOffButton = dropOffButtonTransfrom.gameObject.GetComponent(typeof(Button)) as Button;

            dropOffButtonObject = dropOffButtonTransfrom.gameObject;
            */
            dropOffUI = gameManager.UIManager.DropOffUI;
            uiManager = gameManager.UIManager;
            globeManager = FindObjectOfType<GlobeManager>();
            globeParser = globeManager.GlobeParser;
            globeInfo = globeManager.GlobeInfo;
            SetDestination(); uiManager.InventoryPopUpUI.DisplayPopUp("Hey there!  I want to see " + destinationName + "!", false);
            dropOffUI.ToggleOptionForDropOff(false);
        }

        private void SetDestination()
        {
            List<int> provinceChoices = new List<int>();
            List<string> landmarkChoices = new List<string>();
            List<int> countryChoices = new List<int>();
            int timeMultiplier;
            int totalMultiplier;
            //Get possible Provinces
            foreach (int province in gameManager.CurrentRegion.provinces)
            {
                timeMultiplier = gameManager.RecentProvinceDestinations.IndexOf(province);

                //Check if destination is no longer being tracked
                if (timeMultiplier < 0)
                    timeMultiplier = gameManager.TrackingTime;

                totalMultiplier = timeMultiplier * PROVINCE_MULTIPLIER;

                for (int n = 0; n < totalMultiplier; n++)
                {
                    provinceChoices.Add(province);
                }
            }
            //Get possible Landmarks
            foreach (string landmark in gameManager.CurrentRegion.landmarks)
            {
                timeMultiplier = gameManager.RecentLandmarkDestinations.IndexOf(landmark);

                //Check if destination is no longer being tracked
                if (timeMultiplier < 0)
                    timeMultiplier = gameManager.TrackingTime;

                totalMultiplier = timeMultiplier * LANDMARK_MULTIPLIER;

                for (int n = 0; n < totalMultiplier; n++)
                {
                    landmarkChoices.Add(landmark);
                }
            }
            //Get possible Countries
            foreach (int country in gameManager.CurrentRegion.countries)
            {
                timeMultiplier = gameManager.RecentCountryDestinations.IndexOf(country);

                //Check if destination is no longer being tracked
                if (timeMultiplier < 0)
                    timeMultiplier = gameManager.TrackingTime;

                totalMultiplier = timeMultiplier * COUNTRY_MULTIPLIER;

                for (int n = 0; n < totalMultiplier; n++)
                {
                    countryChoices.Add(country);
                }
            }

            //Treat all three lists as a combined list and get a random number that points to an element from one of them
            destinationIndex = Random.Range(0, provinceChoices.Count + landmarkChoices.Count + countryChoices.Count);
            if (destinationIndex < provinceChoices.Count)
            {
                destinationType = PROVINCE;
                provinceDestination = gameManager.worldGlobeMap.provinces[provinceChoices[destinationIndex]];
                destinationName = provinceDestination.name;
                gameManager.RecentProvinceDestinations.Insert(0, provinceChoices[destinationIndex]);
                while (gameManager.RecentProvinceDestinations.Count >= gameManager.TrackingTime)
                {
                    gameManager.RecentProvinceDestinations.RemoveAt(gameManager.TrackingTime - 1);
                }

            }
            else if ((destinationIndex > provinceChoices.Count) && (destinationIndex < (landmarkChoices.Count + provinceChoices.Count)))
            {
                destinationType = LANDMARK;
                destinationIndex = destinationIndex - provinceChoices.Count;
                landmarkDestination = globeInfo.CulturalLandmarksByName[landmarkChoices[destinationIndex]];
                destinationName = landmarkDestination.objectName;
                gameManager.RecentLandmarkDestinations.Insert(0, landmarkChoices[destinationIndex]);
                while (gameManager.RecentLandmarkDestinations.Count >= gameManager.TrackingTime)
                {
                    gameManager.RecentLandmarkDestinations.RemoveAt(gameManager.TrackingTime - 1);
                }
            }
            else
            {
                destinationType = COUNTRY;
                destinationIndex = destinationIndex - provinceChoices.Count - landmarkChoices.Count;
                countryDestination = gameManager.worldGlobeMap.countries[countryChoices[destinationIndex]]; //ERROR: Index Out of Range Exception
                destinationName = countryDestination.name;
                gameManager.RecentCountryDestinations.Insert(0, countryChoices[destinationIndex]);
                while (gameManager.RecentCountryDestinations.Count >= gameManager.TrackingTime)
                {
                    gameManager.RecentCountryDestinations.RemoveAt(gameManager.TrackingTime - 1);
                }
            }
        }

        public override void Selected()
        {
            base.Selected();
            int debug = inventoryLocation;
            dropOffUI.ToggleOptionForDropOff(true);
            //dialog.text = "I want to go to " + destinationName;
            dropOffUI.SetDropOffDelegate(DropOff);
            //dropOffButton.onClick.AddListener(delegate { DropOff(); });
            SetPopUpRequest(true);
        }

        public override void Deselected()
        {
            base.Deselected();
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            dropOffUI.ToggleOptionForDropOff(false);
            dropOffUI.ClearDropOffDelegate();
            uiManager.InventoryPopUpUI.ClearPopUp(true);
            //dropOffButton.onClick.RemoveAllListeners();
        }

        public override void MouseEnter()
        {
            base.MouseEnter();
            uiManager.InventoryPopUpUI.DisplayPopUp("I want to see " + destinationName + "!", false);
        }

        public override void OnMouseExit()
        {
            base.OnMouseExit();
            uiManager.InventoryPopUpUI.ClearPopUp(false);
        }

        public void DropOff()
        {
            //Clear the Event System so that it gets updated with the tourist if the drop off fails
            EventSystem.current.SetSelectedGameObject(null);
            Cell playerCell = gameManager.worldGlobeMap.cells[player.cellLocation];
            switch (destinationType)
            {
                case PROVINCE:
                    List<int> selectedProvinces = globeParser.ProvinceParser.GetProvicesInCell(player.cellLocation);
                    bool correctProvince = false;
                    foreach (int province in selectedProvinces)
                    {
                        if(gameManager.worldGlobeMap.provinces[province] == provinceDestination)
                        {
                            Deselected();
                            //Remove Tourist from Inventory
                            player.RemoveItem(inventoryLocation);
                            gameManager.UpdateScore(TOURIST_DROP_OFF_SCORE);
                            uiManager.CursorOverUI = false;
                            correctProvince = true;
                            uiManager.InventoryPopUpUI.DisplayPopUp("Exactly where I wanted to go!", false);
                            gameManager.DropOff(true);
                        }
                    }
                    if(correctProvince == false)
                    {
                        uiManager.InventoryPopUpUI.DisplayPopUp("Well this doesn't look right. . . .", false);
                        gameManager.DropOff(false);
                    }
                        
                    break;

                case LANDMARK:
                    bool landmarkReached = false;
                    if (playerCell == landmarkDestination.cell)
                    {
                        Deselected();
                        //Remove Tourist from Inventory
                        player.RemoveItem(inventoryLocation);
                        gameManager.UpdateScore(TOURIST_DROP_OFF_SCORE);
                        uiManager.CursorOverUI = false;
                        landmarkReached = true;
                        uiManager.InventoryPopUpUI.DisplayPopUp("Exactly where I wanted to go!", false);
                        gameManager.DropOff(true);
                    }
                    else
                    {
                        Cell[] selectedCellNeighbours = gameManager.worldGlobeMap.GetCellNeighbours(playerCell.index);
                        foreach (Cell cell in selectedCellNeighbours)
                        {
                            if (cell == landmarkDestination.cell)
                            {
                                Deselected();
                                //Remove Tourist from Inventory
                                player.RemoveItem(inventoryLocation);
                                gameManager.UpdateScore(TOURIST_DROP_OFF_SCORE);
                                uiManager.CursorOverUI = false;
                                landmarkReached = true;
                                uiManager.InventoryPopUpUI.DisplayPopUp("Exactly where I wanted to go!", false);
                                gameManager.DropOff(true);
                            }
                        }
                    }

                    if(landmarkReached == false)
                    {
                        uiManager.InventoryPopUpUI.DisplayPopUp("Well this doesn't look right. . . .", false);
                        gameManager.DropOff(false);
                    }
                       
                    break;
                case COUNTRY:
                    List<int> selectedCountries = globeParser.CountryParser.GetCountriesInCell(player.cellLocation);
                    bool correctCountry = false;
                    foreach (int countryIndex in selectedCountries)
                    {
                        if (gameManager.worldGlobeMap.countries[countryIndex] == countryDestination)
                        {
                            Deselected();
                            //Remove Tourist from Inventory
                            player.RemoveItem(inventoryLocation);
                            gameManager.UpdateScore(TOURIST_DROP_OFF_SCORE);
                            uiManager.CursorOverUI = false;
                            correctCountry = true;
                            uiManager.InventoryPopUpUI.DisplayPopUp("Exactly where I wanted to go!", false);
                            gameManager.DropOff(true);
                        }
                    }
                    if (correctCountry == false)
                    {
                        uiManager.InventoryPopUpUI.DisplayPopUp("Well this doesn't look right. . . .", false);
                        gameManager.DropOff(false);
                    }
                    break;
                    /*
                    int selectedCountryIndex = gameManager.worldGlobeMap.GetCountryIndex(playerCell.sphereCenter);
                    if (gameManager.worldGlobeMap.countries[selectedCountryIndex] == countryDestination)
                    {
                        Deselected();
                        //Remove Tourist from Inventory
                        player.RemoveItem(inventoryLocation);
                        gameManager.cursorOverUI = false;
                    }
                    else
                    {
                        bool countryOverlaps = false;
                        foreach (Region region in countryDestination.regions)
                        {
                            foreach (Vector3 spherePoint in region.spherePoints)
                            {
                                if (gameManager.worldGlobeMap.GetCellIndex(spherePoint) == player.cellLocation)
                                {
                                    countryOverlaps = true;
                                    break;
                                }
                            }
                            if (countryOverlaps)
                                break;
                        }
                        if (countryOverlaps)
                        {
                            Deselected();
                            //Remove Tourist from Inventory
                            player.RemoveItem(inventoryLocation);
                            gameManager.UpdateScore(TOURIST_DROP_OFF_SCORE);
                            gameManager.cursorOverUI = false;
                            gameManager.DisplayPopUp("Exactly where I wanted to go!");
                            gameManager.DropOff(true);
                        }
                        else
                        {
                            gameManager.DisplayPopUp("Well this doesn't look right. . . .");
                            gameManager.DropOff(false);
                        }
                    }
                    break;
                    */
                default:
                    break;
            }
        }

        public void SetPopUpRequest(bool persistant)
        {
            uiManager.InventoryPopUpUI.DisplayPopUp("I want to see " + destinationName + "!", persistant);
        }

        public override void OnCellClick(int index)
        {
            //player.OnCellClick(index);
            if(index == player.cellLocation)
            {
                player.Selected();
            }
        }

    }
}
