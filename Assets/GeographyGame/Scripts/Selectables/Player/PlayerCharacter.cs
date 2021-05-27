using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WPM
{
    public class PlayerCharacter : MappableObject, ITurnBasedObject, IPlayerCharacter
    {
        //Public Variables
        public Dictionary<string, int> ClimateCosts { get; protected set; } = new Dictionary<string, int>();
        public Dictionary<string, int> TerrainCosts { get; protected set; }  = new Dictionary<string, int>();

        private List<ILandmark> landmarksInRange = new List<ILandmark>();  //MAKE THIS PUBLIC
        //Constants
        private const int STARTING_NUMBER_OF_TOURISTS = 2;
        //Public Interface References
        public IPathfinder Pathfinder { get; protected set; }
        public IInventory Inventory { get; protected set; }
        //Internal Interface References
        public Vehicle Vehicle { get; set; } = new Vehicle();
        IGeoPosAnimator geoPosAnimator;
        //GameObject InventoryPanel;
        private INavigationUI navigationUI;
        private ITouristManager touristManager;
        private ICameraManager cameraManager;
        private ITurnsManager turnsManager;
        private ILandmarkParser landmarkParser;
        
        //Error Checking
        private bool componentMissing = false;

        protected override void Awake()
        {
            base.Awake();
            try
            { 
                Pathfinder = GetComponent(typeof(IPathfinder)) as IPathfinder;
                if (Pathfinder == null)
                    componentMissing = true;

                Inventory = GetComponent(typeof(IInventory)) as IInventory;
                if (Inventory == null)
                    componentMissing = true;

                geoPosAnimator = GetComponent(typeof(IGeoPosAnimator)) as IGeoPosAnimator;
                if (geoPosAnimator == null)
                    componentMissing = true;
            }
            catch
            {
                componentMissing = true;
            }
            Size = 0.0003f;
            ObjectName = "player";
        }
        
        protected override void Start()
        {
            base.Start();
            if (gameObject.activeSelf)
            {
                if (componentMissing == true)
                    errorHandler.ReportError("Player Character missing component", ErrorState.restart_scene);
                else
                {
                    landmarkParser = globeParser.LandmarkParser;
                    if (landmarkParser == null)
                        errorHandler.ReportError("Landmark parser missing", ErrorState.restart_scene);

                    navigationUI = uiManager.NavigationUI;
                    if (navigationUI == null)
                        errorHandler.ReportError("Navigation UI missing", ErrorState.restart_scene);

                    touristManager = gameManager.TouristManager;
                    if (touristManager == null)
                        errorHandler.ReportError("Tourist Manager missing", ErrorState.restart_scene);
                    else
                    {
                        for (int i = 0; i < STARTING_NUMBER_OF_TOURISTS; i++)
                        {
                            touristManager.GenerateTourist();
                        }
                    }

                    cameraManager = gameManager.CameraManager;
                    if (cameraManager == null)
                        errorHandler.ReportError("Camera Manager missing", ErrorState.restart_scene);
                    else
                        cameraManager.OrientOnLocation(VectorLocation);

                    turnsManager = gameManager.TurnsManager;
                    if (turnsManager == null)
                        errorHandler.ReportError("Turns Manager missing", ErrorState.restart_scene);
                    else
                        turnsManager.TurnBasedObjects.Add(this);

                    Vehicle.InitVehicles();
                    ClimateCosts = Vehicle.GetClimateVehicle("Mild");
                    UpdateLocation(CellLocation.index);
                }
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                cameraManager.OrientOnLocation(VectorLocation);
            }
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                geoPosAnimator.Stop = true;
            }
        }

        public override void Select()
        {
            if(CellLocation == null)
            {
                errorHandler.ReportError("Invalid cell location", ErrorState.close_window);
                return;
            }
            base.Select();
            worldMapGlobe.SetCellColor(CellLocation.index, Color.green, true);
            Pathfinder.SetCellCosts();
        }

        public override void Deselect()
        {
            base.Deselect();
            Pathfinder.ClearCellCosts();
            worldMapGlobe.ClearCells(true, false, false);
        }

        public override void OnCellEnter(int index)
        {
            if (!geoPosAnimator.Auto)
            {
                //Attempt to display path to new location
                worldMapGlobe.ClearCells(true, false, false);
                List<int> newPath = Pathfinder.FindPath(CellLocation.index, index);
                if(newPath != null)
                {
                    Pathfinder.PathIndices = newPath;
                    Pathfinder.PathIndices.Insert(0, CellLocation.index);
                }
                Pathfinder.ColorPath(Pathfinder.PathIndices, CellLocation.index);
                //worldMapGlobe.SetCellColor(CellLocation.index, Color.green, true);
            }
        }

        public override void OnCellClick(int index)
        {
            if (index == CellLocation.index)
            {
                if (geoPosAnimator.Moving)
                    geoPosAnimator.Stop = true;
            }
            //Attempt to move to new location
            else
            {
                if (Pathfinder.PathIndices != null && geoPosAnimator.Moving == false)
                {
                    geoPosAnimator.InitiateMovement(Pathfinder.PathIndices);
                }
            } 
        }

        public void EndOfTurn(int turns)
        {

        }

        public override void UpdateLocation(int newCellIndex)
        {
            if(newCellIndex < 0 || newCellIndex >= worldMapGlobe.cells.Length)
            {
                errorHandler.ReportError("Invalid cell index", ErrorState.close_window);
                return;
            }
            Cell newCell = worldMapGlobe.cells[newCellIndex];

            int neighborIndex = worldMapGlobe.GetCellNeighbourIndex(CellLocation.index, newCellIndex);
            
            base.UpdateLocation(newCellIndex);
            UpdateLandmarksInRange(newCell);
            /*
            List<Landmark>[] landmarksInRangeTemp = landmarkParser.GetLandmarksInRange(newCell, 1);
            //landmarksInRange.Clear();
            ClearLandmarksInRange();
            foreach (List<Landmark> landmarkList in landmarksInRangeTemp)
            {
                if(landmarkList != null)
                    landmarksInRange.AddRange(landmarkList);
            }
            foreach(Landmark landmark in landmarksInRange)
            {
                if (landmark.Outline != null)
                    landmark.Outline.enabled = true;
            }
            */
            List<IMappableObject> mappableLandmarks = landmarksInRange.Cast<IMappableObject>().ToList();

            navigationUI.UpdateNavigationDisplay(ProvincesOccupied, CountriesOccupied, mappableLandmarks);

            //If your previous location was a neighbor, use the cell crossing cost to update the game's turns
            if (neighborIndex >= 0)
            {
                int turns = worldMapGlobe.GetCellNeighbourCost(CellLocation.index, neighborIndex);
                turnsManager.NextTurn(turns);
            }
            
        }

        public override void OnSelectableEnter(ISelectableObject selectableObject)
        {
            //Nothing Happens
        }
        
        public override void OtherObjectSelected(ISelectableObject selectedObject)
        {
            //There will need to be check later to account for multiple object selection
        }

        private void ClearLandmarksInRange()
        {
            foreach(Landmark landmark in landmarksInRange)
            {
                if(landmark.Outline != null)
                {
                    landmark.Outline.enabled = false;
                }
            }
            landmarksInRange.Clear();
        }

        private void UpdateLandmarksInRange(Cell newCellLocation)
        {
            List<Landmark>[] landmarksInRangeTemp = landmarkParser.GetLandmarksInRange(newCellLocation, 1);
            //landmarksInRange.Clear();
            ClearLandmarksInRange();
            foreach (List<Landmark> landmarkList in landmarksInRangeTemp)
            {
                if (landmarkList != null)
                    landmarksInRange.AddRange(landmarkList);
            }
            foreach (Landmark landmark in landmarksInRange)
            {
                if (landmark.Outline != null)
                    landmark.Outline.enabled = true;
            }
        }
    }
}
