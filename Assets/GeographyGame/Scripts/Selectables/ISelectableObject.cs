namespace WPM
{
    public interface ISelectableObject
    {

        string ObjectName { get; set; }
        bool Selected { get; set; }
        void MouseEnter();
        void ObjectSelected(SelectableObject selectedObject);
        void OnCellClick(int index);
        void OnCellEnter(int index);
        void OnMouseDown();
        void OnMouseEnter();
        void OnMouseExit();
        void OnSelectableEnter(SelectableObject selectedObject);
        void Select();
        void Deselect();
    }
}