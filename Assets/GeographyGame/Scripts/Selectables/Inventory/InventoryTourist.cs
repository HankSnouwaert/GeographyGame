using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace WPM
{
    public class InventoryTourist : InventoryItem, IInventoryTourist
    {

        //Local Reference Interfaces
        private WorldMapGlobe worldMapGlobe;
        private IGlobeManager globeManager;
        private IDropOffUI dropOffUI;
        private ITouristManager touristManager;
        private IGlobeParser globeParser;
        private IProvinceParser provinceParser;
        private ICountryParser countryParser;
        private IGlobeInfo globeInfo;
        private IScoreManager scoreManager;
        private IInventoryPopUpUI inventoryPopUpUI;

        //Local Variables
        private Text dialog;
        private string destinationName;
        private int destinationIndex;
        private Province provinceDestination;
        private Landmark landmarkDestination;
        private Country countryDestination;
        private DestinationType destinationType;
        
        //Local Constants
        private const int PROVINCE_MULTIPLIER = 1;
        private const int LANDMARK_MULTIPLIER = 10;
        private const int COUNTRY_MULTIPLIER = 1;
        private const int TOURIST_DROP_OFF_SCORE = 100;

        protected override void Start()
        {
            base.Start();
            if (gameObject.activeSelf)
            {
                globeManager = interfaceFactory.GlobeManager;
                if (globeManager == null)
                    gameObject.SetActive(false);
                else
                {
                    dropOffUI = uiManager.DropOffUI;
                    if (dropOffUI == null)
                        errorHandler.ReportError("Drop Off UI missing", ErrorState.restart_scene);

                    globeParser = globeManager.GlobeParser;
                    if(globeParser == null)
                        errorHandler.ReportError("Globe Parser missing", ErrorState.restart_scene);
                    else
                    {
                        provinceParser = globeParser.ProvinceParser;
                        if(provinceParser == null)
                        {
                            errorHandler.ReportError("Province Parser missing", ErrorState.restart_scene);
                        }
                        countryParser = globeParser.CountryParser;
                        if (provinceParser == null)
                        {
                            errorHandler.ReportError("Country Parser missing", ErrorState.restart_scene);
                        }
                    }

                    globeInfo = globeManager.GlobeInfo;
                    if (globeInfo == null)
                        errorHandler.ReportError("Globe Info missing", ErrorState.restart_scene);

                    touristManager = gameManager.TouristManager;
                    if (touristManager == null)
                        errorHandler.ReportError("Tourist Manager missing", ErrorState.restart_scene);

                    scoreManager = gameManager.ScoreManager;
                    if (scoreManager == null)
                        errorHandler.ReportError("Score Manager missing", ErrorState.restart_scene);

                    inventoryPopUpUI = uiManager.InventoryPopUpUI;
                    if (inventoryPopUpUI == null)
                        errorHandler.ReportError("Inventory Pop Up UI missing", ErrorState.restart_scene);

                    worldMapGlobe = globeManager.WorldMapGlobe;
                    if (worldMapGlobe == null)
                        errorHandler.ReportError("World Map Globe missing", ErrorState.restart_scene);
                }

                bool destinationSet = SetDestination();
                if (destinationSet)
                {
                    inventoryPopUpUI.DisplayPopUp("Hey there!  I want to see " + destinationName + "!", false);
                    dropOffUI.ToggleOptionForDropOff(false);
                }
                else
                    errorHandler.ReportError("Tourist destination not set", ErrorState.close_window);
                
            }
        }
        
        public override void Select()
        {
            base.Select();
            dropOffUI.ToggleOptionForDropOff(true);
            dropOffUI.SetDropOffDelegate(AttemptDropOff);
            SetPopUpRequest(true);
        }

        public override void Deselect()
        {
            base.Deselect();
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            dropOffUI.ToggleOptionForDropOff(false);
            dropOffUI.ClearDropOffDelegate();
            inventoryPopUpUI.ClearPopUp(true);
        }

        public override void MouseEnter()
        {
            inventoryPopUpUI.DisplayPopUp("I want to see " + destinationName + "!", false);
        }
        
        public override void MouseExit()
        {
            inventoryPopUpUI.ClearPopUp(false);
        }
        
        public void SetPopUpRequest(bool persistant)
        {
            inventoryPopUpUI.DisplayPopUp("I want to see " + destinationName + "!", persistant);
        }

        public override void OnCellClick(int index)
        {
            if(index == playerCharacter.CellLocation.index)
            {
                playerCharacter.Select();
            }
        }

        public override void MouseDown()
        {
            Select();
        }

        public override void OnCellEnter(int index)
        {
            //Nothing Happens
        }

        public override void OnSelectableEnter(ISelectableObject selectableObject)
        {
            //Nothing Happens
        }

        public override void OtherObjectSelected(ISelectableObject selectedObject)
        {
            //There will need to be check later to account for multiple object selection
        }

        private bool SetDestination()
        {
            bool destinationSet = false;

            //Get possible Provinces
            List<int> provinceChoices = GetProvinceDestinations(touristManager.CurrentRegion, touristManager.RecentProvinceDestinations);
            //Get possible Landmarks
            List<string> landmarkChoices = GetLandmarkDestinations(touristManager.CurrentRegion, touristManager.RecentLandmarkDestinations);
            //Get possible Countries
            List<int> countryChoices = GetCountryDestinations(touristManager.CurrentRegion, touristManager.RecentCountryDestinations);

            //Treat all three lists as a combined list and get a random number that points to an element from one of them
            destinationIndex = Random.Range(0, provinceChoices.Count + landmarkChoices.Count + countryChoices.Count);
            if (destinationIndex < provinceChoices.Count)
            {
                destinationSet = SetProvinceDestination(provinceChoices[destinationIndex]);
            }
            else if ((destinationIndex >= provinceChoices.Count) && (destinationIndex < (landmarkChoices.Count + provinceChoices.Count)))
            {
                destinationIndex = destinationIndex - provinceChoices.Count;
                destinationSet = SetLandmarkDestination(landmarkChoices[destinationIndex]);
            }
            else if (destinationIndex >= provinceChoices.Count + landmarkChoices.Count)
            {
                destinationIndex = destinationIndex - provinceChoices.Count - landmarkChoices.Count;
                destinationSet = SetCountryDestination(countryChoices[destinationIndex]);
            }
            else
            {
                errorHandler.ReportError("Unable to Set Tourist Destination.", ErrorState.restart_scene);
            }
            return destinationSet;
        }

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

        private bool SetProvinceDestination(int provinceIndex)
        {
            destinationType = DestinationType.province;
            try
            {
                provinceDestination = worldMapGlobe.provinces[provinceIndex];
            }
            catch
            {
                errorHandler.ReportError("Error Setting Province Destination", ErrorState.close_window);
                return false;
            }
            destinationName = provinceDestination.name;
            //Update List of Recent Province Destinations
            touristManager.RecentProvinceDestinations.Insert(0, provinceIndex);
            while (touristManager.RecentProvinceDestinations.Count >= touristManager.TrackingTime)
            {
                touristManager.RecentProvinceDestinations.RemoveAt(touristManager.TrackingTime - 1);
            }
            return true;
        }

        private bool SetLandmarkDestination(string landmark)
        {
            destinationType = DestinationType.landmark;
            try
            {
                landmarkDestination = globeInfo.CulturalLandmarksByName[landmark];
            }
            catch
            {
                errorHandler.ReportError(landmark + " does not exist", ErrorState.close_window);
                return false;
            }
            destinationName = landmarkDestination.ObjectName;
            //Update List of Recent Landmark Destinations
            touristManager.RecentLandmarkDestinations.Insert(0, landmark);
            while (touristManager.RecentLandmarkDestinations.Count >= touristManager.TrackingTime)
            {
                touristManager.RecentLandmarkDestinations.RemoveAt(touristManager.TrackingTime - 1);
            }
            return true;
        }

        private bool SetCountryDestination(int countryIndex)
        {
            destinationType = DestinationType.country;
            try
            {
                countryDestination = worldMapGlobe.countries[countryIndex];
            }
            catch
            {
                errorHandler.ReportError("Country does not exist", ErrorState.close_window);
                return false;
            }
            destinationName = countryDestination.name;
            //Update List of Recent Country Destinations
            touristManager.RecentCountryDestinations.Insert(0, countryIndex);
            while (touristManager.RecentCountryDestinations.Count >= touristManager.TrackingTime)
            {
                touristManager.RecentCountryDestinations.RemoveAt(touristManager.TrackingTime - 1);
            }
            return true;
        }

        public void AttemptDropOff()
        {
            //Clear the Event System so that it gets updated with the tourist if the drop off fails
            EventSystem.current.SetSelectedGameObject(null);

            Cell playerCell = playerCharacter.CellLocation;
            if (playerCharacter.CellLocation == null)
            {
                errorHandler.ReportError("Drop Off Failed: Player in invalid cell", ErrorState.close_window);
                return;
            }

            switch (destinationType)
            {
                case DestinationType.province:
                    AttemptProvinceDropOff(playerCell);
                    break;

                case DestinationType.landmark:
                    AttemptLandmarkDropOff(playerCell);
                    break;

                case DestinationType.country:
                    AttemptCountryDropOff(playerCell);
                    break;
                default:
                    break;
            }
        }

        private void AttemptProvinceDropOff(Cell dropOffCell)
        {
            if(provinceDestination == null)
            {
                errorHandler.ReportError("Drop Off Failed: Province destination not set", ErrorState.close_window);
                return;
            }

            List<int> selectedProvinces = provinceParser.GetProvicesInCell(dropOffCell.index);
            if (selectedProvinces == null)
            {
                errorHandler.ReportError("Drop Off Failed: No provinces in cell", ErrorState.close_window);
                return;
            }

            bool correctProvince = false;
            foreach (int provinceIndex in selectedProvinces)
            {
                try
                {
                    if (worldMapGlobe.provinces[provinceIndex] == provinceDestination)
                    {
                        DropOffSuccess();
                        correctProvince = true;
                        break;
                    }
                }
                catch (System.Exception ex)
                {
                    errorHandler.CatchException(ex, ErrorState.close_window);
                    return;
                }
            }
            if (correctProvince == false)
            {
                DropOffFailure();
            }
        }

        private void AttemptLandmarkDropOff(Cell dropOffCell)
        {
            if (landmarkDestination == null)
            {
                errorHandler.ReportError("Drop Off Failed: Landmark destination not set", ErrorState.close_window);
                return;
            }

            if (landmarkDestination.CellLocation == null)
            {
                errorHandler.ReportError("Drop Off Failed: Landmark destination's cell location not set", ErrorState.close_window);
                return;
            }

            bool landmarkReached = false;
            if (dropOffCell == landmarkDestination.CellLocation)
            {
                DropOffSuccess();
                landmarkReached = true;
            }
            else
            {
                Cell[] selectedCellNeighbours = worldMapGlobe.GetCellNeighbours(dropOffCell.index);
                foreach (Cell cell in selectedCellNeighbours)
                {
                    if (cell == landmarkDestination.CellLocation)
                    {
                        DropOffSuccess();
                        landmarkReached = true;
                        break;
                    }      
                }
            }

            if (landmarkReached == false)
            {
                DropOffFailure();
            }
        }

        private void AttemptCountryDropOff(Cell dropOffCell)
        {
            if (countryDestination == null)
            {
                errorHandler.ReportError("Drop Off Failed: Country destination not set", ErrorState.close_window);
                return;
            }  

            List<int> selectedCountries = countryParser.GetCountriesInCell(dropOffCell.index);
            if(selectedCountries == null)
            {
                errorHandler.ReportError("Drop Off Failed: No countries in cell", ErrorState.close_window);
                return;
            }

            bool correctCountry = false;
            foreach (int countryIndex in selectedCountries)
            {
                try
                {
                    if (worldMapGlobe.countries[countryIndex] == countryDestination)
                    {
                        DropOffSuccess();
                        correctCountry = true;
                    }
                }
                catch(System.Exception ex)
                {
                    errorHandler.CatchException(ex, ErrorState.close_window);
                    return;
                }
            }
            if (correctCountry == false)
            {
                DropOffFailure();
            }
        }

        private void DropOffSuccess()
        {
            Deselect();
            //Remove Tourist from Inventory
            playerCharacter.RemoveItem(InventoryLocation);
            scoreManager.UpdateScore(TOURIST_DROP_OFF_SCORE);
            uiManager.CursorOverUI = false;
            inventoryPopUpUI.DisplayPopUp("Exactly where I wanted to go!", false);

            /*  This will be for drop off sound effects
            if (success)
                dropOffSuccess.Play();
            else
                dropOffFailure.Play();
            */
        }

        private void DropOffFailure()
        {
            inventoryPopUpUI.DisplayPopUp("Well this doesn't look right. . . .", false);
            return;
        }

    }
}
