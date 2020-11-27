using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WPM
{
    public class InventoryTourist : InventoryItem
    {
        private GameObject dialogPanel;
        private Text dialog;
        private Button dropOffButton;
        private string destinationName;
        private int destinationIndex;
        private Province[] possibleProvinces;
        private Province provinceDestination;
        private Landmark landmarkDestination;
        private int destinationType;
        private const int PROVINCE = 0;
        private const int LANDMARK = 1;

        public override void Start()
        {
            base.Start();
            dialogPanel = gameManager.dialogPanel;
            Transform textObject = dialogPanel.transform.GetChild(0);
            dialog = textObject.gameObject.GetComponent(typeof(Text)) as Text;
            Transform dropOffButtonObject = dialogPanel.transform.GetChild(1);
            dropOffButton = dropOffButtonObject.gameObject.GetComponent(typeof(Button)) as Button;
            //Get Random Tourist Destination
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
                destinationName = landmarkDestination.landmarkName;
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

        public override void OnCellEnter(int index)
        {
            //player.OnCellEnter(index);
        }
    }
}
