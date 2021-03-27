namespace WPM
{
    public interface IMouseOverInfoUI
    {
        string MouseOverInfoString { get; set; }
        string CreateMouseOverInfoString(Province province, Country country, MappableObject highlightedObject);
        void SetMouseOverInfoMessage(string textToSet);
        void UpdateUI();
    }
}