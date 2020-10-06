using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WPM
{
    public class InventoryGUI : MonoBehaviour
    {
        private PlayerCharacter playerCharacter;
        public InventoryItem[] displayedItems;
        public Button[] displayedItemButtons;
        private int numberofItems = 0;
        // Start is called before the first frame update
        void Start()
        {
            displayedItemButtons = GetComponentsInChildren<Button>(true);
            displayedItems = new InventoryItem[displayedItemButtons.Length];
        }

        public void AddItem(InventoryItem item)
        {
            displayedItems[numberofItems] = item;
            displayedItemButtons[numberofItems].gameObject.SetActive(true);
            displayedItemButtons[numberofItems].GetComponent<Image>().sprite = item.inventoryIcon;
            numberofItems++;
        }

    }
}
