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
        public List<Country>[] GetCountriesInRange(Cell startCell, List<Cell>[] cellRange)
        {
            int range = cellRange.Length;

            if (range < 0)// || startCell < 0 || worldMapGlobe.cells.Count() < startCell)
            {
                //This will need to be replaced with an error message
                Debug.LogWarning("Invalid input for GetProvincesInRange");
                return null;
            }

            List<Country>[] countries= new List<Country>[range + 1];      //provinces is an array of lists with each list containing 
            List<Country> foundCountries= new List<Country>();            //the provinces that can be reached at that distance. 

            List<Province>[] provinces = provinceParser.GetProvincesInRange(startCell, cellRange);

            //Create lists of the countries within range based off of the provinces in range
            int i = 0;
            //Province province;
            foreach (List<Country> countryList in countries)
            {
                foreach (Province province in provinces[i])
                {
                    //province = worldMapGlobe.provinces[provinceIndex];
                    Country country = worldMapGlobe.countries[province.countryIndex];
                    if (!foundCountries.Contains(country))
                    {
                        foundCountries.Add(country);
                        countries[i].Add(country);
                    }
                }
                i++;
            }

            return countries;
        }

        /// <summary> 
        /// Get all countries that overlap with a given cell
        /// Inputs:
        ///     cellIndex:  Index of the cell in question
        /// Outputs:
        ///     countryIndexes:  An array of countries that overlap with the cell in quesiton
        /// </summary>
        public List<Country> GetCountriesInCell(Cell cell)
        {
            List<Province> provinces = provinceParser.GetProvicesInCell(cell);
            List<Country> countries = GetCountriesFromProvinces(provinces);

            return countries;
        }

        /// <summary>
        ///  Given a list of province indexes, find all the countries they contain
        /// </summary>
        /// <param name="provinceIndexes"></param> The list of province indexes>
        /// <returns></returns> 
        public List<Country> GetCountriesFromProvinces(List<Province> provinces)
        {
            List<Country> countries = new List<Country>();
            Country country;
            //Province province;
            foreach (Province province in provinces)
            {
                //province = worldMapGlobe.provinces[provinceIndex];
                country = worldMapGlobe.countries[province.countryIndex];
                if (!countries.Contains(country))
                {
                    countries.Add(country);
                }
            }
            return countries;
        }

    }
}
