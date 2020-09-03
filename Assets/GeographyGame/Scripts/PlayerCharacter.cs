using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPM
{
    public class PlayerCharacter : MappableObject
    {
        int travelDistance = 10;

        void Start()
        {
            map = WorldMapGlobe.instance;
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
            DrawPath(cellLocation, index);
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
        }

        /// <summary>
        /// Draws a path between startCellIndex and endCellIndex
        /// </summary>
        /// <returns><c>true</c>, if path was found and drawn, <c>false</c> otherwise.</returns>
        /// <param name="startCellIndex">Start cell index.</param>
        /// <param name="endCellIndex">End cell index.</param>
        bool DrawPath(int startCellIndex, int endCellIndex)
        {

            List<int> cellIndices = map.FindPath(startCellIndex, endCellIndex, travelDistance);
            map.ClearCells(true, false, false);
            if (cellIndices == null)
                return false;   // no path found

            // Color starting cell, end cell and path
            map.SetCellColor(cellIndices, Color.gray, true);
            map.SetCellColor(startCellIndex, Color.green, true);
            map.SetCellColor(endCellIndex, Color.red, true);

            return true;
        }

    }
}
