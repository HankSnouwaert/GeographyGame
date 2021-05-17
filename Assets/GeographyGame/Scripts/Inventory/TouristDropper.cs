using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace WPM
{
    public class TouristDropper : MonoBehaviour, ITouristDropper
    {
        //Interal Interface References
        private IGlobeManager globeManager;
        private IGlobeParser globeParser;
        private IProvinceParser provinceParser;
        private ICountryParser countryParser;
        private WorldMapGlobe worldMapGlobe;
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
            errorHandler = interfaceFactory.ErrorHandler;
            globeManager = interfaceFactory.GlobeManager;
            if (errorHandler == null || globeManager == null)
            {
                gameObject.SetActive(false);
            }
            else
            {
                worldMapGlobe = globeManager.WorldMapGlobe;
                if (worldMapGlobe == null)
                    errorHandler.ReportError("World Map Globe missing", ErrorState.close_window);

                globeParser = globeManager.GlobeParser;
                if(globeParser == null)
                    errorHandler.ReportError("Globe Parser missing", ErrorState.close_window);
                else
                {
                    provinceParser = globeParser.ProvinceParser;
                    if (provinceParser == null)
                    {
                        errorHandler.ReportError("Province Parser missing", ErrorState.restart_scene);
                    }
                    countryParser = globeParser.CountryParser;
                    if (provinceParser == null)
                    {
                        errorHandler.ReportError("Country Parser missing", ErrorState.restart_scene);
                    }
                }
            }
        }

        public bool AttemptDropOff(Cell dropOffCell, DestinationType destinationType, Province provinceDestination, Landmark landmarkDestination, Country countryDestination)
        {
            //Clear the Event System so that it gets updated with the tourist if the drop off fails
            EventSystem.current.SetSelectedGameObject(null);

            bool dropOffSuccess = false;

            if (dropOffCell == null)
            {
                errorHandler.ReportError("Drop Off Failed: Player in invalid cell", ErrorState.close_window);
                return false;
            }

            switch (destinationType)
            {
                case DestinationType.province:
                    dropOffSuccess = AttemptProvinceDropOff(dropOffCell, provinceDestination);
                    break;

                case DestinationType.landmark:
                    dropOffSuccess = AttemptLandmarkDropOff(dropOffCell, landmarkDestination);
                    break;

                case DestinationType.country:
                    dropOffSuccess = AttemptCountryDropOff(dropOffCell, countryDestination);
                    break;
                default:
                    break;
            }

            return dropOffSuccess;
        }

        /// <summary> 
        /// Check if a given location is a correct drop off location given a desired province
        /// </summary>
        /// <param name="dropOffCell"> The cell drop off cell </param>
        /// <param name="provinceDestination"> The desired province destination </param>
        private bool AttemptProvinceDropOff(Cell dropOffCell, Province provinceDestination)
        {
            bool dropOffSuccess = false;

            if (provinceDestination == null)
            {
                errorHandler.ReportError("Drop Off Failed: Province destination not set", ErrorState.close_window);
                return false;
            }

            List<Province> selectedProvinces = provinceParser.GetProvicesInCell(dropOffCell);
            if (selectedProvinces == null)
            {
                errorHandler.ReportError("Drop Off Failed: No provinces in cell", ErrorState.close_window);
                return false;
            }

            foreach (Province province in selectedProvinces)
            {
                try
                {
                    if (province == provinceDestination)
                    {
                        dropOffSuccess = true;
                        break;
                    }
                }
                catch (System.Exception ex)
                {
                    errorHandler.CatchException(ex, ErrorState.close_window);
                    return false;
                }
            }

            return dropOffSuccess;
        }

        /// <summary> 
        /// Check if a given location is a correct drop off location given a desired landmark
        /// </summary>
        /// <param name="dropOffCell"> The cell drop off cell </param>
        /// <param name="landmarkDestination"> The desired landmark destination </param>
        private bool AttemptLandmarkDropOff(Cell dropOffCell, Landmark landmarkDestination)
        {
            bool dropOffSuccess = false;

            if (landmarkDestination == null)
            {
                errorHandler.ReportError("Drop Off Failed: Landmark destination not set", ErrorState.close_window);
                return false;
            }

            if (landmarkDestination.CellLocation == null)
            {
                errorHandler.ReportError("Drop Off Failed: Landmark destination's cell location not set", ErrorState.close_window);
                return false;
            }

            if (dropOffCell == landmarkDestination.CellLocation)
            {
                dropOffSuccess = true;
            }
            else
            {
                Cell[] selectedCellNeighbours = worldMapGlobe.GetCellNeighbours(dropOffCell.index);
                foreach (Cell cell in selectedCellNeighbours)
                {
                    if (cell == landmarkDestination.CellLocation)
                    {
                        dropOffSuccess = true;
                        break;
                    }
                }
            }

            return dropOffSuccess;
        }

        /// <summary> 
        /// Check if a given location is a correct drop off location given a desired country
        /// </summary>
        /// <param name="dropOffCell"> The cell drop off cell </param>
        /// <param name="countryDestination"> The desired country destination </param>
        private bool AttemptCountryDropOff(Cell dropOffCell, Country countryDestination)
        {
            bool dropOffSuccess = false;

            if (countryDestination == null)
            {
                errorHandler.ReportError("Drop Off Failed: Country destination not set", ErrorState.close_window);
                return false;
            }

            List<Country> selectedCountries = countryParser.GetCountriesInCell(dropOffCell);
            if (selectedCountries == null)
            {
                errorHandler.ReportError("Drop Off Failed: No countries in cell", ErrorState.close_window);
                return false;
            }

            foreach (Country country in selectedCountries)
            {
                try
                {
                    if (country == countryDestination)
                    {
                        dropOffSuccess = true;
                    }
                }
                catch (System.Exception ex)
                {
                    errorHandler.CatchException(ex, ErrorState.close_window);
                    return false;
                }
            }
            return dropOffSuccess;
        }
    }

}

