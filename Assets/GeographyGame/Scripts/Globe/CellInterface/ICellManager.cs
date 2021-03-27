namespace WPM
{
    /// <summary>
    ///  Used to hold interfaces related to cell interaction
    /// </summary>
    public interface ICellManager
    {
        ICellClicker CellClicker { get; set; }
        ICellEnterer CellEnterer { get; set; }
        ICellExiter CellExiter { get; set; }
        int highlightedCellIndex { get; set; }
        Cell highlightedCell { get; set; }
    }
}