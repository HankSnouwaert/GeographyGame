using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace WPM
{

    public class InventoryItem : SelectableObject
    {
        public Sprite inventoryIcon;
        //public Color selectedColor = new Color(194, 194, 194, 0);
        //public Color normalColor = new Color(255, 255, 255, 255);
        public int inventorySpace = 1;
        public int inventoryLocation;
        protected InventoryGUI inventoryGUI;
        protected SpriteRenderer spriteRenderer; 

        public override void Start()
        {
            base.Start();
            inventoryGUI = FindObjectOfType<InventoryGUI>();
            //spriteRenderer = GetComponent<SpriteRenderer>();
        }
        /*
        public void Update()
        {
            if (selected)
            {
                EventSystem.current.SetSelectedGameObject(gameObject);
            }
        }
        */
        public override void Selected()
        {
            base.Selected();
            //EventSystem.current.SetSelectedGameObject(gameObject);
            //spriteRenderer.color = selectedColor;
        }

        public override void Deselected()
        {
            base.Deselected();
            //EventSystem.current.SetSelectedGameObject(null);
            
            //spriteRenderer.color = normalColor;
        }
    }
}
