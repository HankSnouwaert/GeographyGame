namespace WPM
{
    public interface IGlobeManager
    {
        ICellCursorInterface CellCursorInterface { get; set; }
        IGlobeParser GlobeParser { get; set; }
    }
}