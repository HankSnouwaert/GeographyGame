using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WPM
{
    public class LandmarkParser : MonoBehaviour, ILandmarkParser
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
    }
}
