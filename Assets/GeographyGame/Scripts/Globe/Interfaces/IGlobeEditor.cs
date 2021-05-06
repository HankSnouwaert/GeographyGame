namespace WPM
{
    /// <summary> 
    /// Used to make changes to the provinces of the world map globe
    /// </summary>
    public interface IGlobeEditor
    {
        /// <summary> 
        /// A list of flags determining what province attributes are being used
        /// </summary>
        bool[] ProvinceSettings { get; }

        /// <summary> 
        /// Used merge the provinces in a given country based on a set of province settings
        /// </summary>
        /// <param name="country"> The country having its provinces merged </param>
        /// <param name="provinceSettings"> The flags being used to determine how the provinces should be merged</param>
        void MergeProvincesInCountry(Country country, bool[] provinceSettings);

        /// <summary> 
        /// Gets the attributes of a given province
        /// </summary>
        /// <param name="province"> The province whose attributes are retrieved</param>
        /// <returns> A list of strings, each corresponding to a given province attribute </returns>
        string[] GetProvinceAttributes(Province province);
    }
}