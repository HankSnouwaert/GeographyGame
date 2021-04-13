namespace WPM
{
    public interface IGlobeManager
    {
        WorldMapGlobe WorldGlobeMap { get; set; }
        ICellCursorInterface CellCursorInterface { get; set; }
        IGlobeParser GlobeParser { get; set; }
    }
}