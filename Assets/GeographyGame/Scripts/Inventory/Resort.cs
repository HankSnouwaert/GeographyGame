using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPM
{

    public class Resort : MappableObject
    {
        public string resortName;

        public void createInventoryItem()
        {

        }

        public override void OnCellClick(int index)
        {
            throw new System.NotImplementedException();
        }

        public override void OnCellEnter(int index)
        {
            throw new System.NotImplementedException();
        }

        public override void OnSelectableEnter(ISelectableObject selectableObject)
        {
            throw new System.NotImplementedException();
        }

        public override void OtherObjectSelected(ISelectableObject selectedObject)
        {
            throw new System.NotImplementedException();
        }
    }

}
