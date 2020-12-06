using System.Collections;
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
        private bool boarding = false;
        string savedText = null;

        private const int PROVINCE_MULTIPLIER = 1;
        private const int LANDMARK_MULTIPLIER = 10;
        private const int COUNTRY_MULTIPLIER = 1;

        public override void Start()
        {
            base.Start();
            dialogPanel = gameManager.dialogPanel;
            Transform textObject = dialogPanel.transform.GetChild(0);
            dialog = textObject.gameObject.GetComponent(typeof(Text)) as Text;
            Transform dropOffButtonTransfrom = dialogPanel.transform.GetChild(1);
            dropOffButton = dropOffButtonTransfrom.gameObject.GetComponent(typeof(Button)) as Button;

            dropOffButtonObject = dropOffButtonTransfrom.gameObject;
            dropOffButtonObject.SetActive(false);

            if (dialogPanel.activeSelf)
                savedText = dialog.text;
            else
                dialogPanel.SetActive(true);

            dialog.text = "Tourist Boarding: Please Wait";

            boarding = true;

            

            /*
            int coinFlip = Random.Range(0, 2);
            if (coinFlip == 0)
            {
                //Province
                destinationType = PROVINCE;
                int countryIndex = gameManager.worldGlobeMap.GetCountryIndex("United States of America");
                possibleProvinces = gameManager.worldGlobeMap.countries[countryIndex].provinces;
                destinationIndex = Random.Range(0, possibleProvinces.Length);
                provinceDestination = possibleProvinces[destinationIndex];
                destinationName = possibleProvinces[destinationIndex].name;
            }
            else
            {
                //Landmark
                destinationType = LANDMARK;
                int landmarkIndex = Random.Range(0, gameManager.culturalLandmarks.Count);
                landmarkDestination = gameManager.culturalLandmarks[landmarkIndex];
                destinationName = landmarkDestination.objectName;
            }
            */
        }

        private void Update()
        {
            if (boarding)
            {
                SetDestination();

                if (savedText != null)
                    dialog.text = savedText;
                else
                    dialogPanel.SetActive(false);
                boarding = false;
                dropOffButtonObject.SetActive(true);
            }
           
        }

        private void SetDestination()
        {
            //Get Random Tourist Destination
            List<int>[] cellsInRange = gameManager.GetCellsInRange(player.cellLocation, 10);
            List<int>[] provincesInRange = gameManager.GetProvincesInRange(player.cellLocation, cellsInRange);
            List<string>[] landmarksInRange = gameManager.GetLandmarksInRange(player.cellLocation, cellsInRange);
            List<int> provinceChoices = new List<int>();
            List<string> landmarkChoices = new List<string>();
            List<int> countryChoices = new List<int>();
            int i = 1;  //Initialize to 1 rather than 0 to avoid requesting a destination at the player's location
            int timeMultiplier;
            int totalMultiplier;

            while (i < cellsInRange.Length)
            {
                if(provincesInRange[i].Count > 0)
                {
                    foreach(int province in provincesInRange[i])
                    {
                        int country = gameManager.worldGlobeMap.provinces[province].countryIndex;
                        string countryName = gameManager.worldGlobeMap.countries[country].name;
                        if (countryName == "United States of America" || countryName == "Canada")
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
                        else
                        {
                            if (!countryChoices.Contains(country))
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
                        }

                    }
                    
                }
                if (landmarksInRange[i].Count > 0)
                {
                    foreach(string landmark in landmarksInRange[i])
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
                    
                }

                i++;

                //Avoid adding provinces at the starting location if possible
                //if ((provinceChoices.Count > 0 || landmarkChoices.Count > 0  || countryChoices.Count > 0) && i == 0)
                //    break;
            }

            destinationIndex = Random.Range(0, provinceChoices.Count+landmarkChoices.Count+countryChoices.Count);
            if(destinationIndex < provinceChoices.Count)
            {
                destinationType = PROVINCE;
                provinceDestination = gameManager.worldGlobeMap.provinces[provinceChoices[destinationIndex]];
                destinationName = provinceDestination.name;
                gameManager.recentProvinceDestinations.Insert(0, provinceChoices[destinationIndex]);
                while (gameManager.recentProvinceDestinations.Count >= gameManager.trackingTime)
                {
                    gameManager.recentProvinceDestinations.RemoveAt(gameManager.trackingTime-1); //ERROR: Index was out of range
                }
                    
            }
            else if((destinationIndex > provinceChoices.Count) && (destinationIndex < (landmarkChoices.Count+provinceChoices.Count)))
            {
                destinationType = LANDMARK;
                destinationIndex = destinationIndex - provinceChoices.Count;
                landmarkDestination = gameManager.culturalLandmarks[landmarkChoices[destinationIndex]];
                destinationName = landmarkDestination.objectName;
                gameManager.recentLandmarkDestinations.Insert(0, landmarkChoices[destinationIndex]);
                while (gameManager.recentLandmarkDestinations.Count >= gameManager.trackingTime)
                {
                    gameManager.recentLandmarkDestinations.RemoveAt(gameManager.trackingTime);
                }
            }
            else
            {
                destinationType = COUNTRY;
                destinationIndex = destinationIndex - provinceChoices.Count - landmarkChoices.Count;
                countryDestination = gameManager.worldGlobeMap.countries[countryChoices[destinationIndex]];
                destinationName = countryDestination.name;
                gameManager.recentCountryDestinations.Insert(0, countryChoices[destinationIndex]);
                while (gameManager.recentCountryDestinations.Count >= gameManager.trackingTime)
                {
                    gameManager.recentCountryDestinations.RemoveAt(gameManager.trackingTime);
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

        public void DropOff()
        {
            //Clear the Event System so that it gets updated with the tourist if the drop off fails
            EventSystem.current.SetSelectedGameObject(null);
            Cell playerCell = gameManager.worldGlobeMap.cells[player.cellLocation];
            switch (destinationType)
            {

                case PROVINCE:
                    List<int> selectedProvinces = gameManager.GetProvicesInCell(player.cellLocation);
                    foreach(int province in selectedProvinces)
                    {
                        if(gameManager.worldGlobeMap.provinces[province] == provinceDestination)
                        {
                            Deselected();
                            //Remove Tourist from Inventory
                            player.RemoveItem(inventoryLocation);
                            gameManager.cursorOverUI = false;
                        }
                    }
                    /*
                    int selectedProvinceIndex = gameManager.worldGlobeMap.GetProvinceIndex(playerCell.sphereCenter);
                    //Check for cases where hex center is not on a province
                    if (selectedProvinceIndex < 0)
                    {

                    }
                    if (gameManager.worldGlobeMap.provinces[selectedProvinceIndex] == provinceDestination)
                    {
                        Deselected();
                        //Remove Tourist from Inventory
                        player.RemoveItem(inventoryLocation);
                        gameManager.cursorOverUI = false;
                    }
                    else
                    {
                        bool provinceOverlaps = false;
                        foreach (Region region in provinceDestination.regions)
                        {
                            foreach (Vector3 spherePoint in region.spherePoints)
                            {
                                if(gameManager.worldGlobeMap.GetCellIndex(spherePoint) == player.cellLocation)
                                {
                                    provinceOverlaps = true;
                                    break;
                                }
                            }
                            if (provinceOverlaps)
                                break;
                        }
                        if (provinceOverlaps)
                        {
                            Deselected();
                            //Remove Tourist from Inventory
                            player.RemoveItem(inventoryLocation);
                            gameManager.cursorOverUI = false;
                        }
                        else
                        {
                            Debug.Log("Incorrect Location");
                        }
                    }
                    */
                    break;

                case LANDMARK:
                    if (playerCell == landmarkDestination.cell)
                    {
                        Deselected();
                        //Remove Tourist from Inventory
                        player.RemoveItem(inventoryLocation);
                        gameManager.cursorOverUI = false;
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
                                gameManager.cursorOverUI = false;
                            }
                        }
                    }
                    break;
                case COUNTRY:
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
                            gameManager.cursorOverUI = false;
                        }
                        else
                        {
                            Debug.Log("Incorrect Location");
                        }
                    }
                    break;

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
