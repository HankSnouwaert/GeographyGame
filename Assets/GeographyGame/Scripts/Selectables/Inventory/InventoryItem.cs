using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace WPM
{

    public class InventoryItem : SelectableObject, IInventoryItem
    {
        public Sprite inventoryIcon;
        //public Color selectedColor = new Color(194, 194, 194, 0);
        //public Color normalColor = new Color(255, 255, 255, 255);
        public int inventorySpace = 1;
        public int inventoryLocation;
        protected InventoryUI inventoryUI;
        protected SpriteRenderer spriteRenderer;
        protected IPlayerCharacter playerCharacter;

        protected override void Awake()
        {
            base.Awake();
            inventoryUI = FindObjectOfType<InventoryUI>();
            //spriteRenderer = GetComponent<SpriteRenderer>();
        }

        protected override void Start()
        {
            base.Start();
            playerCharacter = gameManager.PlayerManager.PlayerCharacter;
        }

        public virtual void MouseEnter()
        {
            
        }

        public virtual void MouseDown()
        {

        }

        public virtual void MouseExit()
        {

        }
    }
}
