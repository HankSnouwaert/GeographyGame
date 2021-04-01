using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WPM
{
    public class GlobeParser : MonoBehaviour, IGlobeParser
    {
        private GameManager gameManager;
        private WorldMapGlobe worldGlobeMap;
        public void Awake()
        {
            gameManager = FindObjectOfType<GameManager>();
            worldGlobeMap = gameManager.worldGlobeMap;
        }

        public List<int>[] GetCellsInRange(int startCell, int range = 0)
        {
            if (range < 0 || startCell < 0 || worldGlobeMap.cells.Count() < startCell)
            {
                //This will need to be replaced with an error message
                Debug.LogWarning("Invalid input for GetCellsInRange");
                return null;
            }

            int distance = 0;                           //distance measures how many rings of hexes we've moved out
            List<int>[] cells = new List<int>[range + 1]; //cells is an array of lists with each list other than 0 containing 
                                                          //the ring of hexes at that distance.  List 0 contains
                                                          //all hexes at every distance
                                                          //Add the startCell to List0
            cells[0] = new List<int>();
            cells[0].Add(startCell);
            worldGlobeMap.cells[startCell].flag = true;

            if (range > 0)
            {
                //Add the neighbors of the start cell to List1
                //And add them to List0
                distance++;
                cells[distance] = new List<int>();
                foreach (Cell neighbour in worldGlobeMap.GetCellNeighbours(startCell))
                {
                    cells[0].Add(neighbour.index);
                    cells[distance].Add(neighbour.index);
                    neighbour.flag = true;
                }
            }
            while (distance < range)
            {
                //Continue adding rings of hexes to List0 and creating a new
                //List for each ring until the distance checked equals the 
                //disired range
                distance++;
                cells[distance] = new List<int>();
                foreach (int cell in cells[distance - 1])
                {
                    foreach (Cell neighbour in worldGlobeMap.GetCellNeighbours(cell))
                    {
                        if (!neighbour.flag)
                        {
                            cells[0].Add(neighbour.index);
                            cells[distance].Add(neighbour.index);
                            neighbour.flag = true;
                        }
                    }
                }
            }
            //Lower all cell flags
            foreach (int cell in cells[0])
            {
                worldGlobeMap.cells[cell].flag = false;
            }
            return cells;
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

        /// <summary> 
        /// Get all countries within a certain range (measured in cells) of a target cell
        /// Inputs:
        ///     startCell:  Target cell the range is being measured from
        ///     range:      The range (in cells) out from startCell that the method increments through
        /// Outputs:
        ///     countryIndexes:  An array of lists, with ListX containing the countries reachable within
        ///                 X number of cells away from the target cell
        /// </summary>
        public List<int>[] GetCountriesInRange(int startCell, List<int>[] cellRange)
        {
            int range = cellRange.Length;

            if (range < 0 || startCell < 0 || worldGlobeMap.cells.Count() < startCell)
            {
                //This will need to be replaced with an error message
                Debug.LogWarning("Invalid input for GetProvincesInRange");
                return null;
            }

            List<int>[] countryIndexes = new List<int>[range + 1];      //provinces is an array of lists with each list containing 
            List<int> foundCountryIndexes = new List<int>();            //the provinces that can be reached at that distance. 

            List<int>[] provinceIndexes = GetProvincesInRange(startCell, cellRange);

            //Create lists of the countries within range based off of the provinces in range
            int i = 0;
            Province province;
            foreach (List<int> indexList in countryIndexes)
            {
                foreach (int provinceIndex in provinceIndexes[i])
                {
                    province = worldGlobeMap.provinces[provinceIndex];
                    if (!foundCountryIndexes.Contains(province.countryIndex))
                    {
                        foundCountryIndexes.Add(province.countryIndex);
                        countryIndexes[i].Add(province.countryIndex);
                    }
                }
                i++;
            }

            return countryIndexes;
        }

        /// <summary> 
        /// Get all landmarks within a certain range (measured in cells) of a target cell
        /// Inputs:
        ///     startCell:  Target cell the range is being measured from
        ///     range:      The range (in cells) out from startCell that the method increments through
        /// Outputs:
        ///     landmarks:  An array of lists, with ListX containing the landmarks within
        ///                 X number of cells from the target cell
        /// </summary>
        public List<Landmark>[] GetLandmarksInRange(int startCell, List<int>[] cellRange)
        {
            int range = cellRange.Length;

            if (range < 0 || startCell < 0 || worldGlobeMap.cells.Count() < startCell)
            {
                //This will need to be replaced with an error message
                Debug.LogWarning("Invalid input for GetCellsInRange");
                return null;
            }

            List<Landmark> landmarksFound = new List<Landmark>();
            List<Landmark> landmarksInCell;
            int distance = 0;                                           //distance measures how many rings of hexes we've moved out
            List<Landmark>[] landmarks = new List<Landmark>[range + 1];     //landmarks is an array of lists with each list containing 
            landmarks[0] = new List<Landmark>();                          //the landmarks that can be reached at that distance.  

            bool startHex = true;
            foreach (List<int> hexRing in cellRange)
            {
                if (startHex)
                {
                    //Get landmarks at start hex
                    landmarksInCell = GetLandmarksInCell(startCell);
                    startHex = false;
                    landmarks[0] = landmarksInCell;
                }
                else
                {
                    distance++;
                    landmarks[distance] = new List<Landmark>();
                    foreach (int cellIndex in hexRing)
                    {
                        landmarksInCell = GetLandmarksInCell(cellIndex);
                        foreach (Landmark landmark in landmarksInCell)
                        {
                            if (!landmarksFound.Contains(landmark))
                                landmarks[distance].Add(landmark);
                        }
                    }
                }
            }
            return landmarks;

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
        /// Get all countries that overlap with a given cell
        /// Inputs:
        ///     cellIndex:  Index of the cell in question
        /// Outputs:
        ///     countryIndexes:  An array of countries that overlap with the cell in quesiton
        /// </summary>
        public List<int> GetCountriesInCell(int cellIndex)
        {
            List<int> provinceIndexes = GetProvicesInCell(cellIndex);
            List<int> countryIndexes = GetCountriesFromProvinces(provinceIndexes);

            return countryIndexes;
        }

        /// <summary> 
        /// Get all landmarks located in a given cell
        /// Inputs:
        ///     cellIndex: Index of the cell in question
        /// Outputs:
        ///     landmarks: The landmarks in the cell in question
        /// </summary>
        public List<Landmark> GetLandmarksInCell(int cellIndex)
        {
            List<Landmark> landmarks = new List<Landmark>();
            foreach (MappableObject mappableObject in worldGlobeMap.cells[cellIndex].occupants)
            {
                if (mappableObject is Landmark)
                {
                    Landmark castedLandmark = mappableObject as Landmark;
                    landmarks.Add(castedLandmark);
                }
            }
            return landmarks;
        }

        /// <summary>
        ///  Given a list of province indexes, find all the countries they contain
        /// </summary>
        /// <param name="provinceIndexes"></param> The list of province indexes>
        /// <returns></returns> 
        public List<int> GetCountriesFromProvinces(List<int> provinceIndexes)
        {
            List<int> countryIndexes = new List<int>();
            int countryIndex;
            Province province;
            foreach (int provinceIndex in provinceIndexes)
            {
                province = worldGlobeMap.provinces[provinceIndex];
                countryIndex = province.countryIndex;
                if (!countryIndexes.Contains(countryIndex))
                {
                    countryIndexes.Add(countryIndex);
                }
            }
            return countryIndexes;
        }

    }
}
