using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPM
{
    public class Inventory : MonoBehaviour, IInventory
    {
        public List<IInventoryItem> InventoryList { get; protected set; } = new List<IInventoryItem>();
        private readonly int inventorySize = 7;
        GameObject InventoryPanel;
        IInventoryUI inventoryUI;
        bool started = false;
        private IGameManager gameManager;
        private IUIManager uiManager;
        private ITouristManager touristManager;
        //Error Checking
        private InterfaceFactory interfaceFactory;
        private IErrorHandler errorHandler;
        private bool componentMissing = false;

        private void Awake()
        {
            interfaceFactory = FindObjectOfType<InterfaceFactory>();
            if (interfaceFactory == null)
                gameObject.SetActive(false);
        }

        private void Start()
        {
            gameManager = interfaceFactory.GameManager;
            errorHandler = interfaceFactory.ErrorHandler;
            uiManager = interfaceFactory.UIManager;
            if (gameManager == null || errorHandler == null || uiManager == null)
                gameObject.SetActive(false);
            else
            {
                touristManager = gameManager.TouristManager;
                if (touristManager == null)
                    errorHandler.ReportError("Tourist Manager missing", ErrorState.restart_scene);

                inventoryUI = uiManager.InventoryUI;
                if (inventoryUI == null)
                    errorHandler.ReportError("Inventory UI missing", ErrorState.restart_scene);

                started = true;
            }
        }


        //INVENTORY SPECIFIC
        public bool AddItem(IInventoryItem item, int location)
        {
            if (!started)
                Start();

            InventoryList.Insert(location, item);

            if (InventoryList.Count > inventorySize)
                RemoveItem(inventorySize);
            /*
            if (inventory.Count < inventorySize)
            {
                item.inventoryLocation = 0; //inventory.Count;
                inventory.Add(item);
                inventoryGUI.AddItem(item, 0);
                return true;
            }
            else
            {
                return false;
            }
            */
            //Update inventory item locations
            foreach (InventoryItem inventoryItem in InventoryList)
            {
                inventoryItem.InventoryLocation = InventoryList.IndexOf(inventoryItem);
            }
            inventoryUI.UpdateInventory(InventoryList);

            return true;
        }

        //INVENTORY SPECIFIC
        public void RemoveItem(int itemLocation)
        {
            InventoryList.RemoveAt(itemLocation);
            foreach (InventoryItem inventoryItem in InventoryList)
            {
                inventoryItem.InventoryLocation = InventoryList.IndexOf(inventoryItem);
            }
            inventoryUI.UpdateInventory(InventoryList);
            if (InventoryList.Count == 0)
            {
                touristManager.GenerateTourist();
            }
        }
    }
}

