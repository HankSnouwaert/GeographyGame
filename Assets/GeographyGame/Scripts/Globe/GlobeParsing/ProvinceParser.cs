using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WPM
{
    public class ProvinceParser : MonoBehaviour, IProvinceParser
    {
        //Internal Interface References
        private IGlobeManager globeManager;
        private IGlobeParser globeParser;
        private WorldMapGlobe worldMapGlobe;
        //Error Checking
        private InterfaceFactory interfaceFactory;
        private IErrorHandler errorHandler;

        private void Awake()
        {
            interfaceFactory = FindObjectOfType<InterfaceFactory>();
            if (interfaceFactory == null)
                gameObject.SetActive(false);
        }

        private void Start()
        {
            globeManager = interfaceFactory.GlobeManager;
            errorHandler = interfaceFactory.ErrorHandler;
            if (globeManager == null || errorHandler == null)
                gameObject.SetActive(false);
            else
            {
                worldMapGlobe = globeManager.WorldMapGlobe;
                if (worldMapGlobe == null)
                    errorHandler.ReportError("World Map Globe missing", ErrorState.restart_scene);
                globeParser = globeManager.GlobeParser;
                if (globeParser == null)
                    errorHandler.ReportError("Globe Parser missing", ErrorState.restart_scene);
            }
        }

        public List<Province> GetProvicesInCell(Cell cell)
        {
            List<Province> foundProvinces= new List<Province>();
            List<Province> checkedProvinces = new List<Province>();
            int provinceIndex;

            try
            {
                //Create a list of points including each vertex of the cell and its center point
                List<Vector3> cellPoints = cell.vertices.ToList();
                cellPoints.Add(cell.sphereCenter);

                foreach (Vector3 cellPoint in cellPoints)
                {
                    provinceIndex = worldMapGlobe.GetProvinceIndex(cellPoint);
                    //Check if cell point is on a province
                    if (provinceIndex == -1)
                    {
                        //Get closest province to point if it is not centered on one
                        provinceIndex = worldMapGlobe.GetProvinceNearPoint(cellPoint);
                    }
                    //Add the province, if it is not already in the province list
                    Province province = worldMapGlobe.provinces[provinceIndex];
                    if (!foundProvinces.Contains(province))
                        foundProvinces.Add(province);

                    //Check to see if neighbours of province overlap with cell
                    List<Province> provinceNeighbours = worldMapGlobe.ProvinceNeighbours(provinceIndex);
                    bool provinceOverlaps;
                    foreach (Province neighbor in provinceNeighbours)
                    {
                        //countryIndex = neighbor.countryIndex;
                        //neighborIndex = worldMapGlobe.GetProvinceIndex(countryIndex, neighbor.name);

                        //Make sure you haven't already checked the province, this saves time
                        if (!checkedProvinces.Contains(neighbor))
                        {
                            checkedProvinces.Add(neighbor);
                            provinceOverlaps = false;

                            foreach (Region region in neighbor.regions)
                            {
                                foreach (Vector3 spherePoint in region.spherePoints)
                                {
                                    if (worldMapGlobe.GetCellIndex(spherePoint) == cell.index)
                                    {
                                        provinceOverlaps = true;
                                        break;
                                    }
                                }
                                if (provinceOverlaps)
                                    break;
                            }
                            if (provinceOverlaps && !foundProvinces.Contains(province))
                            {
                                foundProvinces.Add(neighbor);
                            }
                        }
                    }
                }
            }
            catch(System.Exception ex)
            {
                errorHandler.CatchException(ex, ErrorState.close_window);
                return null;
            }

            return foundProvinces;
        }

        public List<Province>[] GetProvincesInRange(Cell startCell, int range)
        {
            if (range < 0 || startCell.index < 0 || worldMapGlobe.cells.Count() < startCell.index)
            {
                errorHandler.ReportError("Invalid input for GetProvincesInRange", ErrorState.close_window);
                return null;
            }

            List<Cell>[] cellsInRange = globeParser.GetCellsInRange(startCell, range);

            int distance = 0;                                               //distance measures how many rings of hexes we've moved out
            List<Province>[] provinces = new List<Province>[range + 1];      //provinces is an array of lists with each list containing 
            List<Province> foundProvinces = new List<Province>();             //the provinces that can be reached at that distance.  
            List<Province> provincesInHex = new List<Province>();

            try
            { 
                bool startHex = true;
                foreach (List<Cell> hexRing in cellsInRange)
                {
                    if (startHex)
                    {
                        //Get provinces at start hex
                        provinces[0] = new List<Province>();
                        provincesInHex = GetProvicesInCell(startCell);
                        foreach (Province province in provincesInHex)
                        {
                            foundProvinces.Add(province);
                            provinces[0].Add(province);
                        }
                        startHex = false;
                    }
                    else
                    {
                        distance++;
                        provinces[distance] = new List<Province>();
                        foreach (Cell cell in hexRing)
                        {
                            //Check if there is a path from the start cell to this one
                            if (worldMapGlobe.FindPath(startCell.index, cell.index) != null)
                            {
                                provincesInHex = GetProvicesInCell(cell);
                                foreach (Province province in provincesInHex)
                                {
                                    //Check that this province hasn't already been added
                                    if (!foundProvinces.Contains(province))
                                    {
                                        foundProvinces.Add(province);
                                        provinces[distance].Add(province);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch(System.Exception ex)
            {
                errorHandler.CatchException(ex, ErrorState.close_window);
                return null;
            }
            return provinces;
        }
    }
}
