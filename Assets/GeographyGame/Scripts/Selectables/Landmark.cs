using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPM
{
    public class Landmark : MappableObject, ILandmark
    {
        public MountPoint MountPoint { get; set; }

        protected override void OnMouseDown()
        {
            //Landmarks do nothing when clicked
        }

        public override void OnCellClick(int index)
        {
            
        }

        public override void OnCellEnter(int index)
        {
            
        }

        public override void OnSelectableEnter(ISelectableObject selectableObject)
        {
            
        }

        public override void OtherObjectSelected(ISelectableObject selectedObject)
        {
            
        }

    }

}