using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WPM
{
    public class CountryParser : MonoBehaviour, ICountryParser
    {
        private IGlobeManager globeManager;
        private WorldMapGlobe worldMapGlobe;
        private IGlobeParser globeParser;
        private IProvinceParser provinceParser;
        //Error Checking
        private InterfaceFactory interfaceFactory;
        private IErrorHandler errorHandler;

        private void Awake()
        {
            interfaceFactory = FindObjectOfType<InterfaceFactory>();
            if (interfaceFactory == null)
                gameObject.SetActive(false);
        }
        private void Start()
        {
            globeManager = interfaceFactory.GlobeManager;
            errorHandler = interfaceFactory.ErrorHandler;
            if (globeManager == null || errorHandler == null)
                gameObject.SetActive(false);
            else
            {
                worldMapGlobe = globeManager.WorldMapGlobe;
                if (worldMapGlobe == null)
                    errorHandler.ReportError("World Map Globe missing", ErrorState.restart_scene);
                globeParser = globeManager.GlobeParser;
                if (globeParser == null)
                    errorHandler.ReportError("Globe Parser missing", ErrorState.restart_scene);
                else
                {
                    provinceParser = globeParser.ProvinceParser;
                    if(provinceParser == null)
                        errorHandler.ReportError("Province Parser missing", ErrorState.restart_scene);
                }
            }
            
        }
        
        public List<Country>[] GetCountriesInRange(Cell startCell, int range)
        {
            if (range < 0 || startCell.index < 0 || worldMapGlobe.cells.Count() < startCell.index)
            {
                errorHandler.ReportError("Invalid input for GetCountriesInRange", ErrorState.close_window);
                return null;
            }

            List<Country>[] countries= new List<Country>[range + 1];      //provinces is an array of lists with each list containing 
            List<Country> foundCountries= new List<Country>();            //the provinces that can be reached at that distance. 

            List<Province>[] provinces = provinceParser.GetProvincesInRange(startCell, range);
            if(provinces == null)
            {
                errorHandler.ReportError("Failed to parse provinces", ErrorState.close_window);
                return null;
            }

            try
            {
                //Create lists of the countries within range based off of the provinces in range
                int i = 0;
                //Province province;
                foreach (List<Country> countryList in countries)
                {
                    foreach (Province province in provinces[i])
                    {
                        Country country = worldMapGlobe.countries[province.countryIndex];
                        if (!foundCountries.Contains(country))
                        {
                            foundCountries.Add(country);
                            countries[i].Add(country);
                        }
                    }
                    i++;
                }
            }
            catch (System.Exception ex)
            {
                errorHandler.CatchException(ex, ErrorState.close_window);
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
            if (provinces == null)
            {
                errorHandler.ReportError("Failed to parse provinces", ErrorState.close_window);
                return null;
            }
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
            try
            {
                foreach (Province province in provinces)
                {
                    country = worldMapGlobe.countries[province.countryIndex];
                    if (!countries.Contains(country))
                    {
                        countries.Add(country);
                    }
                }
            }
            catch(System.Exception ex)
            {
                errorHandler.CatchException(ex, ErrorState.close_window);
            }
            return countries;
        }

    }
}
