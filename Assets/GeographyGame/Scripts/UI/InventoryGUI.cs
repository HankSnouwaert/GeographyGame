﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace WPM
{
    public class InventoryGUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
       //private PlayerCharacter playerCharacter;
        private GameManager gameManager;
        public InventoryItem[] displayedItems;
        public Button[] displayedItemButtons;
        private GameObject selectedObject;
        private int selectedItemIndex;
        private bool inventorySelected = false;
        private int numberofItems = 0;
        // Start is called before the first frame update
        void Start()
        {
            displayedItemButtons = GetComponentsInChildren<Button>(true);
            displayedItems = new InventoryItem[displayedItemButtons.Length];
            gameManager = FindObjectOfType<GameManager>();
            //playerCharacter = FindObjectOfType<PlayerCharacter>();
        }
        void Update()
        {
            if(selectedObject != null)
            {
                //Check if the inventory has been selected while the GUI still thinks it's selected
                if (inventorySelected && !displayedItems[selectedItemIndex].selected)
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
        public void AddItem(InventoryItem item)
        {
            displayedItems[numberofItems] = item;
            displayedItemButtons[numberofItems].gameObject.SetActive(true);
            displayedItemButtons[numberofItems].GetComponent<Image>().sprite = item.inventoryIcon;
            numberofItems++;
        }

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
                    displayedItems[i].inventoryLocation = i;
                    displayedItemButtons[i].gameObject.SetActive(true);
                    displayedItemButtons[i].GetComponent<Image>().sprite = displayedItemButtons[i + 1].GetComponent<Image>().sprite;
                }
                i++;
            } while (i <= numberofItems);
           
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log(gameObject.name + ": I was entered!");
            gameManager.cursorOverUI = true;
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            Debug.Log(gameObject.name + ": I was exited!");
            gameManager.cursorOverUI = false;
        }

        /// <summary>
        /// This function is called when an item in the inventory is selected
        /// </summary>
        /// <param name="inventoryNumber">What position in the inventory the item is in.</param>
        public void ItemSelected(int inventoryNumber)
        {
            //The inventory position needs to be offset so it starts at 0 instead of 1
            //inventoryNumber--;
            if (inventoryNumber <= numberofItems && inventoryNumber >= 0)
            {
                if (displayedItems[inventoryNumber].selected)
                {
                    displayedItems[inventoryNumber].Deselected();  
                }
                else
                {
                    displayedItems[inventoryNumber].Selected();
                }   
            }
            inventorySelected = true;
            selectedItemIndex = inventoryNumber;
            selectedObject = EventSystem.current.currentSelectedGameObject;
        }

    }
}
