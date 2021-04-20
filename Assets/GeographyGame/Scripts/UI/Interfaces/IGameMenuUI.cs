namespace WPM
{
    public interface IGameMenuUI : IUIElement
    {
        void ExitGameSelected();
        void RestartGameSelected();
        void ReturnToGameSelected();
    }
}