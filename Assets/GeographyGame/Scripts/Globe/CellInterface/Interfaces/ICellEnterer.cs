namespace WPM
{
    /// <summary>
    ///  Called when the cursor moves over a cell on the world globe map
    /// </summary>
    public interface ICellEnterer
    {
        /// <summary>
        ///  Called when the cursor moves over a cell on the world globe map
        /// </summary>
        /// <param name="cellIndex"></param> The cell the cursor moves over>
        /// <returns></returns> 
        void HandleOnCellEnter(int cellIndex);
    }
}