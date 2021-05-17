using System.Collections.Generic;

namespace WPM
{
    /// <summary> 
    /// UI responsible for displaying the player's inventory
    /// </summary>
    public interface IInventoryUI : IUIElement
    {
        /// <summary> 
        /// Add an item to the player's inventory UI
        /// </summary>
        /// <param name="item"> The item to be added to the inventory</param>
        /// <param name="location"> The location in the inventory where the item should be added</param>
        void AddItem(IInventoryItem item, int location);

        /// <summary> 
        /// Remove an item to the player's inventory UI
        /// </summary>
        /// <param name="location"> The location in the inventory of the item to be removed </param>
        void RemoveItem(int location);

        /// <summary> 
        /// Called when the mouse over the image of an inventory icon
        /// </summary>
        /// <param name="inventoryNumber"> The index of the inventory item being moused over </param>
        void ItemMouseEnter(int inventoryNumber);

        /// <summary> 
        /// Called when the mouse moves out of the image of an inventory icon
        /// </summary>
        /// <param name="inventoryNumber"> The index of the inventory item being moused out of</param>
        void ItemMouseExit(int inventoryNumber);

        /// <summary> 
        /// Called when and inventory item is selected
        /// </summary>
        /// <param name="inventoryNumber"> The index of the inventory item being selected </param>
        void ItemSelected(int inventoryNumber);

        /// <summary> 
        /// Updates the inventory UI to match a given list of inventory items
        /// </summary>
        /// <param name="inventory"> The list of inventory items being used to update the inventory UI </param>
        void UpdateInventory(List<IInventoryItem> inventory);
    }
}