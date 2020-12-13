using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WPM
{
    public class PlayerCharacter : MappableObject
    {
        int travelRange = 10;
        //int distanceTraveled = 0;
        public int destination = 0;
        bool moving = false;
        public List<int> pathIndices = null;
        public float size = 0.005f;
        public List<InventoryItem> inventory = new List<InventoryItem>();
        private int inventorySize = 7;
        GeoPosAnimator anim;
        Vehicle vehicle = new Vehicle();
        //GameManager gameManager;
        GameObject InventoryPanel; 
        InventoryGUI inventoryGUI;
        List<int>[] cellsInRange;
        Dictionary<string, int> climateCosts = new Dictionary<string, int>();
        Dictionary<string, int> terrainCosts = new Dictionary<string, int>();
        public const int IMPASSABLE = 0;
        private const int STARTING_NUMBER_OF_TOURISTS = 2;

        public override void Start()
        {
            base.Start();
            //gameManager = GameManager.instance;
            map = WorldMapGlobe.instance;
            anim = gameObject.GetComponent(typeof(GeoPosAnimator)) as GeoPosAnimator;
            InventoryPanel = GameObject.Find("Canvas/InventoryPanel");
            inventoryGUI = InventoryPanel.GetComponent(typeof(InventoryGUI)) as InventoryGUI;
            vehicle.InitVehicles();
            climateCosts = vehicle.GetClimateVehicle("Mild");
            cellsInRange = gameManager.GetCellsInRange(cellLocation, travelRange+1);
            gameManager.OrientOnLocation(vectorLocation);

            //Generate Initial Tourists
            for (int i = 0; i < STARTING_NUMBER_OF_TOURISTS; i++)
            {
                gameManager.GenerateTourist();
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                gameManager.OrientOnLocation(vectorLocation);
            }
        }

        public override void Selected()
        {
            base.Selected();
            map.SetCellColor(cellLocation, Color.green, true);
            SetCellCosts();
        }

        public override void Deselected()
        {
            base.Deselected();
            ClearCellCosts();
            map.ClearCells(true, false, false);
            selected = false;
        }

        public override void OnCellEnter(int index)
        {
            if (!anim.auto)
            {
                //Attempt to display path to new location
                map.ClearCells(true, false, false);
                //map.SetCellColor(cellLocation, Color.green, true);
                pathIndices = DrawPath(cellLocation, index);
                if (pathIndices != null)
                {
                    pathIndices.Insert(0, cellLocation);
                }
                map.SetCellColor(cellLocation, Color.green, true);
            }
        }

        public override void OnCellClick(int index)
        {
            if (index == cellLocation)
            {
                Deselected();
            }
            //Attempt to move to new location
            else 
            if (pathIndices != null && moving == false)
            {
                destination = index;
                //Add latlon of each hex in path to animator's path
                anim.GenerateLatLon(pathIndices);
                // Compute path length
                anim.ComputePath();
                anim.auto = true;
                moving = true;
            }
        }

        public override void EndOfTurn(int turns)
        {
            // distanceTraveled = 0;
            //if (selected) 
            ClearCellCosts();
            Array.Clear(cellsInRange, 0, travelRange);
            cellsInRange = gameManager.GetCellsInRange(cellLocation, travelRange+1);
            //if (selected) 
            SetCellCosts();
        }

        /// <summary>
        /// Draws a path between startCellIndex and endCellIndex
        /// </summary>
        /// <returns><c>true</c>, if path was found and drawn, <c>false</c> otherwise.</returns>
        /// <param name="startCellIndex">Start cell index.</param>
        /// <param name="endCellIndex">End cell index.</param>
        List<int> DrawPath(int startCellIndex, int endCellIndex)
        {
            List<int> cellIndices;
            cellIndices = map.FindPath(startCellIndex, endCellIndex);
            map.ClearCells(true, false, false);

            if (cellIndices == null)
                return null;   // no path found

            //Check that there is enough remaining movement to travel path
            //Start by getting the cost between the starting cell and the first cell in the path
            int neighborIndex = map.GetCellNeighbourIndex(startCellIndex, cellIndices[0]);
            int pathCost = map.GetCellNeighbourCost(startCellIndex, neighborIndex);
            int i = 0;
            //Get the cumlative cost for the rest of the path
            foreach(int cellIndex in cellIndices)
            {
                if (i < (cellIndices.Count - 1))
                {
                    neighborIndex = map.GetCellNeighbourIndex(cellIndices[i], cellIndices[i + 1]);
                    pathCost = pathCost + map.GetCellNeighbourCost(cellIndices[i], neighborIndex);
                    i++;
                }
            }

            if (pathCost > travelRange)
                return null;   //Path costs more movement than is available

            //Path Successful
            // Color starting cell, end cell and path
            if (pathCost == travelRange)
                map.SetCellColor(cellIndices, Color.red, true);
            else
                map.SetCellColor(cellIndices, Color.grey, true);
            map.SetCellColor(startCellIndex, Color.green, true);
            
          //map.SetCellColor(endCellIndex, Color.red, true);

            return cellIndices;
        }

        /// <summary>
        /// Update new cell location for player character, update the cell tags to reflect change in occupancy,
        /// and update the distance the player character has travelled
        /// </summary>
        /// <param name="newCellIndex"></param>
        public void UpdateLocation(int newCellIndex)
        {
            //Update distance travelled
            int neighborIndex = map.GetCellNeighbourIndex(cellLocation, newCellIndex);
            //distanceTraveled = distanceTraveled + map.GetCellNeighbourCost(cellLocation, neighborIndex);
            //Update cell tags and player character location
            map.cells[cellLocation].tag = null;
            cellLocation = newCellIndex;
            map.cells[cellLocation].tag = GetInstanceID().ToString();
            vectorLocation = map.cells[cellLocation].sphereCenter;
            //Update Turns
            int turns = map.GetCellNeighbourCost(cellLocation, neighborIndex);
            gameManager.NextTurn(turns);
        }

        /// <summary>
        /// Clean up done at the end of player movement
        /// </summary>
        public void FinishedPathFinding()
        {
            pathIndices.Clear();
            map.ClearCells(true, false, false);
            if (selected)
            {
                map.SetCellColor(cellLocation, Color.green, true);
                if (!gameManager.cursorOverUI && gameManager.worldGlobeMap.lastHighlightedCellIndex >= 0)
                {
                    OnCellEnter(gameManager.worldGlobeMap.lastHighlightedCellIndex);
                }
            }
            moving = false;
        }

        /// <summary>
        /// Set the terrain costs of all the cells that are reachable by the player
        /// </summary>
        public void SetCellCosts()
        {
            foreach (int cell in cellsInRange[0])
            {
                //Get Cell Attributes from Province
                int provinceIndex = map.GetProvinceNearPoint(map.cells[cell].sphereCenter);
                Province province = map.provinces[provinceIndex];

                //Loop Through Each Neighbor and Set the Cost from the Neighbor to the Cell
                string climateAttribute = province.attrib["ClimateGroup"];
                if (climateAttribute != "")
                {
                    bool cellOccupied = false;
                    //Check if cell is occupied
                    if (map.cells[cell].tag != null)
                    {
                       //Check if cell is occupied by something other than the player
                       if(map.cells[cell].tag != GetInstanceID().ToString())
                       {
                            cellOccupied = true;
                       }
                    }
                    int cost = climateCosts[climateAttribute];
                    if (cost == IMPASSABLE || cellOccupied)  
                    {
                        map.SetCellCanCross(cell, false);
                    }
                    else
                    {
                        map.SetCellCanCross(cell, true);
                        foreach (Cell neighbour in map.GetCellNeighbours(cell))
                        {
                            map.SetCellNeighbourCost(neighbour.index, cell, cost, false);
                        }
                        
                    }
                }
            }
        }

        /// <summary>
        /// Clear all the terrain costs that were set by the player
        /// </summary>
        public void ClearCellCosts()
        {
            foreach (int cell in cellsInRange[0])
            {
                map.SetCellCanCross(cell, true);
                foreach (Cell neighbour in map.GetCellNeighbours(cell))
                {
                    map.SetCellNeighbourCost(neighbour.index, cell, 0, false);
                }
            }
        }

        public bool AddItem(InventoryItem item, int location)
        {
            inventory.Insert(location, item);
        
            if (inventory.Count > inventorySize)
                RemoveItem(inventorySize);
            /*
            if (inventory.Count < inventorySize)
            {
                item.inventoryLocation = 0; //inventory.Count;
                inventory.Add(item);
                inventoryGUI.AddItem(item, 0);
                return true;
            }
            else
            {
                return false;
            }
            */
            //Update inventory item locations
            foreach(InventoryItem inventoryItem in inventory)
            {
                inventoryItem.inventoryLocation = inventory.IndexOf(inventoryItem);
            }
            inventoryGUI.UpdateInventory(inventory);

            return true;
        }

        public void RemoveItem(int itemLocation)
        {
            inventory.RemoveAt(itemLocation);
            foreach (InventoryItem inventoryItem in inventory)
            {
                inventoryItem.inventoryLocation = inventory.IndexOf(inventoryItem);
            }
            inventoryGUI.UpdateInventory(inventory);
            if(inventory.Count == 0)
            {
                gameManager.GenerateTourist();
            }
        }
    }
}
