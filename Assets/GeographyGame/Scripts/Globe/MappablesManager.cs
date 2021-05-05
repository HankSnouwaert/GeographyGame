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
        private IGlobeInfo globeInfo;
        private IPlayerManager playerManager;
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
                globeInfo = globeManager.GlobeInfo;
                if (globeInfo == null)
                    errorHandler.ReportError("Globe Info missing", ErrorState.restart_scene);

                playerManager = gameManager.PlayerManager;
                if(playerManager == null)
                    errorHandler.ReportError("Player Manager missing", ErrorState.restart_scene);

                worldMapGlobe = globeManager.WorldMapGlobe;
                if (worldMapGlobe == null)
                    errorHandler.ReportError("World Map Globe missing", ErrorState.restart_scene);
            }
        }

        public void IntantiateMappables(Country country)
        {
            int countryNameIndex = worldMapGlobe.GetCountryIndex(country.name);
            //worldGlobeMap.ReloadMountPointsData();
            List<MountPoint> countryMountPoints = new List<MountPoint>();
            int mountPointCount = worldMapGlobe.GetMountPoints(countryNameIndex, countryMountPoints);

            foreach (MountPoint mountPoint in countryMountPoints)
            {
                if (mountPoint.type == START_POINT && mountPoint.provinceIndex == worldMapGlobe.GetProvinceIndex(startingCountry, startingProvince))
                {
                    int startingCellIndex = worldMapGlobe.GetCellIndex(mountPoint.localPosition);
                    Cell startingCell = worldMapGlobe.cells[startingCellIndex];
                    bool playerInstantiated = playerManager.IntantiatePlayer(startingCell);
                    /*
                    GameObject playerObject = Instantiate(playerPrefab);
                    gameManager.Player = playerObject.GetComponent(typeof(IPlayerCharacter)) as IPlayerCharacter;
                    int startingCellIndex = worldMapGlobe.GetCellIndex(mountPoint.localPosition);
                    gameManager.Player.CellLocation = startingCellIndex;
                    gameManager.Player.Latlon = worldMapGlobe.cells[startingCellIndex].latlon;
                    Vector3 startingLocation = worldMapGlobe.cells[startingCellIndex].sphereCenter;
                    gameManager.Player.VectorLocation = startingLocation;
                    float playerSize = gameManager.Player.GetSize();
                    worldMapGlobe.AddMarker(playerObject, startingLocation, playerSize, false, 0.0f, true, true);
                    //string playerID = gameManager.Player.GetInstanceID().ToString();
                    worldMapGlobe.cells[startingCellIndex].occupants.Add(gameManager.Player);
                    //globeInfo.MappedObjects.Add(playerID, gameManager.Player);
                    */
                }
                if (mountPoint.type == CULTURAL_POINT) //&& loadedMapSettings.culturalLandmarks)
                {
                    string mountPointName = mountPoint.name;
                    string tempName = mountPointName.Replace("The", "");
                    tempName = tempName.Replace(" ", "");
                    var model = Resources.Load<GameObject>(landmarkFilePath + tempName);
                    var modelClone = Instantiate(model);
                    Landmark landmarkComponent = modelClone.GetComponent(typeof(Landmark)) as Landmark;
                    landmarkComponent.MountPoint = mountPoint;
                    landmarkComponent.ObjectName = mountPointName;
                    landmarkComponent.CellIndex = worldMapGlobe.GetCellIndex(mountPoint.localPosition);
                    landmarkComponent.CellLocation = worldMapGlobe.cells[landmarkComponent.CellIndex];
                    landmarkComponent.CellLocation.canCross = false;
                    worldMapGlobe.AddMarker(modelClone, mountPoint.localPosition, 0.001f, false, -5.0f, true, true);
                    string landmarkID = landmarkComponent.GetInstanceID().ToString();
                    worldMapGlobe.cells[landmarkComponent.CellIndex].occupants.Add(landmarkComponent);
                    globeInfo.MappedObjects.Add(landmarkID, landmarkComponent);
                    globeInfo.CulturalLandmarks.Add(landmarkID, landmarkComponent);
                    globeInfo.CulturalLandmarksByName.Add(landmarkComponent.ObjectName, landmarkComponent);
                }
            }
        }

    }
}


