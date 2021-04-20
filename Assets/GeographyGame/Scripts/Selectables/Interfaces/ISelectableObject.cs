namespace WPM
{
    public interface ISelectableObject
    {

        string ObjectName { get; set; }
        bool Selected { get; set; }
        void MouseEnter();
        void ObjectSelected(ISelectableObject selectedObject);
        void OnCellClick(int index);
        void OnCellEnter(int index);
        void OnMouseDown();
        void OnMouseEnter();
        void OnMouseExit();
        void OnSelectableEnter(ISelectableObject selectedObject);
        void Select();
        void Deselect();
    }
}