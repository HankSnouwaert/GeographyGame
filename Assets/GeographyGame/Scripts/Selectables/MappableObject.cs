using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPM
{
    public class MappableObject : SelectableObject
    {
        public int cellLocation;
        public Vector3 vectorLocation;
        public Vector2[] latlon;
        /*
        protected CellManager cellManager;

        public override void Start()
        {
            cellManager = FindObjectOfType<CellManager>();
        }


        public override void OnMouseExit()
        {
            cellManager.HandleOnCellEnter(cellLocation);
        }
        */
    }
}
