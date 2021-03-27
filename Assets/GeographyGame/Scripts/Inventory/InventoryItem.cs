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
        protected InventoryUI inventoryUI;
        protected SpriteRenderer spriteRenderer;

        public override void Awake()
        {
            base.Awake();
            inventoryUI = FindObjectOfType<InventoryUI>();
            //spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public virtual void Start()
        {

        }

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

        public override void MouseEnter()
        {
            base.MouseEnter();
        }
    }
}
