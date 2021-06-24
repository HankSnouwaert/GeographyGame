using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPM
{
    public class Inventory : MonoBehaviour, IInventory
    {
        //Public Variables
        public List<IInventoryItem> InventoryList { get; protected set; } = new List<IInventoryItem>();
        //Private Variables
        private bool started = false;
        private readonly int inventorySize = 9;
        //Private Interface References
        private IInventoryUI inventoryUI;
        private IGameManager gameManager;
        private IUIManager uiManager;
        private ITouristManager touristManager;
        //Error Checking
        private InterfaceFactory interfaceFactory;
        private IErrorHandler errorHandler;

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

        public bool AddItem(IInventoryItem item, int location)
        {
            if (!started)
                Start();

            if(item == null || location < 0 || location > inventorySize)
            {
                errorHandler.ReportError("Invalid input to AddItem", ErrorState.close_window);
                return false;
            }

            InventoryList.Insert(location, item);

            if (InventoryList.Count > inventorySize)
                RemoveItem(inventorySize);
           
            //Update inventory item locations
            foreach (InventoryItem inventoryItem in InventoryList)
            {
                inventoryItem.InventoryLocation = InventoryList.IndexOf(inventoryItem);
            }
            inventoryUI.UpdateInventory(InventoryList);

            return true;
        }

        public void RemoveItem(int itemLocation)
        {
            if(itemLocation < 0 || itemLocation >= InventoryList.Count)
            {
                errorHandler.ReportError("Attempting to remove inventory item from invalid location", ErrorState.close_window);
                return;
            }

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

