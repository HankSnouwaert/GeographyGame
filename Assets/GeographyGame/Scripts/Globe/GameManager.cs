﻿using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SpeedTutorMainMenuSystem;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace WPM
{
    public class GameManager : MonoBehaviour
    {
        #region Variable Declaration 
        //[Header("Player Components")]
        public WorldMapGlobe worldGlobeMap;
        public GameObject uiManagerObject;
        public IUIManager UIManager { get; set; }
        private GlobeManager globeManager;  //WILL NEED INTERFACE LATER
        public GameObject errorHandlerObject;
        public IErrorHandler ErrorHandler { get; set; }
        public GameObject touristManagerObject;
        public ITouristManager TouristManager { get; set; }
        public GameObject cameraManagerObject;
        public ICameraManager CameraManager { get; set; }
        public GameObject playerPrefab;
        public GameObject errorPanel;
        public InventoryUI inventoryUI;
        public AudioSource dropOffSuccess;
        public AudioSource dropOffFailure;
        //TO BE SORTED
        private ICellClicker cellClicker;
        public ICellCursorInterface CellCursorInterface { get; set; }
        public List<ITurnBasedObject> TurnBasedObjects { get; set; } = new List<ITurnBasedObject>();
        //Panel Messages
        private Text errorMessage;  //Error Manager
        private InputField stackTraceInputField; //Error Manager
        //Prefabs
        //private InventoryTourist touristPrefab; //Tourist Manager
        //Counters
        private int globalTurnCounter = 0; 
        //private int touristCounter = 0; //Tourist Manager
        public int score = 0;
        //private int touristsInCurrentRegion = -2;  //This number is the starting number of tourists * -1  (Tourist Manager)
        private int turnsRemaining = 250;
        //private int touristImageIndex = 0; //Tourist Manager
        //Flags
        public bool GamePaused { get; set; } = false;
        public bool GameMenuOpen { get; set; } = false;
        public ErrorState errorState = ErrorState.close_window;  //Error Manager
        //Game Settings
        //private int touristSpawnRate = 10; //Number of rounds for a tourist to spawn  (Tourist Manager)
        //public int TrackingTime { get; } = 10; //Number of rounds a tourist is remembered  (Tourist Manager)
        //public const int MIN_TIME_IN_REGION = 5;  //Tourist Manager
        //public const int MAX_TIME_IN_REGION = 10;  //Tourist Manager
        //In-Game Objects
        public PlayerCharacter player;
        private SelectableObject selectedObject;
        public SelectableObject SelectedObject
        {
            get { return selectedObject; }
            set
            {
                selectedObject = value;
                if (selectedObject != null)
                    cellClicker.NewObjectSelected = true;
            }
        }
        public SelectableObject HighlightedObject { get; set; } = null;

        //public Dictionary<string, MappableObject> mappedObjects = new Dictionary<string, MappableObject>();  //Globe Manager: GlobeInfo
        
        //Tourist Tracking Lists
        //public List<int> RecentProvinceDestinations { get; set; } = new List<int>();  //Tourist Manager
        //public List<string> RecentLandmarkDestinations { get; set; } = new List<string>();  //Tourist Manager
        //public List<int> RecentCountryDestinations { get; set; } = new List<int>();  //Tourist Manager
        //Map Regions
        //public List<TouristRegion> touristRegions = new List<TouristRegion>();  //Tourist Manager
        //public TouristRegion CurrentRegion { get; set; }  //Tourist Manager
        //private List<TouristRegion> regionsVisited = new List<TouristRegion>();  //Tourist Manager
        //Landmark Lists
        //public Dictionary<string, Landmark> culturalLandmarks = new Dictionary<string, Landmark>();  //Globe Manager: GlobeInfo
        //public Dictionary<string, Landmark> CulturalLandmarksByName { get; } = new Dictionary<string, Landmark>(); //Globe Manager: GlobeInfo
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
        //public const string CELL_PLAYER = "Player";
        //Error States
        public const int CLOSE_WINDOW = 0;
        public const int RESTART_SCENE = 1;
        public const int CLOSE_APPLICATION = 2;

        //Tourist Image Management
        //private string[] touristImageFiles; //Tourist Manager
        //private const int NUMBER_OF_TOURIST_IMAGES = 8; //Tourist Manager

        static GameManager _instance;

        #endregion

        /// <summary>
        /// Instance of the game manager. Use this property to access World Map functionality.
        /// </summary>
        public static GameManager instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<GameManager>();
                    if (_instance == null)
                    {
                        Debug.LogWarning("'GameManger' GameObject could not be found in the scene. Make sure it's created with this name before using any map functionality.");
                    }
                }
                return _instance;
            }
        }

        private void Awake()
        {
            globeManager = FindObjectOfType<GlobeManager>();
            ErrorHandler = errorHandlerObject.GetComponentInChildren(typeof(IErrorHandler)) as IErrorHandler;
            UIManager = uiManagerObject.GetComponent(typeof(IUIManager)) as IUIManager;
            TouristManager = touristManagerObject.GetComponent(typeof(ITouristManager)) as ITouristManager;
            CameraManager = cameraManagerObject.GetComponent(typeof(ICameraManager)) as ICameraManager;
        }

        void Start()
        {
            cellClicker = globeManager.CellCursorInterface.CellClicker;
        }

        void Update()
        {
            //Esc out of Selected Objects and UI Menus
            if (Input.GetKeyDown("escape"))
            {
                if (GameMenuOpen)
                    UIManager.ExitCurrentUI();
                else
                {
                    if (selectedObject != null)
                        selectedObject.Deselected();
                    else
                    {
                        UIManager.GameMenuUI.OpenUI();
                        GameMenuOpen = true;
                        GamePaused = true;
                    }
                }      
            }
        }


        /// <summary>
        /// Called whenever a new turn happens in game. Multiple turns can pass at once.
        /// Inputs:
        ///     turns: How many turns are passing
        /// </summary>
        public void NextTurn(int turns)
        {
            globalTurnCounter = globalTurnCounter + turns;
            UpdateRemainingTurns(turns*-1);
            //Run any end of turn scripts for the rest of the game's objects
            foreach(ITurnBasedObject turnBasedObject in TurnBasedObjects)
            {
                turnBasedObject.EndOfTurn(turns);
            }
            /*
            SelectableObject[] selectableObjects = FindObjectsOfType<SelectableObject>();
                foreach(SelectableObject selectableObject in selectableObjects)
                {
                    selectableObject.EndOfTurn(turns);
                }
            */
        }

        
        //THIS SHOULD BE REMOVED

        /// <summary>
        /// Called whenever the selected object needs to be cleared
        /// </summary>
        public void DeselectObject()
        {
            SelectedObject = null;
        }

        //RANGE CHECKING
        /*
        /// <summary> 
        /// Get all cells within a certain range (measured in cells) of a target cell
        /// Inputs:
        ///     startCell:  Target cell the range is being measured from
        ///     range:      The range (in cells) out from startCell that the method increments through
        /// Outputs:
        ///     cells:  An array of lists, with List0 containing all cells within range
        ///             and ListX containing the cells X number of cells away from the target cell
        /// </summary>
        public List<int>[] GetCellsInRange(int startCell, int range = 0)
        {
            if (range < 0 || startCell < 0 || worldGlobeMap.cells.Count() < startCell)
            {
                //This will need to be replaced with an error message
                Debug.LogWarning("Invalid input for GetCellsInRange");
                return null;
            }

            int distance = 0;                           //distance measures how many rings of hexes we've moved out
            List<int>[] cells = new List<int>[range+1]; //cells is an array of lists with each list other than 0 containing 
                                                        //the ring of hexes at that distance.  List 0 contains
                                                        //all hexes at every distance
            //Add the startCell to List0
            cells[0] = new List<int>();
            cells[0].Add(startCell);
            worldGlobeMap.cells[startCell].flag = true;

            if(range > 0)
            {
                //Add the neighbors of the start cell to List1
                //And add them to List0
                distance++;
                cells[distance] = new List<int>();
                foreach (Cell neighbour in worldGlobeMap.GetCellNeighbours(startCell))
                {
                    cells[0].Add(neighbour.index);
                    cells[distance].Add(neighbour.index);
                    neighbour.flag = true;
                }
            }
            while (distance < range)
            {
                //Continue adding rings of hexes to List0 and creating a new
                //List for each ring until the distance checked equals the 
                //disired range
                distance++;
                cells[distance] = new List<int>();
                foreach (int cell in cells[distance - 1])
                {
                    foreach (Cell neighbour in worldGlobeMap.GetCellNeighbours(cell))
                    {
                        if (!neighbour.flag)
                        {
                            cells[0].Add(neighbour.index);
                            cells[distance].Add(neighbour.index);
                            neighbour.flag = true;
                        }
                    }
                }
            }
            //Lower all cell flags
            foreach (int cell in cells[0])
            {
                worldGlobeMap.cells[cell].flag = false;
            }
            return cells;
        }

        /// <summary> 
        /// Get all provinces within a certain range (measured in cells) of a target cell
        /// Inputs:
        ///     startCell:  Target cell the range is being measured from
        ///     range:      The range (in cells) out from startCell that the method increments through
        /// Outputs:
        ///     provinces:  An array of lists, with ListX containing the provinces reachable within
        ///                 X number of cells away from the target cell
        /// </summary>
        public List<int>[] GetProvincesInRange(int startCell, List<int>[] cellRange)
        {
            int range = cellRange.Length;

            if (range < 0 || startCell < 0 || worldGlobeMap.cells.Count() < startCell)
            {
                //This will need to be replaced with an error message
                Debug.LogWarning("Invalid input for GetProvincesInRange");
                return null;
            }

            int distance = 0;                                      //distance measures how many rings of hexes we've moved out
            List<int>[] provinces = new List<int>[range + 1];      //provinces is an array of lists with each list containing 
            List<int> foundProvinces = new List<int>();             //the provinces that can be reached at that distance.  
            List<int> provincesInHex = new List<int>();

            bool startHex = true;
            foreach( List<int> hexRing in cellRange)
            {
                if (startHex)
                {
                    //Get provinces at start hex
                    provinces[0] = new List<int>();
                    provincesInHex = GetProvicesInCell(startCell);
                    foreach (int provinceIndex in provincesInHex)
                    {
                        foundProvinces.Add(provinceIndex);
                        provinces[0].Add(provinceIndex);
                    }
                    startHex = false;
                }
                else
                {
                    distance++;
                    provinces[distance] = new List<int>();
                    foreach (int cellIndex in hexRing)
                    {
                        //Check if there is a path from the start cell to this one
                        if (worldGlobeMap.FindPath(startCell, cellIndex) != null)
                        {
                            provincesInHex = GetProvicesInCell(cellIndex);
                            foreach (int provinceIndex in provincesInHex)
                            {
                                //Check that this province hasn't already been added
                                if (!foundProvinces.Contains(provinceIndex))
                                {
                                    foundProvinces.Add(provinceIndex);
                                    provinces[distance].Add(provinceIndex);
                                }
                            }
                        } 
                    }
                }
            }
            return provinces;
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

            if (range < 0 || startCell < 0 || worldGlobeMap.cells.Count() < startCell)
            {
                //This will need to be replaced with an error message
                Debug.LogWarning("Invalid input for GetProvincesInRange");
                return null;
            }

            List<int>[] countryIndexes = new List<int>[range + 1];      //provinces is an array of lists with each list containing 
            List<int> foundCountryIndexes = new List<int>();            //the provinces that can be reached at that distance. 

            List<int>[] provinceIndexes = GetProvincesInRange(startCell, cellRange);

            //Create lists of the countries within range based off of the provinces in range
            int i = 0;
            Province province;
            foreach(List<int> indexList in countryIndexes)
            {
                foreach(int provinceIndex in provinceIndexes[i])
                {
                    province = worldGlobeMap.provinces[provinceIndex];
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
            /// Get all landmarks within a certain range (measured in cells) of a target cell
            /// Inputs:
            ///     startCell:  Target cell the range is being measured from
            ///     range:      The range (in cells) out from startCell that the method increments through
            /// Outputs:
            ///     landmarks:  An array of lists, with ListX containing the landmarks within
            ///                 X number of cells from the target cell
            /// </summary>
        public List<Landmark>[] GetLandmarksInRange(int startCell, List<int>[] cellRange)
        {
            int range = cellRange.Length;

            if (range < 0 || startCell < 0 || worldGlobeMap.cells.Count() < startCell)
            {
                //This will need to be replaced with an error message
                Debug.LogWarning("Invalid input for GetCellsInRange");
                return null;
            }

            List<Landmark> landmarksFound = new List<Landmark>();
            List<Landmark> landmarksInCell;
            int distance = 0;                                           //distance measures how many rings of hexes we've moved out
            List<Landmark>[] landmarks = new List<Landmark>[range + 1];     //landmarks is an array of lists with each list containing 
            landmarks[0] = new List<Landmark>();                          //the landmarks that can be reached at that distance.  

            bool startHex = true;
            foreach (List<int> hexRing in cellRange)
            {
                if (startHex)
                {
                    //Get landmarks at start hex
                    landmarksInCell = GetLandmarksInCell(startCell);
                    startHex = false;
                    landmarks[0] = landmarksInCell;
                }
                else
                {
                    distance++;
                    landmarks[distance] = new List<Landmark>();
                    foreach(int cellIndex in hexRing)
                    {
                        landmarksInCell = GetLandmarksInCell(cellIndex);
                        foreach(Landmark landmark in landmarksInCell)
                        {
                            if (!landmarksFound.Contains(landmark))
                                landmarks[distance].Add(landmark);
                        }
                    }
                }
            }
            return landmarks;

        }

        //CELL CHECKING

        /// <summary> 
        /// Get all provinces that overlap with a given cell
        /// Inputs:
        ///     cellIndex:  Index of the cell in question
        /// Outputs:
        ///     provinces:  An array of provinces that overlap with the cell in quesiton
        /// </summary>
        public List<int> GetProvicesInCell(int cellIndex)
        {
            List<int> foundProvinceIndexes = new List<int>();
            List<int> checkedProvinceIndexes = new List<int>();
            int provinceIndex;
            int countryIndex;
            int neighborIndex;

            //Create a list of points including each vertex of the cell and its center point
            List<Vector3> cellPoints = worldGlobeMap.cells[cellIndex].vertices.ToList();
            cellPoints.Add(worldGlobeMap.cells[cellIndex].sphereCenter);

            foreach(Vector3 cellPoint in cellPoints)
            {
                provinceIndex = worldGlobeMap.GetProvinceIndex(cellPoint);
                //Check if cell point is on a province
                if (provinceIndex == -1)
                {
                    //Get closest province to point if it is not centered on one
                    provinceIndex = worldGlobeMap.GetProvinceNearPoint(cellPoint);
                }
                //Add the province, if it is not already in the province list
                if (!foundProvinceIndexes.Contains(provinceIndex))
                    foundProvinceIndexes.Add(provinceIndex);

                //Check to see if neighbours of province overlap with cell
                List<Province> provinceNeighbours = worldGlobeMap.ProvinceNeighbours(provinceIndex);
                bool provinceOverlaps;
                foreach (Province neighbor in provinceNeighbours)
                {
                    countryIndex = neighbor.countryIndex;
                    neighborIndex = worldGlobeMap.GetProvinceIndex(countryIndex, neighbor.name);
                    //Make sure you haven't already checked the province, this saves time
                    if (!checkedProvinceIndexes.Contains(neighborIndex))
                    {
                        checkedProvinceIndexes.Add(neighborIndex);
                        provinceOverlaps = false;

                        foreach (Region region in neighbor.regions)
                        {
                            foreach (Vector3 spherePoint in region.spherePoints)
                            {
                                if (worldGlobeMap.GetCellIndex(spherePoint) == cellIndex)
                                {
                                    provinceOverlaps = true;
                                    break;
                                }
                            }
                            if (provinceOverlaps)
                                break;
                        }
                        if (provinceOverlaps && !foundProvinceIndexes.Contains(provinceIndex))
                        {
                            foundProvinceIndexes.Add(neighborIndex);
                        }
                    }
                }
            }
           
            return foundProvinceIndexes;
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
            List<int> provinceIndexes = GetProvicesInCell(cellIndex);
            List<int> countryIndexes = GetCountriesFromProvinces(provinceIndexes);

            return countryIndexes;
        }

        /// <summary> 
        /// Get all landmarks located in a given cell
        /// Inputs:
        ///     cellIndex: Index of the cell in question
        /// Outputs:
        ///     landmarks: The landmarks in the cell in question
        /// </summary>
        public List<Landmark> GetLandmarksInCell(int cellIndex)
        {
            List<Landmark> landmarks = new List<Landmark>();
            foreach (MappableObject mappableObject in worldGlobeMap.cells[cellIndex].occupants)
            {
                if(mappableObject is Landmark)
                {
                    Landmark castedLandmark = mappableObject as Landmark;
                    landmarks.Add(castedLandmark);
                }
            }
            return landmarks; 
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
                province = worldGlobeMap.provinces[provinceIndex];
                countryIndex = province.countryIndex;
                if (!countryIndexes.Contains(countryIndex))
                {
                    countryIndexes.Add(countryIndex);
                }
            }
            return countryIndexes;
        }
        */


        //CAMERA CONTROLS


        /// <summary>
        ///  Orient the camera on a given location
        /// </summary>
        /// <param name="vectorLocation"></param> The location to orient on>
        /// <returns></returns> 
        /*
        public void OrientOnLocation(Vector3 vectorLocation)
        {
            worldGlobeMap.FlyToLocation(vectorLocation, 1.5F, 0.05F, 0.01F, 0);
            worldGlobeMap.pitch = 0;
            worldGlobeMap.yaw = 0;
        }
        */

        //GLOBE INITIALIZATION

        /// <summary>
        ///  Instantiate provinces and mappable objects based on settings file
        ///  NOTE: Settings file is not currently used and the settings have been hard coded
        /// </summary>
        /*
        void ApplyGlobeSettings()
        {
            Debug.Log("Applying Globe Settings");
            //if (File.Exists(Application.dataPath + "/student.txt"))
            //{
            //Load Settings
           
            bool[] provinceSettings = new bool[NUMBER_OF_PROVINCE_ATTRIBUTES];
            provinceSettings[POLITICAL_PROVINCE] = true;
            provinceSettings[TERRAIN] = false;
            provinceSettings[CLIMATE] = false;

            foreach (Country country in worldGlobeMap.countries)
                {
                    if (country.continent == "North America")
                    {
                        int countryNameIndex = worldGlobeMap.GetCountryIndex(country.name);
                        #region Merge Provinces
                            if (countryNameIndex >= 0)
                            {
                                //Get all provinces for country and loop through them
                                Province[] provinces = worldGlobeMap.countries[countryNameIndex].provinces;
                                int index = 0;
                                Province province;
                                string[] provinceAttributes = new string[NUMBER_OF_PROVINCE_ATTRIBUTES];
                                while (index < provinces.Length)
                                {
                                    //Get province attributes
                                    province = provinces[index];
                                    provinceAttributes[POLITICAL_PROVINCE] = province.attrib["PoliticalProvince"];
                                    if (provinceAttributes[POLITICAL_PROVINCE] == null) provinceAttributes[POLITICAL_PROVINCE] = "";
                                    provinceAttributes[TERRAIN] = province.attrib["Terrain"];
                                    if (provinceAttributes[TERRAIN] == null) provinceAttributes[TERRAIN] = "";
                                    provinceAttributes[CLIMATE] = province.attrib["Climate"];
                                    if (provinceAttributes[CLIMATE] == null) provinceAttributes[CLIMATE] = "";
                                    //Get all neighbors for province and loop through them
                                    int provinceIndex = worldGlobeMap.GetProvinceIndex(countryNameIndex, province.name);
                                    List<Province> neighbors = worldGlobeMap.ProvinceNeighboursOfMainRegion(provinceIndex);
                                    foreach (Province neighbor in neighbors)
                                    {
                                        //Check that neighbor is in same country
                                        if (neighbor.countryIndex == countryNameIndex)
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
                                                worldGlobeMap.ProvinceTransferProvinceRegion(provinceIndex, neighbor.mainRegion, true);
                                                List<Province> provinceList = provinces.ToList();
                                                provinceList.Remove(neighbor);
                                                provinces = provinceList.ToArray();
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
                                    index++;
                                }
                            }
                            worldGlobeMap.drawAllProvinces = false;
                            #endregion
                        #region Intantiate Player and Landmarks
                            //worldGlobeMap.ReloadMountPointsData();
                            List<MountPoint> countryMountPoints = new List<MountPoint>();
                            int mountPointCount = worldGlobeMap.GetMountPoints(countryNameIndex, countryMountPoints);

                            foreach (MountPoint mountPoint in countryMountPoints)
                            {
                                if (mountPoint.type == START_POINT && mountPoint.provinceIndex == worldGlobeMap.GetProvinceIndex(startingCountry, startingProvince))
                                {
                                    GameObject playerObject = Instantiate(playerPrefab);
                                    player = playerObject.GetComponent(typeof(PlayerCharacter)) as PlayerCharacter;
                                    int startingCellIndex = worldGlobeMap.GetCellIndex(mountPoint.localPosition);
                                    player.cellLocation = startingCellIndex;
                                    player.latlon = worldGlobeMap.cells[startingCellIndex].latlon;
                                    Vector3 startingLocation = worldGlobeMap.cells[startingCellIndex].sphereCenter;
                                    player.vectorLocation = startingLocation;
                                    float playerSize = player.GetSize();
                                    worldGlobeMap.AddMarker(playerObject, startingLocation, playerSize, false, 0.0f, true, true);
                                    string playerID = player.GetInstanceID().ToString();
                                    worldGlobeMap.cells[startingCellIndex].occupants.Add(player);
                                    mappedObjects.Add(playerID, player);
                                }
                                if (mountPoint.type == CULTURAL_POINT ) //&& loadedMapSettings.culturalLandmarks)
                                {
                                    string mountPointName = mountPoint.name;
                                    string tempName = mountPointName.Replace("The", "");
                                    tempName = tempName.Replace(" ", "");
                                    var model = Resources.Load<GameObject>("Prefabs/Landmarks/" + tempName);
                                    var modelClone = Instantiate(model);
                                    Landmark landmarkComponent = modelClone.GetComponent(typeof(Landmark)) as Landmark;
                                    landmarkComponent.mountPoint = mountPoint;
                                    landmarkComponent.objectName = mountPointName;
                                    landmarkComponent.cellIndex = worldGlobeMap.GetCellIndex(mountPoint.localPosition);
                                    landmarkComponent.cell = worldGlobeMap.cells[landmarkComponent.cellIndex];
                                    landmarkComponent.cell.canCross = false;
                                    worldGlobeMap.AddMarker(modelClone, mountPoint.localPosition, 0.001f, false, -5.0f, true, true);
                                    string landmarkID = landmarkComponent.GetInstanceID().ToString();
                                    worldGlobeMap.cells[landmarkComponent.cellIndex].occupants.Add(landmarkComponent);
                                    mappedObjects.Add(landmarkID, landmarkComponent);
                                    culturalLandmarks.Add(landmarkID, landmarkComponent);
                                    CulturalLandmarksByName.Add(landmarkComponent.objectName, landmarkComponent);
                                }
                            }

                            #endregion
                    }

                }
            //}
        }
        */

        /*
        /// <summary>
        ///  Instantiate all tourist regions and set initial region
        /// </summary>
        void InitTouristRegions()
        {
            #region Create North East Region
            TouristRegion northAmericaNorthEast = new TouristRegion();
            touristRegions.Add(northAmericaNorthEast);
            northAmericaNorthEast.regionName = "North America: North East";
            //Provinces
            northAmericaNorthEast.provinces.Add(494); //Newfoundland and Labrador
            northAmericaNorthEast.provinces.Add(491); //Québec
            northAmericaNorthEast.provinces.Add(490); //Ontario
            northAmericaNorthEast.provinces.Add(493); //Nova Scotia
            northAmericaNorthEast.provinces.Add(492); //New Brunswick
            //Prince Edward Island (495) Not being included due to small size
            northAmericaNorthEast.provinces.Add(3917); //Maine
            northAmericaNorthEast.provinces.Add(3894); //New Hampshire
            northAmericaNorthEast.provinces.Add(3896); //Vermont
            northAmericaNorthEast.provinces.Add(3869); //Massachusetts
            northAmericaNorthEast.provinces.Add(3895); //Rhode Island
            northAmericaNorthEast.provinces.Add(3893); //Connecticut
            northAmericaNorthEast.provinces.Add(3915); //New York
            //Landmarks
            northAmericaNorthEast.landmarks.Add("The Statue Of Liberty");
            northAmericaNorthEast.landmarks.Add("The CN Tower");
            northAmericaNorthEast.landmarks.Add("Parliament Hill");
            northAmericaNorthEast.landmarks.Add("The Fairmont Le Château Frontenac");
            #endregion

            #region Create US Mid West Regions
            TouristRegion northAmericaUSMidWestMidAtlantic = new TouristRegion();
            touristRegions.Add(northAmericaUSMidWestMidAtlantic);
            northAmericaUSMidWestMidAtlantic.regionName = "North America: US Mid West & Mid Atlantic";
            //Provinces
            northAmericaUSMidWestMidAtlantic.provinces.Add(490); //Ontario
            northAmericaUSMidWestMidAtlantic.provinces.Add(3915); //New York
            northAmericaUSMidWestMidAtlantic.provinces.Add(3914); //New Jersey
            northAmericaUSMidWestMidAtlantic.provinces.Add(3916); //Pennsylvania
            northAmericaUSMidWestMidAtlantic.provinces.Add(3911); //Delaware
            northAmericaUSMidWestMidAtlantic.provinces.Add(3913); //Maryland
            northAmericaUSMidWestMidAtlantic.provinces.Add(3906); //Ohio
            northAmericaUSMidWestMidAtlantic.provinces.Add(3910); //West Virginia
            northAmericaUSMidWestMidAtlantic.provinces.Add(3908); //Virginia
            northAmericaUSMidWestMidAtlantic.provinces.Add(3904); //Kentucky
            northAmericaUSMidWestMidAtlantic.provinces.Add(3918); //Michigan
            northAmericaUSMidWestMidAtlantic.provinces.Add(3903); //Indiana
            northAmericaUSMidWestMidAtlantic.provinces.Add(3902); //Illinois
            northAmericaUSMidWestMidAtlantic.provinces.Add(3909); //Wisconsin
            northAmericaUSMidWestMidAtlantic.provinces.Add(3870); //Minnesota
            northAmericaUSMidWestMidAtlantic.provinces.Add(3885); //Iowa
            northAmericaUSMidWestMidAtlantic.provinces.Add(3887); //Missouri
            //Landmarks
            northAmericaUSMidWestMidAtlantic.landmarks.Add("The Statue Of Liberty");
            northAmericaUSMidWestMidAtlantic.landmarks.Add("The Washington Monument");
            northAmericaUSMidWestMidAtlantic.landmarks.Add("The Lincoln Memorial");
            northAmericaUSMidWestMidAtlantic.landmarks.Add("The Gateway Arch");
            northAmericaUSMidWestMidAtlantic.landmarks.Add("The CN Tower");
            northAmericaUSMidWestMidAtlantic.landmarks.Add("Parliament Hill");
            #endregion

            #region Create US South East
            TouristRegion northAmericaUSSouthEast = new TouristRegion();
            touristRegions.Add(northAmericaUSSouthEast);
            northAmericaUSSouthEast.regionName = "North America: US South East";
            //Provinces
            northAmericaUSSouthEast.provinces.Add(3911); //Delaware
            northAmericaUSSouthEast.provinces.Add(3913); //Maryland
            northAmericaUSSouthEast.provinces.Add(3910); //West Virginia
            northAmericaUSSouthEast.provinces.Add(3908); //Virginia
            northAmericaUSSouthEast.provinces.Add(3904); //Kentucky
            northAmericaUSSouthEast.provinces.Add(3905); //North Carolina
            northAmericaUSSouthEast.provinces.Add(3907); //Tennessee
            northAmericaUSSouthEast.provinces.Add(3901); //South Carolina
            northAmericaUSSouthEast.provinces.Add(3899); //Georgia
            northAmericaUSSouthEast.provinces.Add(3897); //Alabama
            northAmericaUSSouthEast.provinces.Add(3898); //Florida
            northAmericaUSSouthEast.provinces.Add(3900); //Mississippi
            northAmericaUSSouthEast.provinces.Add(3891); //Louisiana
            northAmericaUSSouthEast.provinces.Add(3884); //Arkansas
            northAmericaUSSouthEast.provinces.Add(3892); //Texas
            northAmericaUSSouthEast.provinces.Add(3889); //Oklahoma
            //Landmarks
            northAmericaUSSouthEast.landmarks.Add("The Washington Monument");
            northAmericaUSMidWestMidAtlantic.landmarks.Add("The Lincoln Memorial");
            #endregion

            #region Create US South West
            TouristRegion northAmericaSouthWest = new TouristRegion();
            touristRegions.Add(northAmericaSouthWest);
            northAmericaSouthWest.regionName = "North America: South West";
            //Provinces
            northAmericaSouthWest.provinces.Add(3892); //Texas
            northAmericaSouthWest.provinces.Add(3889); //Oklahoma
            northAmericaSouthWest.provinces.Add(3886); //Kansas
            northAmericaSouthWest.provinces.Add(3878); //Colorado
            northAmericaSouthWest.provinces.Add(3880); //New Mexico
            northAmericaSouthWest.provinces.Add(3876); //Arizona
            northAmericaSouthWest.provinces.Add(3882); //Utah
            northAmericaSouthWest.provinces.Add(3879); //Nevada
            northAmericaSouthWest.provinces.Add(3877); //California
            //Landmarks
            northAmericaSouthWest.landmarks.Add("The Hoover Dam");
            northAmericaSouthWest.landmarks.Add("The Golden Gate Bridge");
            northAmericaSouthWest.landmarks.Add("Mesa Verde National Park");
            //Countries
            northAmericaSouthWest.countries.Add(165); //Mexico
            #endregion

            #region Create North West US
            TouristRegion northAmericaNorthWestUS = new TouristRegion();
            touristRegions.Add(northAmericaNorthWestUS);
            northAmericaNorthWestUS.regionName = "North America: North West US";
            //Provinces
            northAmericaNorthWestUS.provinces.Add(3881); //Oregon
            northAmericaNorthWestUS.provinces.Add(3875); //Washington
            northAmericaNorthWestUS.provinces.Add(486); //British Columbia
            northAmericaNorthWestUS.provinces.Add(3874); //Idaho
            northAmericaNorthWestUS.provinces.Add(3883); //Wyoming
            northAmericaNorthWestUS.provinces.Add(3871); //Montana
            northAmericaNorthWestUS.provinces.Add(485); //Alberta
            northAmericaNorthWestUS.provinces.Add(484); //Saskatchewan
            northAmericaNorthWestUS.provinces.Add(3872); //North Dakota
            northAmericaNorthWestUS.provinces.Add(3890); //South Dakota
            northAmericaNorthWestUS.provinces.Add(3888); //Nebraska
            //Landmarks
            northAmericaNorthWestUS.landmarks.Add("Mount Rushmore");
            northAmericaNorthWestUS.landmarks.Add("The Space Needle");
            #endregion

            #region Create North Western North America
            TouristRegion northAmericaNorthWest = new TouristRegion();
            touristRegions.Add(northAmericaNorthWest);
            northAmericaNorthWest.regionName = "North America: North West";
            //Provinces
            northAmericaNorthWest.provinces.Add(3875); //Washington
            northAmericaNorthWest.provinces.Add(486); //British Columbia
            northAmericaNorthWest.provinces.Add(485); //Alberta
            northAmericaNorthWest.provinces.Add(484); //Saskatchewan
            northAmericaNorthWest.provinces.Add(487); //Nunavut
            northAmericaNorthWest.provinces.Add(484); //Saskatchewan
            northAmericaNorthWest.provinces.Add(483); //Manitoba
            northAmericaNorthWest.provinces.Add(488); //Northwest Territories
            northAmericaNorthWest.provinces.Add(489); //Yukon
            northAmericaNorthWest.provinces.Add(3919); //Alaska
            //Landmarks
            northAmericaNorthWestUS.landmarks.Add("The Space Needle");
            #endregion

            #region Create Central America
            TouristRegion northAmericaCentralAmerica = new TouristRegion();
            touristRegions.Add(northAmericaCentralAmerica);
            northAmericaCentralAmerica.regionName = "North America: Central America";
            //Provinces
            northAmericaCentralAmerica.provinces.Add(3892); //Texas
            //Landmarks
            northAmericaCentralAmerica.landmarks.Add("El Castillo of Chichen Itza");
            northAmericaCentralAmerica.landmarks.Add("The Ruins of Tikal");
            northAmericaCentralAmerica.landmarks.Add("The Angel of Mexican Independence");
            northAmericaCentralAmerica.landmarks.Add("The Panama Canal");
            //Contries
            northAmericaCentralAmerica.countries.Add(165);//Mexico
            northAmericaCentralAmerica.countries.Add(21); //Belize
            northAmericaCentralAmerica.countries.Add(58); //Guatemala
            northAmericaCentralAmerica.countries.Add(23); //El Salvador
            northAmericaCentralAmerica.countries.Add(69); //Honduras
            northAmericaCentralAmerica.countries.Add(70); //Nicaragua
            northAmericaCentralAmerica.countries.Add(48); //Costa Rica
            northAmericaCentralAmerica.countries.Add(54); //Panama
            #endregion

            #region Set Tourist Region Neighbours
            //North East Region 
            northAmericaNorthEast.neighbouringRegions.Add(northAmericaUSMidWestMidAtlantic);
            northAmericaNorthEast.neighbouringRegions.Add(northAmericaNorthWestUS);
            northAmericaNorthEast.neighbouringRegions.Add(northAmericaNorthWest);

            //US Mid West Region 
            northAmericaUSMidWestMidAtlantic.neighbouringRegions.Add(northAmericaNorthEast);
            northAmericaUSMidWestMidAtlantic.neighbouringRegions.Add(northAmericaUSSouthEast);
            northAmericaUSMidWestMidAtlantic.neighbouringRegions.Add(northAmericaSouthWest);
            northAmericaUSMidWestMidAtlantic.neighbouringRegions.Add(northAmericaNorthWestUS);
            northAmericaUSMidWestMidAtlantic.neighbouringRegions.Add(northAmericaNorthWest);

            //US South East Region 
            northAmericaUSSouthEast.neighbouringRegions.Add(northAmericaUSMidWestMidAtlantic);
            northAmericaUSSouthEast.neighbouringRegions.Add(northAmericaSouthWest);
            northAmericaUSSouthEast.neighbouringRegions.Add(northAmericaCentralAmerica);

            //US South West Region 
            northAmericaSouthWest.neighbouringRegions.Add(northAmericaUSSouthEast);
            northAmericaSouthWest.neighbouringRegions.Add(northAmericaUSMidWestMidAtlantic);
            northAmericaSouthWest.neighbouringRegions.Add(northAmericaNorthWestUS);
            northAmericaSouthWest.neighbouringRegions.Add(northAmericaCentralAmerica);

            //US North West Region 
            northAmericaNorthWestUS.neighbouringRegions.Add(northAmericaSouthWest);
            northAmericaNorthWestUS.neighbouringRegions.Add(northAmericaUSMidWestMidAtlantic);
            northAmericaNorthWestUS.neighbouringRegions.Add(northAmericaNorthEast);
            northAmericaNorthWestUS.neighbouringRegions.Add(northAmericaNorthWest);

            //North West Region
            northAmericaNorthWest.neighbouringRegions.Add(northAmericaNorthWestUS);
            northAmericaNorthWest.neighbouringRegions.Add(northAmericaNorthEast);
            northAmericaNorthWest.neighbouringRegions.Add(northAmericaUSMidWestMidAtlantic);

            //Central America Region
            northAmericaCentralAmerica.neighbouringRegions.Add(northAmericaSouthWest);
            northAmericaCentralAmerica.neighbouringRegions.Add(northAmericaUSSouthEast);

            #endregion

            CurrentRegion = northAmericaUSSouthEast;
        }
        //GAME UPDATES
        */

        /*
        /// <summary> 
        /// Called when a tourist needs to be generated and added to the palyer's inventory
        /// </summary>
        public void GenerateTourist()
        {
            //Intantiate tourist
            InventoryTourist tourist = Instantiate(touristPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            tourist.transform.parent = gameObject.transform.Find("Canvas/InventoryPanel");
            //Give tourist its image
            tourist.inventoryIcon = Resources.Load<Sprite>(touristImageFiles[touristImageIndex]);
            touristImageIndex++;
            if (touristImageIndex >= NUMBER_OF_TOURIST_IMAGES)
                touristImageIndex = 0;
            //Add tourist to player's inventory
            player.AddItem(tourist, 0);
            //Check if a region switch is needed
            touristsInCurrentRegion++;
            int rand = Random.Range(MIN_TIME_IN_REGION, MAX_TIME_IN_REGION);
            if(touristsInCurrentRegion >= rand)
            {
                //Switch regions
                int newRegionNeighbourIndex = Random.Range(0, CurrentRegion.neighbouringRegions.Count - 1);
                CurrentRegion = CurrentRegion.neighbouringRegions[newRegionNeighbourIndex];
                touristsInCurrentRegion = 0;
            }
        }
        */
        /// <summary>
        ///  Update the games current score
        /// </summary>
        /// <param name="scoreModification"></param> The amount the score should be changed by>
        /// <returns></returns> 
        public void UpdateScore(int scoreModification)
        {
            score = score + scoreModification;
            UIManager.ScoreUI.UpdateDisplayedScore(score);
            //scoreInfo.text = "Score: " + score + System.Environment.NewLine + "Turns Left: " + turnsRemaining;
        }

        /// <summary> 
        /// Update the turns remaining until the game ends and check if game has ended
        /// </summary>
        /// <param name="turnModification"></param> The number of turns the reminaing turns
        /// are updated by
        public void UpdateRemainingTurns(int turnModification)
        {
            turnsRemaining = turnsRemaining + turnModification;
            if (turnsRemaining <= 0)
            {
                turnsRemaining = 0;
                GameOver();
            }
            UIManager.TurnsUI.UpdateDisplayedRemainingTurns(turnsRemaining);
            //scoreInfo.text = "Score: " + score + System.Environment.NewLine + "Turns Left: " + turnsRemaining;
        }

        //CHANGE GAME STATE

        /// <summary> 
        /// Called when game ends
        /// </summary>
        public void GameOver()
        {
            //Setup U.I. panels and flags
            UIManager.GameOver();

            //gameOverPanel.SetActive(true);
            //dialogPanel.SetActive(false);
            GamePaused = true;
            //gameOverMessage.text = "Time's Up!" + System.Environment.NewLine + "Your Score Was: " + score;
            //popUpPanel.SetActive(false);
        }

        /// <summary> 
        /// Resets the game by reloading the scene
        /// </summary>
        public void GameReset()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        /// <summary> 
        /// Exit the application
        /// </summary>
        public void ExitGame()
        {
            Application.Quit();
        }

        /*
        /// <summary> 
        /// Open in game menu
        /// </summary>
        public void OpenGameMenu()
        {
            gameMenuPanel.SetActive(true);
            GamePaused = true;
        }
        */

        public void ResumeGame()
        {
            GamePaused = false;
            GameMenuOpen = false;
        }
        /*
        /// <summary> 
        /// Display a popup notification with a given message
        /// </summary>
        /// <param name="displayText"></param> The text to be displayed on the pop up
        /// are updated by
        public void DisplayPopUp(string displayText)
        {
            popUpPanel.SetActive(true);
            popUpMessage.text = displayText;
        }

        /// <summary> 
        /// Close active pop up
        /// </summary>
        public void ClosePopUp()
        {
            popUpPanel.SetActive(false);
        }
        */
        /// <summary> 
        /// Display an error popup with a given error message
        /// </summary>
        /// <param name="errorText"></param> The error text to be displayed on the pop up
        /// are updated by
        public void DisplayError(string errorText, string stackTraceText)
        {
            errorPanel.SetActive(true);
            errorMessage.text = "Error: " + errorText;
            stackTraceInputField.text = stackTraceText;
        }

        /*
        public void ErrorButton()
        {
            errorPanel.SetActive(false);
            ClosingGUIPanel = true;
            switch (errorState)
            {
                case (ErrorState.close_window):
                    break;
                case (ErrorState.restart_scene):
                    GameReset();
                    break;
                case (ErrorState.close_application):
                    ExitGame();
                    break;
                default:
                    break;
            }   
        }
        */

        public void DropOff(bool success)
        {
            /*  This will be for drop off sound effects
            if (success)
                dropOffSuccess.Play();
            else
                dropOffFailure.Play();
            */    
        }
        
    }
}
