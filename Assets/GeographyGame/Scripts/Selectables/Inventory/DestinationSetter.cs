using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPM
{
    public class DestinationSetter : MonoBehaviour, IDestinationSetter
    {
        //Interal Interface References
        private IGlobeManager globeManager;
        private IGlobeInfo globeInfo;
        private WorldMapGlobe worldMapGlobe;
        private IGameManager gameManager;
        private ITouristManager touristManager;
        //Local Variables
        private int destinationIndex;
        private bool started = false;
        //Local Constants
        private const int PROVINCE_MULTIPLIER = 1;
        private const int LANDMARK_MULTIPLIER = 10;
        private const int COUNTRY_MULTIPLIER = 1;
        //Error Checking
        private InterfaceFactory interfaceFactory;
        private IErrorHandler errorHandler;
        private bool componentMissing = false;

        private void Awake()
        {
            interfaceFactory = FindObjectOfType<InterfaceFactory>();
            if (interfaceFactory == null)
                gameObject.SetActive(false);
        }

        private void Start()
        {
            gameManager = interfaceFactory.GameManager;
            errorHandler = interfaceFactory.ErrorHandler;
            globeManager = interfaceFactory.GlobeManager;
            if(gameManager == null || errorHandler == null || globeManager == null)
            {
                gameObject.SetActive(false);
            }
            else
            {
                touristManager = gameManager.TouristManager;
                if (touristManager == null)
                    errorHandler.ReportError("Tourist Manager missing", ErrorState.close_window);

                worldMapGlobe = globeManager.WorldMapGlobe;
                if (worldMapGlobe == null)
                    errorHandler.ReportError("World Map Globe missing", ErrorState.close_window);

                globeInfo = globeManager.GlobeInfo;
                if (globeInfo == null)
                    errorHandler.ReportError("Globe Info missing", ErrorState.close_window);

                started = true;
            }
        }

        public bool SetDestination(IInventoryTourist tourist, TouristRegion touristRegion)
        {
            if(tourist == null)
            {
                errorHandler.ReportError("Unable to Set Tourist Destination.", ErrorState.restart_scene);
                return false;
            }

            if (started == false)
                Start();

            bool destinationSet = false;

            //Get possible Provinces
            List<int> provinceChoices = GetProvinceDestinations(touristRegion, touristManager.RecentProvinceDestinations);
            //Get possible Landmarks
            List<string> landmarkChoices = GetLandmarkDestinations(touristRegion, touristManager.RecentLandmarkDestinations);
            //Get possible Countries
            List<int> countryChoices = GetCountryDestinations(touristRegion, touristManager.RecentCountryDestinations);

            //Treat all three lists as a combined list and get a random number that points to an element from one of them
            destinationIndex = Random.Range(0, provinceChoices.Count + landmarkChoices.Count + countryChoices.Count);
            if (destinationIndex < provinceChoices.Count)
            {
                destinationSet = SetProvinceDestination(provinceChoices[destinationIndex], tourist);
            }
            else if ((destinationIndex >= provinceChoices.Count) && (destinationIndex < (landmarkChoices.Count + provinceChoices.Count)))
            {
                destinationIndex = destinationIndex - provinceChoices.Count;
                destinationSet = SetLandmarkDestination(landmarkChoices[destinationIndex], tourist);
            }
            else if (destinationIndex >= provinceChoices.Count + landmarkChoices.Count)
            {
                destinationIndex = destinationIndex - provinceChoices.Count - landmarkChoices.Count;
                destinationSet = SetCountryDestination(countryChoices[destinationIndex], tourist);
            }
            else
            {
                errorHandler.ReportError("Unable to Set Tourist Destination.", ErrorState.restart_scene);
            }
            return destinationSet;
        }

        /// <summary> 
        /// Gets possible province destinations from a given tourist region
        /// </summary>
        /// <param name="touristRegion"> The tourist region the province is being selected from</param>
        /// <param name="recentProvinceDestinations"> A list of recent province destinations used to avoid picking a 
        /// recent destination</param>
        private List<int> GetProvinceDestinations(TouristRegion touristRegion, List<int> recentProvinceDestinations)
        {
            if (recentProvinceDestinations == null)
            {
                errorHandler.ReportError("Recent province destinations missing", ErrorState.restart_scene);
                return null;
            }
            if (touristRegion.provinces == null)
            {
                errorHandler.ReportError("Tourist region provinces missing", ErrorState.restart_scene);
                return null;
            }

            List<int> provinceDestinations = new List<int>();

            foreach (int province in touristRegion.provinces)
            {

                //Get Time Multiplier
                int timeMultiplier = recentProvinceDestinations.IndexOf(province);

                //Check if destination is no longer being tracked
                if (timeMultiplier < 0)
                    timeMultiplier = touristManager.TrackingTime;

                int totalMultiplier = timeMultiplier * PROVINCE_MULTIPLIER;

                //Add Province a Number of Times Equal to Total Multiplier
                for (int n = 0; n < totalMultiplier; n++)
                {
                    provinceDestinations.Add(province);
                }
            }

            return provinceDestinations;
        }

        /// <summary> 
        /// Gets possible landmark destinations from a given tourist region
        /// </summary>
        /// <param name="touristRegion"> The tourist region the landmark is being selected from</param>
        /// <param name="recentLandmarkDestinations"> A list of recent landmark destinations used to avoid picking a 
        /// recent destination</param>
        private List<string> GetLandmarkDestinations(TouristRegion touristRegion, List<string> recentLandmarkDestinations)
        {
            if (recentLandmarkDestinations == null)
            {
                errorHandler.ReportError("Recent landmark destinations missing", ErrorState.restart_scene);
                return null;
            }
            if (touristRegion.landmarks == null)
            {
                errorHandler.ReportError("Tourist region landmarks missing", ErrorState.restart_scene);
                return null;
            }

            List<string> landmarkDestinations = new List<string>();

            foreach (string landmark in touristRegion.landmarks)
            {
                //Get Time Multiplier
                int timeMultiplier = recentLandmarkDestinations.IndexOf(landmark);

                //Check if destination is no longer being tracked
                if (timeMultiplier < 0)
                    timeMultiplier = touristManager.TrackingTime;

                int totalMultiplier = timeMultiplier * LANDMARK_MULTIPLIER;

                //Add Landmark a Number of Times Equal to Total Multiplier
                for (int n = 0; n < totalMultiplier; n++)
                {
                    landmarkDestinations.Add(landmark);
                }
            }

            return landmarkDestinations;
        }

        /// <summary> 
        /// Gets possible country destinations from a given tourist region
        /// </summary>
        /// <param name="touristRegion"> The tourist region the country is being selected from</param>
        /// <param name="recentProvinceDestinations"> A list of recent country destinations used to avoid picking a 
        /// recent destination</param>
        private List<int> GetCountryDestinations(TouristRegion touristRegion, List<int> recentCountryDestinations)
        {
            if (recentCountryDestinations == null)
            {
                errorHandler.ReportError("Recent country destinations missing", ErrorState.restart_scene);
                return null;
            }
            if (touristRegion.countries == null)
            {
                errorHandler.ReportError("Tourist region countries missing", ErrorState.restart_scene);
                return null;
            }

            List<int> countryDestinations = new List<int>();

            foreach (int country in touristManager.CurrentRegion.countries)
            {
                //Get Time Multiplier
                int timeMultiplier = touristManager.RecentCountryDestinations.IndexOf(country);

                //Check if destination is no longer being tracked
                if (timeMultiplier < 0)
                    timeMultiplier = touristManager.TrackingTime;

                int totalMultiplier = timeMultiplier * COUNTRY_MULTIPLIER;

                //Add Country a Number of Times Equal to Total Multiplier
                for (int n = 0; n < totalMultiplier; n++)
                {
                    countryDestinations.Add(country);
                }
            }
            return countryDestinations;
        }

        /// <summary> 
        /// Sets a tourists desired destination to a given province
        /// </summary>
        /// <param name="provinceIndex"> The province being set as the destination </param>
        /// <param name="tourist"> The tourist whose destination is being set </param>
        private bool SetProvinceDestination(int provinceIndex, IInventoryTourist tourist)
        {
            tourist.DestinationType = DestinationType.province;
            try
            {
                tourist.ProvinceDestination = worldMapGlobe.provinces[provinceIndex];
            }
            catch
            {
                errorHandler.ReportError("Error Setting Province Destination", ErrorState.close_window);
                return false;
            }
            tourist.DestinationName = tourist.ProvinceDestination.name;
            //Update List of Recent Province Destinations
            touristManager.RecentProvinceDestinations.Insert(0, provinceIndex);
            while (touristManager.RecentProvinceDestinations.Count >= touristManager.TrackingTime)
            {
                touristManager.RecentProvinceDestinations.RemoveAt(touristManager.TrackingTime - 1);
            }
            return true;
        }

        /// <summary> 
        /// Sets a tourists desired destination to a given landmark
        /// </summary>
        /// <param name="landmark"> The landmark being set as the destination </param>
        /// <param name="tourist"> The tourist whose destination is being set </param>
        private bool SetLandmarkDestination(string landmark, IInventoryTourist tourist)
        {
            tourist.DestinationType = DestinationType.landmark;
            try
            {
                tourist.LandmarkDestination = globeInfo.CulturalLandmarksByName[landmark];
            }
            catch
            {
                errorHandler.ReportError(landmark + " does not exist", ErrorState.close_window);
                return false;
            }
            tourist.DestinationName = tourist.LandmarkDestination.ObjectName;
            //Update List of Recent Landmark Destinations
            touristManager.RecentLandmarkDestinations.Insert(0, landmark);
            while (touristManager.RecentLandmarkDestinations.Count >= touristManager.TrackingTime)
            {
                touristManager.RecentLandmarkDestinations.RemoveAt(touristManager.TrackingTime - 1);
            }
            return true;
        }

        /// <summary> 
        /// Sets a tourists desired destination to a given country
        /// </summary>
        /// <param name="provinceIndex"> The country being set as the destination </param>
        /// <param name="tourist"> The tourist whose destination is being set </param>
        private bool SetCountryDestination(int countryIndex, IInventoryTourist tourist)
        {
            tourist.DestinationType = DestinationType.country;
            try
            {
                tourist.CountryDestination = worldMapGlobe.countries[countryIndex];
            }
            catch
            {
                errorHandler.ReportError("Country does not exist", ErrorState.close_window);
                return false;
            }
            tourist.DestinationName = tourist.CountryDestination.name;
            //Update List of Recent Country Destinations
            touristManager.RecentCountryDestinations.Insert(0, countryIndex);
            while (touristManager.RecentCountryDestinations.Count >= touristManager.TrackingTime)
            {
                touristManager.RecentCountryDestinations.RemoveAt(touristManager.TrackingTime - 1);
            }
            return true;
        }
    }
}


