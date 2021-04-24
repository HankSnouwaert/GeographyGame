namespace WPM
{
    public interface IInventoryItem : ISelectableObject
    {
        void MouseDown();
        void MouseEnter();
        void MouseExit();
    }
}