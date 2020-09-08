using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPM
{
    public class PlayerCharacter : MappableObject
    {
        int travelDistance = 10;
        public int destination = 0;
        public List<int> pathIndices = null;
        public float size = 0.005f;
        GeoPosAnimator anim;

        void Start()
        {
            map = WorldMapGlobe.instance;
            anim = gameObject.GetComponent(typeof(GeoPosAnimator)) as GeoPosAnimator;
        }

        public override void Selected()
        {
            map.SetCellColor(cellLocation, Color.green, true);
        }

        public override void OnCellEnter(int index)
        {
            //Attempt to display path to new location
            map.ClearCells(true, false, false);
            map.SetCellColor(cellLocation, Color.green, true);
            pathIndices = DrawPath(cellLocation, index);
            if(pathIndices != null)
            {
                pathIndices.Insert(0, cellLocation);
            } 
        }

        public override void OnCellClick(int index)
        {
            if(index == cellLocation)
            {
                //The player was clicked while selected
                map.ClearCells(true, false, false);
                selected = false;
            }
            //Attempt to move to new location
            if(pathIndices != null)
            {
                destination = index;
                //Add latlon of each hex in path to animator's path
                anim.GenerateLatLon(pathIndices);
                // Compute path length
                anim.ComputePath();
                anim.auto = true;
                
            }
        }

        /// <summary>
        /// Draws a path between startCellIndex and endCellIndex
        /// </summary>
        /// <returns><c>true</c>, if path was found and drawn, <c>false</c> otherwise.</returns>
        /// <param name="startCellIndex">Start cell index.</param>
        /// <param name="endCellIndex">End cell index.</param>
        List<int> DrawPath(int startCellIndex, int endCellIndex)
        {

            List<int> cellIndices = map.FindPath(startCellIndex, endCellIndex, travelDistance);
            map.ClearCells(true, false, false);
            if (cellIndices == null)
                return null;   // no path found

            // Color starting cell, end cell and path
            map.SetCellColor(cellIndices, Color.gray, true);
            map.SetCellColor(startCellIndex, Color.green, true);
            map.SetCellColor(endCellIndex, Color.red, true);

            return cellIndices;
        }

        public void FinishedPathFinding()
        {
            map.cells[cellLocation].tag = null;
            cellLocation = destination;
            map.cells[cellLocation].tag = GetInstanceID().ToString();
            vectorLocation = map.cells[cellLocation].sphereCenter;
        }

    }
}
