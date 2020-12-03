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
        private int destinationType;
        private const int PROVINCE = 0;
        private const int LANDMARK = 1;
        private bool boarding = false;
        string savedText = null;

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
                GetDestination();

                if (savedText != null)
                    dialog.text = savedText;
                else
                    dialogPanel.SetActive(false);
                boarding = false;
                dropOffButtonObject.SetActive(true);
            }
           
        }

        private void GetDestination()
        {
            //Get Random Tourist Destination
            List<int>[] cellsInRange = gameManager.GetCellsInRange(player.cellLocation, 10);
            List<int>[] provincesInRange = gameManager.GetProvincesInRange(player.cellLocation, cellsInRange);
            List<string>[] landmarksInRange = gameManager.GetLandmarksInRange(player.cellLocation, cellsInRange);
            List<int> provinceChoices = new List<int>();
            List<string> landmarkChoices = new List<string>();
            int i = cellsInRange.Length - 1;
            while (i >= 0)
            {
                if(provincesInRange[i].Count > 0)
                {
                    provinceChoices.AddRange(provincesInRange[i]);
                }
                if (landmarksInRange[i].Count > 0)
                {
                    landmarkChoices.AddRange(landmarksInRange[i]);
                }

                i--;

                //Avoid adding provinces at the starting location if possible
                if ((provinceChoices.Count > 0 || landmarkChoices.Count > 0) && i == 0)
                    break;
            }

            destinationIndex = Random.Range(0, provinceChoices.Count+landmarkChoices.Count);
            if(destinationIndex <= provinceChoices.Count)
            {
                destinationType = PROVINCE;
                provinceDestination = gameManager.worldGlobeMap.provinces[provinceChoices[destinationIndex]];
                destinationName = provinceDestination.name;
            }
            else
            {
                destinationType = LANDMARK;
                destinationIndex = destinationIndex - provinceChoices.Count;
                landmarkDestination = gameManager.culturalLandmarks[landmarkChoices[destinationIndex]];
                destinationName = landmarkDestination.name;
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
                    int selectedProvinceIndex = gameManager.worldGlobeMap.GetProvinceIndex(playerCell.sphereCenter);
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
