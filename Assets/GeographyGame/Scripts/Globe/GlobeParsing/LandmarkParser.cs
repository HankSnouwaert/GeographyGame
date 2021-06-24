using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WPM
{
    public class LandmarkParser : MonoBehaviour, ILandmarkParser
    {
        //Internal Reference Interfaces
        private IGlobeManager globeManager;
        private WorldMapGlobe worldMapGlobe;
        private IGlobeParser globeParser;
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
                if(worldMapGlobe == null)
                    errorHandler.ReportError("World Map Globe missing", ErrorState.restart_scene);
                globeParser = globeManager.GlobeParser;
                if (globeParser == null)
                    errorHandler.ReportError("Globe Parser missing", ErrorState.restart_scene);
            } 
        }

        public List<Landmark> GetLandmarksInCell(Cell cell)
        {
            List<Landmark> landmarks = new List<Landmark>();
            try
            {
                foreach (MappableObject mappableObject in cell.occupants)
                {
                    if (mappableObject is Landmark)
                    {
                        Landmark castedLandmark = mappableObject as Landmark;
                        landmarks.Add(castedLandmark);
                    }
                }
            }
            catch(System.Exception ex)
            {
                errorHandler.CatchException(ex, ErrorState.close_window);
                return null;
            }
            return landmarks;
        }

        public List<Landmark>[] GetLandmarksInRange(Cell startCell, int range)
        {
            List<Cell>[] cellsInRange = globeParser.GetCellsInRange(startCell, range);

            if (range < 0 || startCell.index < 0 || worldMapGlobe.cells.Count() < startCell.index)
            {
                errorHandler.ReportError("Invalid input for GetLandmarksInCell", ErrorState.close_window);
                return null;
            }

            List<Landmark> landmarksFound = new List<Landmark>();
            List<Landmark> landmarksInCell;
            int distance = 0;                                           //distance measures how many rings of hexes we've moved out
            List<Landmark>[] landmarks = new List<Landmark>[range + 1];     //landmarks is an array of lists with each list containing 
            landmarks[0] = new List<Landmark>();                          //the landmarks that can be reached at that distance.  

            try
            {
                bool startHex = true;
                foreach (List<Cell> hexRing in cellsInRange)
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
                        foreach (Cell cell in hexRing)
                        {
                            landmarksInCell = GetLandmarksInCell(cell);
                            foreach (Landmark landmark in landmarksInCell)
                            {
                                if (!landmarksFound.Contains(landmark))
                                    landmarks[distance].Add(landmark);
                            }
                        }
                    }
                }
            }
            catch(System.Exception ex)
            {
                errorHandler.CatchException(ex, ErrorState.close_window);
            }

            return landmarks;

        }
    }
}
