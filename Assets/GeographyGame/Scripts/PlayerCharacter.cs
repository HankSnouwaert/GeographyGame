using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPM
{
    public class PlayerCharacter : MappableObject
    {
        int travelDistance = 5;
        int distanceTraveled = 0;
        public int destination = 0;
        public List<int> pathIndices = null;
        public float size = 0.005f;
        GeoPosAnimator anim;
        Vehicle vehicle = new Vehicle();
        bool moving = false;

        Dictionary<string, int> climateCosts = new Dictionary<string, int>();
        Dictionary<string, int> terrainCosts = new Dictionary<string, int>();

        void Start()
        {
            map = WorldMapGlobe.instance;
            anim = gameObject.GetComponent(typeof(GeoPosAnimator)) as GeoPosAnimator;
            vehicle.InitVehicles();
            climateCosts = vehicle.GetClimateVehicle("Mild");
            SetCellCosts();
        }

        public override void Selected()
        {
            map.SetCellColor(cellLocation, Color.green, true);
        }

        public override void OnCellEnter(int index)
        {
            if (!anim.auto)
            {
                //Attempt to display path to new location
                map.ClearCells(true, false, false);
                map.SetCellColor(cellLocation, Color.green, true);
                pathIndices = DrawPath(cellLocation, index);
                if (pathIndices != null)
                {
                    pathIndices.Insert(0, cellLocation);
                }
            }
        }

        public override void OnCellClick(int index)
        {
            if (index == cellLocation)
            {
                //The player was clicked while selected
                map.ClearCells(true, false, false);
                selected = false;
            }
            //Attempt to move to new location
            else if (pathIndices != null && moving == false)
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

        public override void EndOfTurn()
        {
            distanceTraveled = 0;
        }

        /// <summary>
        /// Draws a path between startCellIndex and endCellIndex
        /// </summary>
        /// <returns><c>true</c>, if path was found and drawn, <c>false</c> otherwise.</returns>
        /// <param name="startCellIndex">Start cell index.</param>
        /// <param name="endCellIndex">End cell index.</param>
        List<int> DrawPath(int startCellIndex, int endCellIndex)
        {
            int debug = cellLocation;
            int remainingMovement = travelDistance - distanceTraveled - 1;
            List<int> cellIndices = map.FindPath(startCellIndex, endCellIndex, remainingMovement);
            map.ClearCells(true, false, false);
            if (cellIndices == null)
                return null;   // no path found

            // Color starting cell, end cell and path
            map.SetCellColor(cellIndices, Color.gray, true);
            map.SetCellColor(startCellIndex, Color.green, true);
            map.SetCellColor(endCellIndex, Color.red, true);

            return cellIndices;
        }

        public void UpdateLocation(int newCellIndex)
        {
            map.cells[cellLocation].tag = null;
            cellLocation = newCellIndex;
            map.cells[cellLocation].tag = GetInstanceID().ToString();
            vectorLocation = map.cells[cellLocation].sphereCenter;
        }

        public void FinishedPathFinding()
        {
            distanceTraveled = distanceTraveled + (pathIndices.Count);
            pathIndices.Clear();
            UpdateLocation(destination);
            moving = false;
        }

        public void SetCellCosts()
        {
            foreach(Cell cell in map.cells)
            {
                //Get Cell Attributes from Province
                int provinceIndex = map.GetProvinceNearPoint(cell.sphereCenter);
                Province province = map.provinces[provinceIndex];

                //Loop Through Each Neighbor and Set the Cost from the Neighbor to the Cell
                string climateAttribute = province.attrib["ClimateGroup"];
                if (climateAttribute != "")
                {
                    int cost = climateCosts[climateAttribute];
                    //map.SetCellCanCross(cell.index, true);
                    //map.SetCellNeighboursCost(cell.index, cost, false);
                    if (cost == 0)  //Need to Change this to CONST
                    {
                        map.SetCellCanCross(cell.index, false);
                    }
                    else
                    {
                    map.SetCellCanCross(cell.index, true);
                        foreach (Cell neighbour in map.GetCellNeighbours(cell.index))
                        {
                            map.SetCellNeighbourCost(neighbour.index, cell.index, cost, false);
                        }
                    }
                }
            }
        }
    }
}
