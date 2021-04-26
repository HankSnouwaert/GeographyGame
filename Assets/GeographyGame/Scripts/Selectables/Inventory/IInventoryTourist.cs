namespace WPM
{
    public interface IInventoryTourist : IInventoryItem
    {
        void AttemptDropOff();
        void SetPopUpRequest(bool persistant);
    }
}