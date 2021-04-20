using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WPM
{
    public class CountryParser : MonoBehaviour, ICountryParser
    {
        private GlobeManager globeManager;
        private WorldMapGlobe worldMapGlobe;
        private IProvinceParser provinceParser;
        private void Awake()
        {
            globeManager = FindObjectOfType<GlobeManager>();
        }
        private void Start()
        {
            provinceParser = globeManager.GlobeParser.ProvinceParser;
            worldMapGlobe = globeManager.WorldMapGlobe;
        }
        /// <summary> 
        /// Get all countries within a certain range (measured in cells) of a target cell
        /// Inputs:
        ///     startCell:  Target cell the range is being measured from
        ///     range:      The range (in cells) out from startCell that the method increments through
        /// Outputs:
        ///     countryIndexes:  An array of lists, with ListX containing the countries reachable within
        ///                 X number of cells away from the target cell
        /// </summary>
        public List<int>[] GetCountriesInRange(int startCell, List<int>[] cellRange)
        {
            int range = cellRange.Length;

            if (range < 0 || startCell < 0 || worldMapGlobe.cells.Count() < startCell)
            {
                //This will need to be replaced with an error message
                Debug.LogWarning("Invalid input for GetProvincesInRange");
                return null;
            }

            List<int>[] countryIndexes = new List<int>[range + 1];      //provinces is an array of lists with each list containing 
            List<int> foundCountryIndexes = new List<int>();            //the provinces that can be reached at that distance. 

            List<int>[] provinceIndexes = provinceParser.GetProvincesInRange(startCell, cellRange);

            //Create lists of the countries within range based off of the provinces in range
            int i = 0;
            Province province;
            foreach (List<int> indexList in countryIndexes)
            {
                foreach (int provinceIndex in provinceIndexes[i])
                {
                    province = worldMapGlobe.provinces[provinceIndex];
                    if (!foundCountryIndexes.Contains(province.countryIndex))
                    {
                        foundCountryIndexes.Add(province.countryIndex);
                        countryIndexes[i].Add(province.countryIndex);
                    }
                }
                i++;
            }

            return countryIndexes;
        }

        /// <summary> 
        /// Get all countries that overlap with a given cell
        /// Inputs:
        ///     cellIndex:  Index of the cell in question
        /// Outputs:
        ///     countryIndexes:  An array of countries that overlap with the cell in quesiton
        /// </summary>
        public List<int> GetCountriesInCell(int cellIndex)
        {
            List<int> provinceIndexes = provinceParser.GetProvicesInCell(cellIndex);
            List<int> countryIndexes = GetCountriesFromProvinces(provinceIndexes);

            return countryIndexes;
        }

        /// <summary>
        ///  Given a list of province indexes, find all the countries they contain
        /// </summary>
        /// <param name="provinceIndexes"></param> The list of province indexes>
        /// <returns></returns> 
        public List<int> GetCountriesFromProvinces(List<int> provinceIndexes)
        {
            List<int> countryIndexes = new List<int>();
            int countryIndex;
            Province province;
            foreach (int provinceIndex in provinceIndexes)
            {
                province = worldMapGlobe.provinces[provinceIndex];
                countryIndex = province.countryIndex;
                if (!countryIndexes.Contains(countryIndex))
                {
                    countryIndexes.Add(countryIndex);
                }
            }
            return countryIndexes;
        }

    }
}
