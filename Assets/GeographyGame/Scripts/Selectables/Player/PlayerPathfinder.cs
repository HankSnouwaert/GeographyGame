using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WPM
{
    public class PlayerPathfinder : MonoBehaviour, IPathfinder
    {
        //Public Variables
        public int TravelRange { get; protected set; } = 10;
        public List<int> PathIndices { get; set; } = null;
        //Private Variables
        public List<Cell>[] CellsInRange { get; set; }
        //Constants
        public const int IMPASSABLE = 0;
        //Internal Reference Interfaces
        private IGeoPosAnimator geoPosAnimator;
        private IUIManager uiManager;
        private IGameManager gameManager;
        private IGlobeManager globeManager;
        private IGlobeParser globeParser;
        private WorldMapGlobe worldMapGlobe;
        private IPlayerManager playerManager;
        private IPlayerCharacter playerCharacter;
        //Error Checking
        private InterfaceFactory interfaceFactory;
        private IErrorHandler errorHandler;
        private bool componentMissing = false;

        void Awake()
        {
            interfaceFactory = FindObjectOfType<InterfaceFactory>();
            if (interfaceFactory == null)
                gameObject.SetActive(false);
            try
            {
                playerCharacter = GetComponent(typeof(IPlayerCharacter)) as IPlayerCharacter;
                if (playerCharacter == null)
                    componentMissing = true;

                geoPosAnimator = GetComponent(typeof(IGeoPosAnimator)) as IGeoPosAnimator;
                if (geoPosAnimator == null)
                    componentMissing = true;
            }
            catch
            {
                componentMissing = true;
            }
        }

        void Start()
        {
            errorHandler = interfaceFactory.ErrorHandler;
            gameManager = interfaceFactory.GameManager;
            globeManager = interfaceFactory.GlobeManager;
            uiManager = interfaceFactory.UIManager;
            if (errorHandler == null || gameManager == null || globeManager == null || uiManager == null)
                gameObject.SetActive(false);
            else
            {
                if (componentMissing == true)
                    errorHandler.ReportError("Pathfinder missing component", ErrorState.restart_scene);
                globeParser = globeManager.GlobeParser;
                if (globeParser == null)
                    errorHandler.ReportError("Globe Parser missing", ErrorState.restart_scene);
                worldMapGlobe = globeManager.WorldMapGlobe;
                if (worldMapGlobe == null)
                    errorHandler.ReportError("World Map Globe missing", ErrorState.restart_scene);
                playerManager = gameManager.PlayerManager;
                if (playerManager == null)
                    errorHandler.ReportError("Player Manager missing", ErrorState.restart_scene);

                CellsInRange = globeParser.GetCellsInRange(playerCharacter.CellLocation, TravelRange + 1);
            }
        }

        public List<int> FindPath(int startCellIndex, int endCellIndex)
        {
            worldMapGlobe.ClearCells(true, false, false);
            List<int> cellIndices = worldMapGlobe.FindPath(startCellIndex, endCellIndex);
            
            if (cellIndices == null)
                return null;   // no path found

            List<int> finalPath = new List<int>();
            finalPath.Add(cellIndices[0]);

            //Check that there is enough remaining movement to travel path
            //Start by getting the cost between the starting cell and the first cell in the path
            int neighborIndex = worldMapGlobe.GetCellNeighbourIndex(startCellIndex, cellIndices[0]);
            int pathCost = worldMapGlobe.GetCellNeighbourCost(startCellIndex, neighborIndex);
            int i = 0;
            //Get the cumlative cost for the rest of the path
            foreach (int cellIndex in cellIndices)
            {
                if (i < (cellIndices.Count - 1))
                {
                    neighborIndex = worldMapGlobe.GetCellNeighbourIndex(cellIndices[i], cellIndices[i + 1]);
                    pathCost = pathCost + worldMapGlobe.GetCellNeighbourCost(cellIndices[i], neighborIndex);
                    if (pathCost <= TravelRange)
                        finalPath.Add(cellIndices[i + 1]);
                    else
                        return finalPath;
                    i++;
                }
            }
            return finalPath;
        }

        public void ColorPath(List<int> cellIndices, int startCellIndex)
        {
            //if (pathCost >= TravelRange)
            //    worldMapGlobe.SetCellColor(cellIndices, Color.red, true);
           // else
            worldMapGlobe.SetCellColor(cellIndices, Color.grey, true);

            worldMapGlobe.SetCellColor(startCellIndex, Color.green, true);
        }

        /// <summary>
        /// Set the terrain costs of all the cells that are reachable by the player
        /// </summary>
        public void SetCellCosts()
        {
            if(CellsInRange == null)
            {
                errorHandler.ReportError("Cells in range empty", ErrorState.restart_scene);
                return;
            }
            foreach (Cell cell in CellsInRange[0]) 
            {
                try
                {
                    //Get Climate From Cell
                    int provinceIndex = worldMapGlobe.GetProvinceNearPoint(cell.sphereCenter);
                    Province province = worldMapGlobe.provinces[provinceIndex];
                    string climateAttribute = province.attrib["ClimateGroup"];

                    //Loop Through Each Neighbor and Set the Cost from the Neighbor to the Cell
                    if (climateAttribute != "")
                    {
                        bool cellOccupied = false;
                        //Check if cell is occupied by something other than the player
                        if (cell.occupants.Any() && !cell.occupants.Contains(playerCharacter))
                        {
                            cellOccupied = true;
                        }
                        int cost = playerCharacter.ClimateCosts[climateAttribute];
                        if (cost == IMPASSABLE || cellOccupied)
                        {
                            worldMapGlobe.SetCellCanCross(cell.index, false);
                        }
                        else
                        {
                            worldMapGlobe.SetCellCanCross(cell.index, true);
                            foreach (Cell neighbour in worldMapGlobe.GetCellNeighbours(cell.index))
                            {
                                worldMapGlobe.SetCellNeighbourCost(neighbour.index, cell.index, cost, false);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    errorHandler.CatchException(ex, ErrorState.close_window);
                }
            }
        }

        /// <summary>
        /// Clear all the terrain costs that were set by the player
        /// </summary>
        public void ClearCellCosts()
        {
            if (CellsInRange == null)
            {
                errorHandler.ReportError("Cells in range empty", ErrorState.restart_scene);
                return;
            }
            foreach (Cell cell in CellsInRange[0])
            {
                worldMapGlobe.SetCellCanCross(cell.index, true);
                foreach (Cell neighbour in worldMapGlobe.GetCellNeighbours(cell.index))
                {
                    worldMapGlobe.SetCellNeighbourCost(neighbour.index, cell.index, 0, false);
                }
            }
        }

        public void FinishedPathFinding()
        {
            PathIndices.Clear();
            worldMapGlobe.ClearCells(true, false, false);
            if (playerCharacter.Selected)
            {
                if(playerCharacter.CellLocation == null)
                {
                    errorHandler.ReportError("Player finished pathfinding in invalid location", ErrorState.restart_scene);
                    return;
                }
                worldMapGlobe.SetCellColor(playerCharacter.CellLocation.index, Color.green, true);
                if (!uiManager.CursorOverUI && worldMapGlobe.lastHighlightedCellIndex >= 0)
                {
                    playerCharacter.OnCellEnter(worldMapGlobe.lastHighlightedCellIndex);
                }
            }

            ClearCellCosts();
            Array.Clear(CellsInRange, 0, CellsInRange.Length);
            CellsInRange = globeParser.GetCellsInRange(playerCharacter.CellLocation, TravelRange + 1);
            SetCellCosts();

            geoPosAnimator.Moving = false;
            geoPosAnimator.Stop = false;
        }
    }

}

