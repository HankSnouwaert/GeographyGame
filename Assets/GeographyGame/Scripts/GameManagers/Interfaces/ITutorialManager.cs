namespace WPM
{
    public interface ITutorialManager
    {
        ICameraTutorial CameraTutorial { get; }
        void BeginTutorial();
        void CurrentTutorialFinished();
    }
}