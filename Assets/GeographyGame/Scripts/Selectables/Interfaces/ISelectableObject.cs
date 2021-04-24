namespace WPM
{
    /// <summary> 
    /// Base interface for any object that can be selected by the player
    /// </summary>
    public interface ISelectableObject
    {
        /// <summary> 
        /// String used to reference this selectable object
        /// </summary>
        string ObjectName { get; set; }
        
        /// <summary> 
        /// Flag used to indicate if the object is selected
        /// </summary>
        bool Selected { get; set; }

        /// <summary> 
        /// Select the object
        /// </summary>
        void Select();
        
        /// <summary> 
        /// Deselect the object
        /// </summary>
        void Deselect();

        /// <summary> 
        /// Called when another selectable object is selected while this object is selected
        /// </summary>
        /// /// <param name="selectedObject"> The object being selected </param>
        void OtherObjectSelected(ISelectableObject selectedObject);
        
        /// <summary> 
        /// Called when another selectable object is moused over while this object is selected
        /// </summary>
        /// <param name="selectedObject"> The object being moused over </param>
        void OnSelectableEnter(ISelectableObject selectableObject);
        
        /// <summary> 
        /// Called when a cell is clicked while this object is selected
        /// </summary>
        void OnCellClick(int index);
        
        /// <summary> 
        /// Called when a cell is moused while this object is selected
        /// </summary>
        void OnCellEnter(int index);

        
        
    }
}