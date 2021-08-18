namespace WPM
{
    public interface ITutorialUI : IUIElement
    {
        void EnableButton1(bool enabled);
        void EnableButton2(bool enabled);
        void SetButton1Delegate(DropOffDelegate buttonDelegate);
        void SetButton2Delegate(DropOffDelegate buttonDelegate);
        void SetButton1Text(string textString);
        void SetButton2Text(string textString);
        void SetMainText(string textString);

        void SetUIPosition();
    }
}