using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WPM
{
    public class SelectableObject : MonoBehaviour
    {
        public bool selected;
        protected WorldMapGlobe map;
        // Start is called before the first frame update

        public virtual void Selected()
        {

        }

        public virtual void OnCellEnter(int index)
        {

        }

        public virtual void OnCellClick(int index)
        {

        }
    }
}
