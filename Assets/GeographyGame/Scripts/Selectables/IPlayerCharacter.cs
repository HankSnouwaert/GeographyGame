namespace WPM
{
    public interface IPlayerCharacter : IMappableObject
    {
        bool AddItem(InventoryItem item, int location);
        void ClearCellCosts();
        void EndOfTurn(int turns);
        void FinishedPathFinding();
        float GetSize();
        void RemoveItem(int itemLocation);
        void SetCellCosts();
    }
}