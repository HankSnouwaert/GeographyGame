using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPM
{
    public class Landmark : MappableObject, ILandmark
    {
        public MountPoint MountPoint { get; set; }

        public Outline Outline { get; set; }

        protected override void Awake()
        {
            base.Awake();
            Outline = GetComponentInChildren(typeof(Outline)) as Outline;
            if (Outline != null)
                Outline.enabled = false;
        }

        protected override void Start()
        {
            base.Start();
            selectionEnabled = false;
        }

        protected override void OnMouseDown()
        {
            
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