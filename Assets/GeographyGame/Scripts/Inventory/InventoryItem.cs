using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace WPM
{

    abstract public class InventoryItem : SelectableObject, IInventoryItem
    {
        public Sprite InventoryIcon { get; set; }
        //public Color selectedColor = new Color(194, 194, 194, 0);
        //public Color normalColor = new Color(255, 255, 255, 255);
        public int InventorySpace { get; set; } = 1;
        public int InventoryLocation { get; set; }
        //Internal Variables and Reference Interfaces
        protected IUIManager uiManager;
        protected IInventoryUI inventoryUI;
        protected SpriteRenderer spriteRenderer;
        protected IPlayerCharacter playerCharacter;

        protected override void Awake()
        {
            base.Awake();
            //spriteRenderer = GetComponent<SpriteRenderer>();
        }

        protected override void Start()
        {
            base.Start();
            if (gameObject.activeSelf)
            {
                uiManager = interfaceFactory.UIManager;
                if (uiManager == null)
                    gameObject.SetActive(false);
                else
                {
                    inventoryUI = uiManager.InventoryUI;
                    if (inventoryUI == null)
                        errorHandler.ReportError("Inventory UI missing", ErrorState.restart_scene);
                    IPlayerManager playerManager = gameManager.PlayerManager;
                    if(playerManager == null)
                        errorHandler.ReportError("Player manager missing", ErrorState.restart_scene);
                    else
                    {
                        playerCharacter = gameManager.PlayerManager.PlayerCharacter;
                        if(playerCharacter == null)
                            errorHandler.ReportError("Player Character missing", ErrorState.restart_scene);
                    }
                }
            } 
        }

        abstract public void MouseEnter();

        abstract public void MouseDown();

        abstract public void MouseExit();
    }
}
