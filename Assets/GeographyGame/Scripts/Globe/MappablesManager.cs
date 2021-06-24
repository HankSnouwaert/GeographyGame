using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPM
{
    public class MappablesManager : MonoBehaviour, IMappablesManager
    {
        //Internal Interface References
        public GameObject playerPrefab;
        private IGameManager gameManager;
        private WorldMapGlobe worldMapGlobe;
        private IGlobeManager globeManager;
        private IPlayerManager playerManager;
        //Public Interface References
        public ILandmarkManager LandmarkManager { get; protected set; }
        //Public Variables
        public Dictionary<string, MappableObject> MappedObjects { get; set; } = new Dictionary<string, MappableObject>();
        //Private Variables
        private bool started = false;
        private bool componentMissing = false;
        //Mount Points
        public const int START_POINT = 0;
        public const int NATURAL_POINT = 1;
        public const int CULTURAL_POINT = 2;
        //Path of Landmark Assets
        private string landmarkFilePath = "Prefabs/Landmarks/";
        //Starting Info
        readonly string startingCountry = "United States of America";
        readonly string startingProvince = "North Carolina";
        //Error Checking
        private InterfaceFactory interfaceFactory;
        private IErrorHandler errorHandler;

        private void Awake()
        {
            interfaceFactory = FindObjectOfType<InterfaceFactory>();
            if (interfaceFactory == null)
                gameObject.SetActive(false);
            try
            {
                LandmarkManager = GetComponent(typeof(ILandmarkManager)) as ILandmarkManager;
                if (LandmarkManager == null)
                    componentMissing = true;
            }
            catch
            {
                componentMissing = true;
            }
        }

        private void Start()
        {
            gameManager = interfaceFactory.GameManager;
            globeManager = interfaceFactory.GlobeManager;
            errorHandler = interfaceFactory.ErrorHandler;
            if (gameManager == null || globeManager == null || errorHandler == null)
                gameObject.SetActive(false);
            else
            {
                if (componentMissing == true)
                    errorHandler.ReportError("Mapplable Manager component missing", ErrorState.restart_scene);

                playerManager = gameManager.PlayerManager;
                if(playerManager == null)
                    errorHandler.ReportError("Player Manager missing", ErrorState.restart_scene);

                worldMapGlobe = globeManager.WorldMapGlobe;
                if (worldMapGlobe == null)
                    errorHandler.ReportError("World Map Globe missing", ErrorState.restart_scene);

                started = true;
            }
        }

        public void IntantiateMappables(Country country)
        {
            if (!started)
                Start();

            List<MountPoint> countryMountPoints = new List<MountPoint>();
            try
            {
                int countryIndex = worldMapGlobe.GetCountryIndex(country.name);
                int mountPointCount = worldMapGlobe.GetMountPoints(countryIndex, countryMountPoints);
            }
            catch
            {
                errorHandler.ReportError("Failed to load country's mount points", ErrorState.restart_scene);
                return;
            }

            foreach (MountPoint mountPoint in countryMountPoints)
            {
                if (mountPoint.type == START_POINT && mountPoint.provinceIndex == worldMapGlobe.GetProvinceIndex(startingCountry, startingProvince))
                {
                    int startingCellIndex = worldMapGlobe.GetCellIndex(mountPoint.localPosition);
                    if(startingCellIndex < 0 || startingCellIndex > worldMapGlobe.cells.Length)
                    {
                        errorHandler.ReportError("Invalid starting mount point", ErrorState.restart_scene);
                    }
                    else
                    {
                        Cell startingCell = worldMapGlobe.cells[startingCellIndex];
                        bool playerInstantiated = playerManager.IntantiatePlayer(startingCell);
                    }
                }
                if (mountPoint.type == CULTURAL_POINT) //&& loadedMapSettings.culturalLandmarks)
                {
                    LandmarkManager.InstantiateCulturalLandmark(mountPoint);
                   
                }
            }
        }

    }
}


