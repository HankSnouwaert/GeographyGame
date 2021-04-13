﻿using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WPM
{
    public class PlayerCharacter : MappableObject, ITurnBasedObject
    {
        readonly int travelRange = 30;
        //int distanceTraveled = 0;
        public int destination = 0;
        bool moving = false;
        public List<int> pathIndices = null;
        private readonly float size = 0.0003f;
        public List<InventoryItem> inventory = new List<InventoryItem>();
        private readonly int inventorySize = 7;
        public bool stop = false;
        GeoPosAnimator anim;
        Vehicle vehicle = new Vehicle();
        GameObject InventoryPanel;
        InventoryUI inventoryUI;
        List<int>[] cellsInRange;
        Dictionary<string, int> climateCosts = new Dictionary<string, int>();
        readonly Dictionary<string, int> terrainCosts = new Dictionary<string, int>();
        public const int IMPASSABLE = 0;
        private const int STARTING_NUMBER_OF_TOURISTS = 2;
        private INavigationUI navigationUI;
        private ITouristManager touristManager;
        private ICameraManager cameraManager;
        private List<Landmark> landmarksInRange = new List<Landmark>();


        public override void Awake()
        {
            base.Awake();
            gameManager.TurnBasedObjects.Add(this);
        }

        public override void Start()
        {
            base.Start();
            navigationUI = uiManager.NavigationUI;
            cameraManager = gameManager.CameraManager;
            objectName = "player";
            //gameManager = GameManager.instance;
            map = WorldMapGlobe.instance;
            anim = gameObject.GetComponent(typeof(GeoPosAnimator)) as GeoPosAnimator;
            InventoryPanel = GameObject.Find("Canvas/InventoryPanel");
            inventoryUI = InventoryPanel.GetComponent(typeof(InventoryUI)) as InventoryUI;
            vehicle.InitVehicles();
            climateCosts = vehicle.GetClimateVehicle("Mild");
            cellsInRange = globeParser.GetCellsInRange(cellLocation, travelRange+1);
            cameraManager.OrientOnLocation(vectorLocation);
            touristManager = gameManager.TouristManager;

            //Generate Initial Tourists
            for (int i = 0; i < STARTING_NUMBER_OF_TOURISTS; i++)
            {
                touristManager.GenerateTourist();
            }
            UpdateLocation(cellLocation);
        }

        public float GetSize()
        {
            return size;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                cameraManager.OrientOnLocation(vectorLocation);
            }
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                stop = true;
            }
        }

        public override void OnMouseDown()
        {
            base.OnMouseDown();
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
                try
                {
                    pathIndices = DrawPath(cellLocation, index);
                }
                catch (Exception ex)
                {
                    errorHandler.catchException(ex);
                }


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
                if (moving)
                    stop = true;
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

        public void EndOfTurn(int turns)
        {
            /*
            // distanceTraveled = 0;
            //if (selected) 
            ClearCellCosts();
            Array.Clear(cellsInRange, 0, travelRange);
            cellsInRange = gameManager.GetCellsInRange(cellLocation, travelRange+1);
            //if (selected) 
            SetCellCosts();
            */
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
        public override void UpdateLocation(int newCellIndex)
        {
            //Update distance travelled
            int neighborIndex = map.GetCellNeighbourIndex(cellLocation, newCellIndex);

            base.UpdateLocation(newCellIndex);

            List<int>[] cellNeighbors = globeParser.GetCellsInRange(newCellIndex, 1);
            List<Landmark>[] landmarksInRangeTemp = globeParser.LandmarkParser.GetLandmarksInRange(newCellIndex, cellNeighbors);
            landmarksInRange.Clear();
            foreach (List<Landmark> landmarkList in landmarksInRangeTemp)
            {
                if(landmarkList != null)
                    landmarksInRange.AddRange(landmarkList);
            }

            List<MappableObject> mappableLandmarks = landmarksInRange.Cast<MappableObject>().ToList();

            navigationUI.UpdateNavigationDisplay(provincesOccupied, countriesOccupied, mappableLandmarks);
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
                if (!uiManager.CursorOverUI && globeManager.WorldGlobeMap.lastHighlightedCellIndex >= 0)
                {
                    OnCellEnter(globeManager.WorldGlobeMap.lastHighlightedCellIndex);
                }
            }

            ClearCellCosts();
            Array.Clear(cellsInRange, 0, travelRange);
            cellsInRange = globeParser.GetCellsInRange(cellLocation, travelRange + 1);
            SetCellCosts();

            moving = false;
            stop = false;
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
                    //if (map.cells[cell].tag != null)
                    if(map.cells[cell].occupants.Any() || map.cells[cell].occupants == null)
                    {
                       //Check if cell is occupied by something other than the player
                       //if(map.cells[cell].tag != GetInstanceID().ToString())
                       if(!map.cells[cell].occupants.Contains(this))
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
            inventoryUI.UpdateInventory(inventory);

            return true;
        }

        public void RemoveItem(int itemLocation)
        {
            inventory.RemoveAt(itemLocation);
            foreach (InventoryItem inventoryItem in inventory)
            {
                inventoryItem.inventoryLocation = inventory.IndexOf(inventoryItem);
            }
            inventoryUI.UpdateInventory(inventory);
            if(inventory.Count == 0)
            {
                touristManager.GenerateTourist();
            }
        }
    }
}
