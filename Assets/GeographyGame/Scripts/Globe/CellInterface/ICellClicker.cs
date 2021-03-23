namespace WPM
{
    /// <summary>
    /// Interface for handling cell clicks on the world globe map
    /// </summary>
    public interface ICellClicker
    {
        /// <summary>
        /// A flag used to determine if the current click should be ignored
        /// because a panel is being closed
        /// </summary>
        bool ClosingUIPanel { get; set; }

        /// <summary>
        /// A flag used to determine if the current click should be ignored
        /// because a new object was just selected
        /// </summary>
        bool NewObjectSelected { get; set; }

        /// <summary>
        /// Called when a cell on the globe is selected
        /// </summary>
        /// <param name="cellIndex"></param> Index of the cell being clicked
        void HandleOnCellClick(int cellIndex);
    }
}