namespace WPM
{
    public delegate void DropOffDelegate();
    public interface IDropOffUI
    {
        void ClearDropOffDelegate();
        void SetDropOffDelegate(DropOffDelegate dropOffDelegate);
        void ToggleOptionForDropOff(bool active);
    }
}