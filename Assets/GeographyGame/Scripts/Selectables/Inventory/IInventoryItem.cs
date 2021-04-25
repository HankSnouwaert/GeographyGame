namespace WPM
{
    /// <summary> 
    /// Item that is contained in the player's inventory
    /// </summary>
    public interface IInventoryItem : ISelectableObject
    {
        /// <summary> 
        /// Icon used to represent the item on the inventory UI
        /// </summary>
        UnityEngine.Sprite InventoryIcon { get; set; }
        /// <summary> 
        /// How many inventory slots the item takes up
        /// </summary>
        int InventorySpace { get; set; }
        /// <summary> 
        /// The location of the first inventory slot the item takes up
        /// </summary>
        int InventoryLocation { get; set; }
        /// <summary> 
        /// Called when the player clicks on the item's inventory button
        /// </summary>
        void MouseDown();
        /// <summary> 
        /// Called when the player's cursor moves over the item's inventory button
        /// </summary>
        void MouseEnter();
        /// <summary> 
        /// Called when the player's cursor moves out of the item's inventory button
        /// </summary>
        void MouseExit();
    }
}