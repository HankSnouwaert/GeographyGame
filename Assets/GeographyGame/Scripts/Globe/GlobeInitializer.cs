using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WPM
{
    public class GlobeInitializer : MonoBehaviour, IGlobeInitializer
    {
        //Internal Interface References
        public GameObject playerPrefab;
        private IGameManager gameManager;
        private WorldMapGlobe worldMapGlobe;
        private IGlobeManager globeManager;
        private IGlobeInfo globeInfo;
        private IPlayerManager playerManager;
        //Starting Info
        readonly string startingCountry = "United States of America";
        readonly string startingProvince = "North Carolina";
        //Path of Landmark Assets
        private string landmarkFilePath = "Prefabs/Landmarks/";
        //MACROS
        //Province Attributes
        public const int NUMBER_OF_PROVINCE_ATTRIBUTES = 3;
        public const int POLITICAL_PROVINCE = 0;
        public const int TERRAIN = 1;
        public const int CLIMATE = 2;
        //Mount Points
        public const int START_POINT = 0;
        public const int NATURAL_POINT = 1;
        public const int CULTURAL_POINT = 2;
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
                worldMapGlobe = globeManager.WorldMapGlobe;
                if (worldMapGlobe == null)
                    errorHandler.ReportError("World Map Globe missing", ErrorState.restart_scene);
                globeInfo = globeManager.GlobeInfo;
                if (globeInfo == null)
                    errorHandler.ReportError("Globe Info missing", ErrorState.restart_scene);
                playerManager = gameManager.PlayerManager;
                if (playerManager == null)
                    errorHandler.ReportError("Player Manager missing", ErrorState.restart_scene);
            }
        }

        public void ApplyGlobeSettings()
        {
            //if (File.Exists(Application.dataPath + "/student.txt"))
            //{
            //Load Settings
            /*
            string savedMapSettings = File.ReadAllText(Application.dataPath + "/student.txt");
            SaveObject loadedMapSettings = JsonUtility.FromJson<SaveObject>(savedMapSettings);
            bool[] provinceSettings = new bool[NUMBER_OF_PROVINCE_ATTRIBUTES];
            provinceSettings[POLITICAL_PROVINCE] = loadedMapSettings.provinces;
            provinceSettings[TERRAIN] = loadedMapSettings.terrain;
            provinceSettings[CLIMATE] = loadedMapSettings.climate;
            */
            bool[] provinceSettings = new bool[NUMBER_OF_PROVINCE_ATTRIBUTES];
            provinceSettings[POLITICAL_PROVINCE] = true;
            provinceSettings[TERRAIN] = false;
            provinceSettings[CLIMATE] = false;

            foreach (Country country in worldMapGlobe.countries)
            {
                if (country.continent == "North America")
                {
                    MergeProvincesInCountry(country, provinceSettings);
                    IntantiateMappables(country);
                }
            }
            //}
        }

        private void MergeProvincesInCountry(Country country, bool[] provinceSettings)
        {
            int countryIndex = worldMapGlobe.GetCountryIndex(country.name);
            if (countryIndex >= 0)
            {
                //Get all provinces for country and loop through them
                Province[] countryProvinces = worldMapGlobe.countries[countryIndex].provinces;
                List<Province> countryProvinceList = countryProvinces.ToList();

                Province province;
                List<Province> removedProvinces = new List<Province>();
                for(int i = 0; i < countryProvinceList.Count; i++)
                {
                    //Get province attributes
                    province = countryProvinceList[i];
                    removedProvinces = MergeProvinceWithNeighbors(province, provinceSettings);
                    if(removedProvinces.Count > 0)
                    {
                        foreach(Province removedProvince in removedProvinces)
                        {
                            countryProvinceList.Remove(removedProvince);
                        }
                    }
                }
            }
            worldMapGlobe.drawAllProvinces = false;
        }

        private List<Province> MergeProvinceWithNeighbors(Province province, bool[] provinceSettings)
        {
            string[] provinceAttributes = new string[NUMBER_OF_PROVINCE_ATTRIBUTES];
            int countryIndex = province.countryIndex;

            provinceAttributes[POLITICAL_PROVINCE] = province.attrib["PoliticalProvince"];
            if (provinceAttributes[POLITICAL_PROVINCE] == null) provinceAttributes[POLITICAL_PROVINCE] = "";
            provinceAttributes[TERRAIN] = province.attrib["Terrain"];
            if (provinceAttributes[TERRAIN] == null) provinceAttributes[TERRAIN] = "";
            provinceAttributes[CLIMATE] = province.attrib["Climate"];
            if (provinceAttributes[CLIMATE] == null) provinceAttributes[CLIMATE] = "";
            //Get all neighbors for province and loop through them
            int provinceIndex = worldMapGlobe.GetProvinceIndex(countryIndex, province.name);

            List<Province> neighbors = worldMapGlobe.ProvinceNeighboursOfMainRegion(provinceIndex);

            List<Province> removedProvinces = new List<Province>();

            foreach (Province neighbor in neighbors)
            {
                //Check that neighbor is in same country
                if (neighbor.countryIndex == countryIndex)
                {
                    //Default to assuming the neighbor will be merged
                    bool mergeNeighbor = true;

                    //Get neighbor attributes
                    string[] neighborAttributes = new string[NUMBER_OF_PROVINCE_ATTRIBUTES];
                    neighborAttributes[POLITICAL_PROVINCE] = neighbor.attrib["PoliticalProvince"];
                    if (neighborAttributes[POLITICAL_PROVINCE] == null) neighborAttributes[POLITICAL_PROVINCE] = "";
                    neighborAttributes[TERRAIN] = neighbor.attrib["Terrain"];
                    if (neighborAttributes[TERRAIN] == null) neighborAttributes[TERRAIN] = "";
                    neighborAttributes[CLIMATE] = neighbor.attrib["Climate"];
                    if (neighborAttributes[CLIMATE] == null) neighborAttributes[CLIMATE] = "";

                    //Loop through all attributes a province can have
                    int i = 0;
                    bool attributeSet;
                    while (i < NUMBER_OF_PROVINCE_ATTRIBUTES)
                    {
                        //If the attribute is being used AND is different between the province and its neighbor, 
                        // OR the attributes haven't been set for either province, abort the merge
                        attributeSet = provinceAttributes[i] != "" && neighborAttributes[i] != "";
                        if ((provinceAttributes[i] != neighborAttributes[i] && provinceSettings[i]) || !attributeSet)
                        {
                            mergeNeighbor = false;
                        }
                        i++;
                    }

                    //This is a temp fix for provinces that haven't had their political name put in yet
                    if (neighbor.attrib["PoliticalProvince"] == "")
                        neighbor.attrib["PoliticalProvince"] = neighbor.name;

                    if (mergeNeighbor)
                    {
                        //Merge provinces
                        worldMapGlobe.ProvinceTransferProvinceRegion(provinceIndex, neighbor.mainRegion, true);

                        removedProvinces.Add(neighbor);

                        /*
                        List<Province> provinceList = countryProvinces.ToList();
                        provinceList.Remove(neighbor);
                        countryProvinces = provinceList.ToArray();
                        */

                        //Clear unused attributes
                        //if (!loadedMapSettings.provinces)
                        //    neighbor.attrib["PoliticalProvince"] = "";
                        //if (!loadedMapSettings.terrain)
                        neighbor.attrib["Terrain"] = "";
                        //if (!loadedMapSettings.climate)
                        neighbor.attrib["Climate"] = "";
                    }
                }
            }
            return removedProvinces;
        }

        private void IntantiateMappables(Country country)
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
