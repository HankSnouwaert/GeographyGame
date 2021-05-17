using System.Collections.Generic;

namespace WPM
{
    /// <summary> 
    /// Represents an in game inventory
    /// </summary>
    public interface IInventory
    {
        /// <summary> 
        /// The list of items currently contained by the inventory
        /// </summary>
        List<IInventoryItem> InventoryList { get; }

        /// <summary> 
        /// Adds a given item to the inventory
        /// </summary>
        /// <param name="item"> The item being added to the inventory</param>
        /// <param name="location"> The inventory location the item is being added to</param>
        /// <returns> A flag indicating whether the item was successfully added or not </returns>
        bool AddItem(IInventoryItem item, int location);

        /// <summary> 
        /// Removes an item from a given inventory location
        /// </summary>
        /// <param name="itemLocation"> The location in the inventory the item is being removed from </param>
        void RemoveItem(int itemLocation);
    }
}