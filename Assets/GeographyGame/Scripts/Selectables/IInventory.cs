using System.Collections.Generic;

namespace WPM
{
    public interface IInventory
    {
        List<IInventoryItem> InventoryList { get; }

        bool AddItem(IInventoryItem item, int location);
        void RemoveItem(int itemLocation);
    }
}