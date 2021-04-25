using System.Collections.Generic;

namespace WPM
{
    public interface IInventoryUI
    {
        void AddItem(InventoryItem item, int location);
        void ItemMouseEnter(int inventoryNumber);
        void ItemMouseExit();
        void ItemSelected(int inventoryNumber);
        void RemoveItem(int location);
        void UpdateInventory(List<InventoryItem> inventory);
    }
}