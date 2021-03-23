namespace WPM
{
    /// <summary>
    ///  Called when the cursor moves out of a cell on the world globe map
    /// </summary>
    public interface ICellExiter
    {
        /// <summary>
        ///  Called when the cursor moves out of a cell on the world globe map
        /// </summary>
        /// <param name="cellIndex"></param> The cell the cursor moves out of>
        /// <returns></returns> 
        void HandleOnCellExit(int cellIndex);
    }
}