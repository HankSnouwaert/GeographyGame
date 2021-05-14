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
        public int TravelRange { get; protected set; } = 30;
        public IPlayerCharacter PlayerCharacter { get; protected set; }
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
        //private IPlayerCharacter playerCharacter;
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
                PlayerCharacter = GetComponent(typeof(IPlayerCharacter)) as IPlayerCharacter;
                if (PlayerCharacter == null)
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

                
                CellsInRange = globeParser.GetCellsInRange(PlayerCharacter.CellLocation, TravelRange + 1);
            }
        }

        public List<int> FindPath(int startCellIndex, int endCellIndex)
        {
            List<int> cellIndices;
            cellIndices = worldMapGlobe.FindPath(startCellIndex, endCellIndex);
            worldMapGlobe.ClearCells(true, false, false);

            if (cellIndices == null)
                return null;   // no path found

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
                    i++;
                }
            }

            if (pathCost > TravelRange)
                return null;   //Path costs more movement than is available

            //map.SetCellColor(endCellIndex, Color.red, true);

            //Path Successful
            // Color starting cell, end cell and path
            if (pathCost == TravelRange)
                worldMapGlobe.SetCellColor(cellIndices, Color.red, true);
            else
                worldMapGlobe.SetCellColor(cellIndices, Color.grey, true);
            worldMapGlobe.SetCellColor(startCellIndex, Color.green, true);

            return cellIndices;
        }

        /// <summary>
        /// Set the terrain costs of all the cells that are reachable by the player
        /// </summary>
        public void SetCellCosts()
        {
            foreach (Cell cell in CellsInRange[0]) 
            {
                //Get Cell Attributes from Province
                int provinceIndex = worldMapGlobe.GetProvinceNearPoint(cell.sphereCenter);
                Province province = worldMapGlobe.provinces[provinceIndex];

                //Loop Through Each Neighbor and Set the Cost from the Neighbor to the Cell
                string climateAttribute = province.attrib["ClimateGroup"];
                if (climateAttribute != "")
                {
                    bool cellOccupied = false;
                    //Check if cell is occupied
                    //if (map.cells[cell].tag != null)
                    if (cell.occupants.Any() || cell.occupants == null)
                    {
                        //Check if cell is occupied by something other than the player
                        //if(map.cells[cell].tag != GetInstanceID().ToString())
                        if (!cell.occupants.Contains(PlayerCharacter))
                        {
                            cellOccupied = true;
                        }
                    }
                    int cost = PlayerCharacter.ClimateCosts[climateAttribute];
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
        }

        /// <summary>
        /// Clear all the terrain costs that were set by the player
        /// </summary>
        public void ClearCellCosts()
        {
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
            if (PlayerCharacter.Selected)
            {
                worldMapGlobe.SetCellColor(PlayerCharacter.CellLocation.index, Color.green, true);
                if (!uiManager.CursorOverUI && globeManager.WorldMapGlobe.lastHighlightedCellIndex >= 0)
                {
                    PlayerCharacter.OnCellEnter(globeManager.WorldMapGlobe.lastHighlightedCellIndex);
                }
            }

            ClearCellCosts();
            Array.Clear(CellsInRange, 0, TravelRange);
            CellsInRange = globeParser.GetCellsInRange(PlayerCharacter.CellLocation, TravelRange + 1);
            SetCellCosts();

            geoPosAnimator.Moving = false;
            geoPosAnimator.Stop = false;
        }

    }

}

