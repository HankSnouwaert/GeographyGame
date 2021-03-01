﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace WPM
{
    public class InventoryTourist : InventoryItem
    {
        private GameObject dialogPanel;
        private GameObject dropOffButtonObject;
        private Text dialog;
        private Button dropOffButton;
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
        string savedText = null;

        private const int PROVINCE_MULTIPLIER = 1;
        private const int LANDMARK_MULTIPLIER = 10;
        private const int COUNTRY_MULTIPLIER = 1;
        private const int TOURIST_DROP_OFF_SCORE = 100;

        public override void Start()
        {
            base.Start();
            dialogPanel = gameManager.dialogPanel;
            Transform textObject = dialogPanel.transform.GetChild(0);
            dialog = textObject.gameObject.GetComponent(typeof(Text)) as Text;
            Transform dropOffButtonTransfrom = dialogPanel.transform.GetChild(1);
            dropOffButton = dropOffButtonTransfrom.gameObject.GetComponent(typeof(Button)) as Button;

            dropOffButtonObject = dropOffButtonTransfrom.gameObject;

            SetDestination();

            gameManager.DisplayPopUp("Hey there!  I want to see " + destinationName + "!");


            dropOffButtonObject.SetActive(true);

        }

        private void SetDestination()
        {
            List<int> provinceChoices = new List<int>();
            List<string> landmarkChoices = new List<string>();
            List<int> countryChoices = new List<int>();
            int timeMultiplier;
            int totalMultiplier;
            //Get possible Provinces
            foreach (int province in gameManager.currentRegion.provinces)
            {
                timeMultiplier = gameManager.recentProvinceDestinations.IndexOf(province);

                //Check if destination is no longer being tracked
                if (timeMultiplier < 0)
                    timeMultiplier = gameManager.trackingTime;

                totalMultiplier = timeMultiplier * PROVINCE_MULTIPLIER;

                for (int n = 0; n < totalMultiplier; n++)
                {
                    provinceChoices.Add(province);
                }
            }
            //Get possible Landmarks
            foreach (string landmark in gameManager.currentRegion.landmarks)
            {
                timeMultiplier = gameManager.recentLandmarkDestinations.IndexOf(landmark);

                //Check if destination is no longer being tracked
                if (timeMultiplier < 0)
                    timeMultiplier = gameManager.trackingTime;

                totalMultiplier = timeMultiplier * LANDMARK_MULTIPLIER;

                for (int n = 0; n < totalMultiplier; n++)
                {
                    landmarkChoices.Add(landmark);
                }
            }
            //Get possible Countries
            foreach (int country in gameManager.currentRegion.countries)
            {
                timeMultiplier = gameManager.recentCountryDestinations.IndexOf(country);

                //Check if destination is no longer being tracked
                if (timeMultiplier < 0)
                    timeMultiplier = gameManager.trackingTime;

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
                gameManager.recentProvinceDestinations.Insert(0, provinceChoices[destinationIndex]);
                while (gameManager.recentProvinceDestinations.Count >= gameManager.trackingTime)
                {
                    gameManager.recentProvinceDestinations.RemoveAt(gameManager.trackingTime - 1);
                }

            }
            else if ((destinationIndex > provinceChoices.Count) && (destinationIndex < (landmarkChoices.Count + provinceChoices.Count)))
            {
                destinationType = LANDMARK;
                destinationIndex = destinationIndex - provinceChoices.Count;
                landmarkDestination = gameManager.culturalLandmarksByName[landmarkChoices[destinationIndex]];
                destinationName = landmarkDestination.objectName;
                gameManager.recentLandmarkDestinations.Insert(0, landmarkChoices[destinationIndex]);
                while (gameManager.recentLandmarkDestinations.Count >= gameManager.trackingTime)
                {
                    gameManager.recentLandmarkDestinations.RemoveAt(gameManager.trackingTime - 1);
                }
            }
            else
            {
                destinationType = COUNTRY;
                destinationIndex = destinationIndex - provinceChoices.Count - landmarkChoices.Count;
                countryDestination = gameManager.worldGlobeMap.countries[countryChoices[destinationIndex]]; //ERROR: Index Out of Range Exception
                destinationName = countryDestination.name;
                gameManager.recentCountryDestinations.Insert(0, countryChoices[destinationIndex]);
                while (gameManager.recentCountryDestinations.Count >= gameManager.trackingTime)
                {
                    gameManager.recentCountryDestinations.RemoveAt(gameManager.trackingTime - 1);
                }
            }
        }

        public override void Selected()
        {
            base.Selected();
            int debug = inventoryLocation;
            dialogPanel.SetActive(true);
            dialog.text = "I want to go to " + destinationName;
            dropOffButton.onClick.AddListener(delegate { DropOff(); });
        }

        public override void Deselected()
        {
            base.Deselected();
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            dialogPanel.SetActive(false);
            dropOffButton.onClick.RemoveAllListeners();
        }

        public override void MouseEnter()
        {
            base.MouseEnter();
            gameManager.DisplayPopUp("I want to see " + destinationName + "!");
        }

        public void DropOff()
        {
            //Clear the Event System so that it gets updated with the tourist if the drop off fails
            EventSystem.current.SetSelectedGameObject(null);
            Cell playerCell = gameManager.worldGlobeMap.cells[player.cellLocation];
            switch (destinationType)
            {
                case PROVINCE:
                    List<int> selectedProvinces = gameManager.GetProvicesInCell(player.cellLocation);
                    bool correctProvince = false;
                    foreach (int province in selectedProvinces)
                    {
                        if(gameManager.worldGlobeMap.provinces[province] == provinceDestination)
                        {
                            Deselected();
                            //Remove Tourist from Inventory
                            player.RemoveItem(inventoryLocation);
                            gameManager.UpdateScore(TOURIST_DROP_OFF_SCORE);
                            gameManager.cursorOverUI = false;
                            correctProvince = true;
                            gameManager.DisplayPopUp("Exactly where I wanted to go!");
                            gameManager.DropOff(true);
                        }
                    }
                    if(correctProvince == false)
                    {
                        gameManager.DisplayPopUp("Well this doesn't look right. . . .");
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
                        gameManager.cursorOverUI = false;
                        landmarkReached = true;
                        gameManager.DisplayPopUp("Exactly where I wanted to go!");
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
                                gameManager.cursorOverUI = false;
                                landmarkReached = true;
                                gameManager.DisplayPopUp("Exactly where I wanted to go!");
                                gameManager.DropOff(true);
                            }
                        }
                    }

                    if(landmarkReached == false)
                    {
                        gameManager.DisplayPopUp("Well this doesn't look right. . . .");
                        gameManager.DropOff(false);
                    }
                       
                    break;
                case COUNTRY:
                    List<int> selectedCountries = gameManager.GetCountriesInCell(player.cellLocation);
                    bool correctCountry = false;
                    foreach (int countryIndex in selectedCountries)
                    {
                        if (gameManager.worldGlobeMap.countries[countryIndex] == countryDestination)
                        {
                            Deselected();
                            //Remove Tourist from Inventory
                            player.RemoveItem(inventoryLocation);
                            gameManager.UpdateScore(TOURIST_DROP_OFF_SCORE);
                            gameManager.cursorOverUI = false;
                            correctCountry = true;
                            gameManager.DisplayPopUp("Exactly where I wanted to go!");
                            gameManager.DropOff(true);
                        }
                    }
                    if (correctCountry == false)
                    {
                        gameManager.DisplayPopUp("Well this doesn't look right. . . .");
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