namespace WPM
{
    /// <summary>
    /// Delegate used to refence the drop off method of the item being dropped off
    /// </summary>
    public delegate void DropOffDelegate();

    /// <summary>
    /// UI used to drop off inventory items
    /// </summary>
    public interface IDropOffUI
    {
        /// <summary>
        /// Clears the current drop off delegate from the UI
        /// </summary>
        void ClearDropOffDelegate();

        /// <summary>
        /// Sets the current drop off delegate of the UI
        /// </summary>
        void SetDropOffDelegate(DropOffDelegate dropOffDelegate);

        /// <summary>
        /// Used to set whether or not the option for drop off is available
        /// </summary>
        /// <param name="active"> Whether or not drop off functionality is available </param>
        void ToggleOptionForDropOff(bool active);
    }
}