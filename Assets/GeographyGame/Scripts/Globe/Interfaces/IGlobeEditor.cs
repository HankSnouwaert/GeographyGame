namespace WPM
{
    public interface IGlobeEditor
    {
        bool[] ProvinceSettings { get; }

        void MergeProvincesInCountry(Country country, bool[] provinceSettings);

        string[] GetProvinceAttributes(Province province);
    }
}