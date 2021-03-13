namespace WPM
{
    public interface ICellClicker
    {
        bool ClosingGUIPanel { get; set; }
        bool NewObjectSelected { get; set; }

        void HandleOnCellClick(int cellIndex);
    }
}