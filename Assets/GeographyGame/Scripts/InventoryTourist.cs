﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WPM
{
    public class InventoryTourist : InventoryItem
    {
        private GameObject dialogPanel;
        private Text dialog;
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
        }

        public override void Deselected()
        {
            base.Deselected();
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            dialogPanel.SetActive(false);
        }

        public override void OnCellClick(int index)
        {
            bool playerAtLocation = false;
            Cell selectedCell = gameManager.worldGlobeMap.cells[index];

            Cell[] playerNeighbourCells = gameManager.worldGlobeMap.GetCellNeighbours(player.cellLocation);
            foreach (Cell cell in playerNeighbourCells)
            {
                if (cell == selectedCell)
                {
                    playerAtLocation = true;
                }
            }

            if (playerAtLocation)
            {
                switch (destinationType)
                {
                    case PROVINCE:
                        int selectedProvinceIndex = gameManager.worldGlobeMap.GetProvinceIndex(selectedCell.sphereCenter);
                        if (gameManager.worldGlobeMap.provinces[selectedProvinceIndex] == provinceDestination)
                        {
                            Deselected();
                            //Remove Tourist from Inventory
                            player.RemoveItem(inventoryLocation);
                        }

                        break;

                    case LANDMARK:
                        if (selectedCell == landmarkDestination.cell)
                        {
                            Deselected();
                            //Remove Tourist from Inventory
                            player.RemoveItem(inventoryLocation);
                        }
                        else
                        {
                            Cell[] selectedCellNeighbours = gameManager.worldGlobeMap.GetCellNeighbours(selectedCell.index);
                            foreach (Cell cell in selectedCellNeighbours)
                            {
                                if (cell == landmarkDestination.cell)
                                {
                                    Deselected();
                                    //Remove Tourist from Inventory
                                    player.RemoveItem(inventoryLocation);
                                }
                            }
                        }
                        break;

                    default:
                        break;
                }
            }
           
        }
    }
}