using System.Collections.Generic;

namespace WPM
{
    public interface IPlayerCharacter : IMappableObject
    {
        IInventory Inventory { get; }
        Vehicle Vehicle { get; set; }
        Dictionary<string, int> ClimateCosts { get; }
        Dictionary<string, int> TerrainCosts { get; }
        //bool Stop { get; set; }
        //bool AddItem(IInventoryItem item, int location);
        //void ClearCellCosts();
        void EndOfTurn(int turns);
        //void FinishedPathFinding();
        //float GetSize();
        //void RemoveItem(int itemLocation);
        //void SetCellCosts();
    }
}