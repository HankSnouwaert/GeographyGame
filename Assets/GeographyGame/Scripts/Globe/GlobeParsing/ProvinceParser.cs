using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WPM
{
    public class ProvinceParser : MonoBehaviour, IProvinceParser
    {
        private GlobeManager globeManager;
        private WorldMapGlobe worldGlobeMap;
        private void Awake()
        {
            globeManager = FindObjectOfType<GlobeManager>(); 
        }

        private void Start()
        {
            worldGlobeMap = globeManager.WorldGlobeMap;
        }

        /// <summary> 
        /// Get all provinces that overlap with a given cell
        /// Inputs:
        ///     cellIndex:  Index of the cell in question
        /// Outputs:
        ///     provinces:  An array of provinces that overlap with the cell in quesiton
        /// </summary>
        public List<int> GetProvicesInCell(int cellIndex)
        {
            List<int> foundProvinceIndexes = new List<int>();
            List<int> checkedProvinceIndexes = new List<int>();
            int provinceIndex;
            int countryIndex;
            int neighborIndex;

            //Create a list of points including each vertex of the cell and its center point
            List<Vector3> cellPoints = worldGlobeMap.cells[cellIndex].vertices.ToList();
            cellPoints.Add(worldGlobeMap.cells[cellIndex].sphereCenter);

            foreach (Vector3 cellPoint in cellPoints)
            {
                provinceIndex = worldGlobeMap.GetProvinceIndex(cellPoint);
                //Check if cell point is on a province
                if (provinceIndex == -1)
                {
                    //Get closest province to point if it is not centered on one
                    provinceIndex = worldGlobeMap.GetProvinceNearPoint(cellPoint);
                }
                //Add the province, if it is not already in the province list
                if (!foundProvinceIndexes.Contains(provinceIndex))
                    foundProvinceIndexes.Add(provinceIndex);

                //Check to see if neighbours of province overlap with cell
                List<Province> provinceNeighbours = worldGlobeMap.ProvinceNeighbours(provinceIndex);
                bool provinceOverlaps;
                foreach (Province neighbor in provinceNeighbours)
                {
                    countryIndex = neighbor.countryIndex;
                    neighborIndex = worldGlobeMap.GetProvinceIndex(countryIndex, neighbor.name);
                    //Make sure you haven't already checked the province, this saves time
                    if (!checkedProvinceIndexes.Contains(neighborIndex))
                    {
                        checkedProvinceIndexes.Add(neighborIndex);
                        provinceOverlaps = false;

                        foreach (Region region in neighbor.regions)
                        {
                            foreach (Vector3 spherePoint in region.spherePoints)
                            {
                                if (worldGlobeMap.GetCellIndex(spherePoint) == cellIndex)
                                {
                                    provinceOverlaps = true;
                                    break;
                                }
                            }
                            if (provinceOverlaps)
                                break;
                        }
                        if (provinceOverlaps && !foundProvinceIndexes.Contains(provinceIndex))
                        {
                            foundProvinceIndexes.Add(neighborIndex);
                        }
                    }
                }
            }

            return foundProvinceIndexes;
        }

        /// <summary> 
        /// Get all provinces within a certain range (measured in cells) of a target cell
        /// Inputs:
        ///     startCell:  Target cell the range is being measured from
        ///     range:      The range (in cells) out from startCell that the method increments through
        /// Outputs:
        ///     provinces:  An array of lists, with ListX containing the provinces reachable within
        ///                 X number of cells away from the target cell
        /// </summary>
        public List<int>[] GetProvincesInRange(int startCell, List<int>[] cellRange)
        {
            int range = cellRange.Length;

            if (range < 0 || startCell < 0 || worldGlobeMap.cells.Count() < startCell)
            {
                //This will need to be replaced with an error message
                Debug.LogWarning("Invalid input for GetProvincesInRange");
                return null;
            }

            int distance = 0;                                      //distance measures how many rings of hexes we've moved out
            List<int>[] provinces = new List<int>[range + 1];      //provinces is an array of lists with each list containing 
            List<int> foundProvinces = new List<int>();             //the provinces that can be reached at that distance.  
            List<int> provincesInHex = new List<int>();

            bool startHex = true;
            foreach (List<int> hexRing in cellRange)
            {
                if (startHex)
                {
                    //Get provinces at start hex
                    provinces[0] = new List<int>();
                    provincesInHex = GetProvicesInCell(startCell);
                    foreach (int provinceIndex in provincesInHex)
                    {
                        foundProvinces.Add(provinceIndex);
                        provinces[0].Add(provinceIndex);
                    }
                    startHex = false;
                }
                else
                {
                    distance++;
                    provinces[distance] = new List<int>();
                    foreach (int cellIndex in hexRing)
                    {
                        //Check if there is a path from the start cell to this one
                        if (worldGlobeMap.FindPath(startCell, cellIndex) != null)
                        {
                            provincesInHex = GetProvicesInCell(cellIndex);
                            foreach (int provinceIndex in provincesInHex)
                            {
                                //Check that this province hasn't already been added
                                if (!foundProvinces.Contains(provinceIndex))
                                {
                                    foundProvinces.Add(provinceIndex);
                                    provinces[distance].Add(provinceIndex);
                                }
                            }
                        }
                    }
                }
            }
            return provinces;
        }
    }
}
