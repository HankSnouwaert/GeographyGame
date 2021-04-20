namespace WPM
{
    public interface IInventoryPopUpUI : IUIElement
    {
        //bool PersistantPopUp { get; set; }
        bool TempPopUp { get; set; }
        void DisplayPopUp(string displayString, bool persistant);
        void ResetPersistantMessage();
        void ClearPopUp(bool persistant);
    }
}