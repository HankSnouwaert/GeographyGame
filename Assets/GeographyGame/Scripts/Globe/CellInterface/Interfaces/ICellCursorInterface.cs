namespace WPM
{
    /// <summary>
    ///  Used to hold interfaces related to cell interaction
    /// </summary>
    public interface ICellCursorInterface
    {
        /// <summary>
        ///  Used to hold interface for cell clicking interaction
        /// </summary>
        ICellClicker CellClicker { get; set; }
        
        /// <summary>
        ///  Used to hold interface for mouse over cell interaction
        /// </summary>
        ICellEnterer CellEnterer { get; set; }

        /// <summary>
        ///  Used to hold interface for mouse moving out of cell interaction
        /// </summary>
        ICellExiter CellExiter { get; set; }

        /// <summary>
        ///  The index of the cell currently highlighted by the cursor
        /// </summary>
        int highlightedCellIndex { get; set; }

        /// <summary>
        ///  The cell currently highlighted by the cursor
        /// </summary>
        Cell highlightedCell { get; set; }
    }
}