using System.Collections.Generic;

namespace WPM
{
    /// <summary> 
    /// Interfaced used for the player character
    /// </summary>
    public interface IPlayerCharacter : IMappableObject
    {
        /// <summary> 
        /// Interface for the player's inventory
        /// </summary>
        IInventory Inventory { get; }

        /// <summary> 
        /// The vehicl currently used by the player
        /// </summary>
        Vehicle Vehicle { get; set; }

        /// <summary> 
        /// The movement costs associated with the player's current vehicle
        /// </summary>
        Dictionary<string, int> ClimateCosts { get; }

        /// <summary> 
        /// The terrain costs associated with the player's current vehicle
        /// </summary>
        Dictionary<string, int> TerrainCosts { get; } 
    }
}