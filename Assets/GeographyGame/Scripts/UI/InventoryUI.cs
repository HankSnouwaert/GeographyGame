using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace WPM
{
    public class InventoryUI : UIElement, IInventoryUI
    {
       //private PlayerCharacter playerCharacter;
        public IInventoryItem[] displayedItems;
        public Button[] displayedItemButtons;
        private GameObject selectedObject;
        private int selectedItemIndex = -1;
        private bool inventorySelected = false;
        private int numberofItems = 0;
        private bool componentsMissing = false;

        protected override void Awake()
        {
            base.Awake();
            if (gameObject.activeSelf)
            {
                displayedItemButtons = GetComponentsInChildren<Button>(true);
                if (displayedItemButtons == null)
                    componentsMissing = true;
                else
                {
                    displayedItems = new IInventoryItem[displayedItemButtons.Length];
                    if (displayedItems == null)
                        componentsMissing = true;
                }
            }
        }

        protected override void Start()
        {
            if (componentsMissing)
                errorHandler.ReportError("Inventory UI components missing", ErrorState.restart_scene);
        }

        void Update()
        {
            try
            {
                if (selectedObject != null && selectedItemIndex < displayedItems.Length && selectedItemIndex >= 0)
                {
                    //Check if the inventory has been selected while the GUI still thinks it's selected
                    if (inventorySelected && !displayedItems[selectedItemIndex].Selected)
                    {
                        //Check if the EventSystem still thinks the inventory item is selected
                        if (EventSystem.current.currentSelectedGameObject == selectedObject)
                        {
                            //The EventSystem still thinks the inventory item is selected and needs to be cleared
                            EventSystem.current.SetSelectedGameObject(null);
                        }
                        //The inventory should be deselected
                        inventorySelected = false;
                        selectedObject = null;
                    }
                    else
                    {
                        //The GUI is up to date
                        //Check if the EventSystem has been cleared when it shouldn't
                        if (EventSystem.current.currentSelectedGameObject == null)
                        {
                            //The EventSystem was cleared and needs to be updated
                            EventSystem.current.SetSelectedGameObject(selectedObject);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                errorHandler.CatchException(ex, ErrorState.restart_scene);
            }
        }

        //THIS METHOD IS NEVER USED
        public void AddItem(IInventoryItem item, int location)
        {
            if(location >= displayedItems.Length || location < 0)
            {
                errorHandler.ReportError("Attempted to add item to invalid inventory location", ErrorState.close_window);
                return;
            }

            if(displayedItems[location] != null)
            {
                //There's someone in my spot
                if(location == numberofItems)
                {
                    //This spot is the end of the line
                    //Push out the existing item and replace it with the new one
                    displayedItems[location] = item;
                    displayedItemButtons[numberofItems].GetComponent<Image>().sprite = item.InventoryIcon;
                }
            }
            else
            {
                displayedItems[numberofItems] = item;
                displayedItemButtons[numberofItems].gameObject.SetActive(true);
                displayedItemButtons[numberofItems].GetComponent<Image>().sprite = item.InventoryIcon;
                numberofItems++;
            }
        }

        //THIS METHOD IS NEVER USED
        public void RemoveItem(int location)
        {
            //Check if you are removing the currently selected object
            if(selectedItemIndex == location)
            {
                EventSystem.current.SetSelectedGameObject(null);
                inventorySelected = false;
                selectedObject = null;
            }

            numberofItems--;
            int i = location;
            do
            {
                if (i == numberofItems)  //Am I at the last item?
                {
                    displayedItems[i] = null;
                    displayedItemButtons[i].gameObject.SetActive(false);
                    displayedItemButtons[i].GetComponent<Image>().sprite = null;
                }
                else
                {
                    displayedItems[i] = displayedItems[i + 1];
                    displayedItems[i].InventoryLocation = i;
                    displayedItemButtons[i].gameObject.SetActive(true);
                    displayedItemButtons[i].GetComponent<Image>().sprite = displayedItemButtons[i + 1].GetComponent<Image>().sprite;
                }
                i++;
            } while (i <= numberofItems);
           
        }

        public void UpdateInventory(List<IInventoryItem> inventory)
        {
            try
            {
                int i = 0;
                //Clear the inventory
                foreach (IInventoryItem item in displayedItems)
                {
                    if (i == selectedItemIndex && inventorySelected)
                    {
                        item.Deselect();
                        EventSystem.current.SetSelectedGameObject(null);
                        inventorySelected = false;
                        selectedObject = null;
                    }
                    displayedItems[i] = null;
                    displayedItemButtons[i].gameObject.SetActive(false);
                    displayedItemButtons[i].GetComponent<Image>().sprite = null;
                    i++;
                }

                i = 0;
                //Update the inventory
                foreach (InventoryItem item in inventory)
                {
                    displayedItems[i] = item;
                    displayedItemButtons[i].gameObject.SetActive(true);
                    displayedItemButtons[i].GetComponent<Image>().sprite = item.InventoryIcon;
                    i++;
                }
                numberofItems = i;
            }
            catch(System.Exception ex)
            {
                errorHandler.CatchException(ex, ErrorState.restart_scene);
            }
            
        }

        /// <summary>
        /// This function is called when an item in the inventory is selected
        /// </summary>
        /// <param name="inventoryNumber">What position in the inventory the item is in.</param>
        public void ItemSelected(int inventoryNumber)
        {
            if (inventoryNumber <= numberofItems && inventoryNumber >= 0)
            {
                if(displayedItems[inventoryNumber] == null)
                {
                    errorHandler.ReportError("Invalid item selection", ErrorState.close_window);
                    return;
                }

                if (displayedItems[inventoryNumber].Selected)
                {
                    displayedItems[inventoryNumber].Deselect();  
                }
                else
                {
                    displayedItems[inventoryNumber].Select();
                }   
            }
            inventorySelected = true;
            selectedItemIndex = inventoryNumber;
            selectedObject = EventSystem.current.currentSelectedGameObject;
        }

        /// <summary>
        /// This function is called when the mouse goes over an item in the inventory
        /// </summary>
        /// <param name="inventoryNumber">What position in the inventory the item is in.</param>
        public void ItemMouseEnter(int inventoryNumber)
        {
            if (inventoryNumber <= numberofItems && inventoryNumber >= 0)
            {
                if (displayedItems[inventoryNumber] == null)
                {
                    errorHandler.ReportError("Invalid item selection", ErrorState.close_window);
                    return;
                }

                displayedItems[inventoryNumber].MouseEnter();
            }
        }
        
        public void ItemMouseExit(int inventoryNumber)
        {
            //gameManager.ClosePopUp();
        }
    }
}
