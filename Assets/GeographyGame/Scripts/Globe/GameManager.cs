using UnityEngine;
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
        [Header("Player Components")]
        public WorldMapGlobe worldGlobeMap;
        public GameObject playerPrefab;
        public EventSystem eventSystem;
        public GameObject inventoryPanel;
        public GameObject dialogPanel;
        public GameObject hexInfoPanel;
        public GameObject scorePanel;
        public GameObject gameOverPanel;
        private Text hexInfo;
        private Text scoreInfo;
        private Text gameOverMessage;
        private InventoryGUI inventoryGUI;
        private InventoryTourist touristPrefab;
        private PlayerCharacter player;
        public int globalTurnCounter = 0;
        private int touristCounter = 0;
        private int score = 0;
        public bool cursorOverUI = false;
        private bool menuOpen = false;
        private int touristSpawnRate = 10;
        PlayerCharacter playerCharacter;
        public SelectableObject selectedObject = null;
        public Dictionary<string, MappableObject> mappedObjects = new Dictionary<string, MappableObject>();
        public List<int> recentProvinceDestinations = new List<int>();
        public List<string> recentLandmarkDestinations = new List<string>();
        public List<int> recentCountryDestinations = new List<int>();
        public int trackingTime = 10;
        private List<TouristRegion> touristRegions = new List<TouristRegion>();
        public TouristRegion currentRegion;
        public List<TouristRegion> regionsVisited = new List<TouristRegion>();
        public int touristsInCurrentRegion = -2;  //This number is the starting number of tourists * -1
        public const int MIN_TIME_IN_REGION = 5;
        public const int MAX_TIME_IN_REGION = 10;
        WorldMapGlobe map;
        public Dictionary<string, Landmark> culturalLandmarks = new Dictionary<string, Landmark>();
        public Dictionary<string, Landmark> culturalLandmarksByName = new Dictionary<string, Landmark>();
        string startingCountry = "United States of America";
        string startingProvince = "North Carolina";
        private int turnsRemaining = 1;
        public const int NUMBER_OF_PROVINCE_ATTRIBUTES = 3;
        public const int POLITICAL_PROVINCE = 0;
        public const int TERRAIN = 1;
        public const int CLIMATE = 2;
        public const int START_POINT = 0;
        public const int NATURAL_POINT = 1;
        public const int CULTURAL_POINT = 2;
        public const string CELL_PLAYER = "Player";

        private string[] touristImageFiles;
        private int touristImageIndex = 0;
        private const int NUMBER_OF_TOURIST_IMAGES = 3;

        private bool debug;

        GUIStyle labelStyle, labelStyleShadow, buttonStyle, sliderStyle, sliderThumbStyle;

        static GameManager _instance;

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

        void Start()
        {
            Debug.Log("Globe Loaded");
            ApplyGlobeSettings();
            // UI Setup - non-important, only for this demo
            labelStyle = new GUIStyle();
            labelStyle.alignment = TextAnchor.MiddleLeft;
            labelStyle.normal.textColor = Color.white;
            labelStyleShadow = new GUIStyle(labelStyle);
            labelStyleShadow.normal.textColor = Color.black;
            buttonStyle = new GUIStyle(labelStyle);
            buttonStyle.alignment = TextAnchor.MiddleLeft;
            buttonStyle.normal.background = Texture2D.whiteTexture;
            buttonStyle.normal.textColor = Color.white;
            sliderStyle = new GUIStyle();
            sliderStyle.normal.background = Texture2D.whiteTexture;
            sliderStyle.fixedHeight = 4.0f;
            sliderThumbStyle = new GUIStyle();
            sliderThumbStyle.normal.background = Resources.Load<Texture2D>("thumb");
            sliderThumbStyle.overflow = new RectOffset(0, 0, 8, 0);
            sliderThumbStyle.fixedWidth = 20.0f;
            sliderThumbStyle.fixedHeight = 12.0f;

            // setup GUI resizer - only for the demo
            GUIResizer.Init(800, 500);

            // Get map instance to Globe API methods
            map = WorldMapGlobe.instance;

            // Initialize Tourist Regions
            InitTouristRegions();

            // Setup grid events
            map.OnCellEnter += HandleOnCellEnter;
            map.OnCellExit += HandleOnCellExit;
            map.OnCellClick += HandleOnCellClick;

            //Get Prefabs
            touristPrefab = Resources.Load<InventoryTourist>("Prefabs/Inventory/InventoryTourist");

            //Get scene objects
            inventoryGUI = FindObjectOfType<InventoryGUI>();
            player = FindObjectOfType<PlayerCharacter>();
            //Dialog Panel
            dialogPanel = GameObject.Find("/Canvas/DialogPanel");
            dialogPanel.SetActive(false);
            //Hex Info Panel
            hexInfoPanel = GameObject.Find("/Canvas/HexInfoPanel");
            hexInfoPanel.SetActive(false);
            Transform textObject = hexInfoPanel.transform.GetChild(0);
            hexInfo = textObject.gameObject.GetComponent(typeof(Text)) as Text;
            hexInfoPanel.SetActive(false);
            //Score Panel
            scorePanel = GameObject.Find("/Canvas/ScorePanel");
            textObject = scorePanel.transform.GetChild(0);
            scoreInfo = textObject.gameObject.GetComponent(typeof(Text)) as Text;
            scoreInfo.text = "Score: " + score + System.Environment.NewLine + "Remaining Movement: " + turnsRemaining;

            //GameOver Panel
            gameOverPanel.SetActive(false);
            textObject = gameOverPanel.transform.GetChild(0);
            gameOverMessage = textObject.gameObject.GetComponent(typeof(Text)) as Text;

            //Set Tourist Images
            touristImageFiles = new string[3];
            touristImageFiles[0] = "Images/Tourist1";
            touristImageFiles[1] = "Images/Tourist2";
            touristImageFiles[2] = "Images/Tourist3";
        }

        public void NextTurn(int turns)
        {
            globalTurnCounter = globalTurnCounter + turns;
            touristCounter = touristCounter + turns;
            UpdateTurns(turns*-1);
            SelectableObject []
            selectableObjects = UnityEngine.Object.FindObjectsOfType<SelectableObject>();
                foreach(SelectableObject selectableObject in selectableObjects)
                {
                    selectableObject.EndOfTurn(turns);
                }
            if(touristCounter >= touristSpawnRate)
            {
                touristCounter = 0;
                GenerateTourist();
            }
        }

        void HandleOnCellClick(int cellIndex)
        {
            if (!cursorOverUI  && !menuOpen)
            {
                Debug.Log("Clicked cell: " + cellIndex);
                if (selectedObject == null)
                {
                    if (worldGlobeMap.cells[cellIndex].tag != null)
                    {
                        //A new mappable object is being selected
                        selectedObject = mappedObjects[worldGlobeMap.cells[cellIndex].tag];
                        selectedObject.Selected();
                    }
                    else
                    {
                        //Nothing is selected, and an empty hex is being clicked
                    }
                }
                else
                {
                    //A hex is being clicked while an object is selected
                    selectedObject.OnCellClick(cellIndex);
                }
            }
        }

        void HandleOnCellEnter(int index)
        {
            if (!cursorOverUI && !menuOpen && worldGlobeMap.lastHighlightedCellIndex >= 0)
            {
                Province province = worldGlobeMap.provinceHighlighted;
                Country country = worldGlobeMap.countryHighlighted;
                string displayText;
                if (province != null)
                {
                    string nameType;
                    if (country.name == "United States of America")
                        nameType = "State: ";
                    else
                        nameType = "Province: ";
                    string politicalProvince = province.attrib["PoliticalProvince"];
                    string climate = province.attrib["ClimateGroup"];
                    displayText = "Country: " + country.name + System.Environment.NewLine + nameType + politicalProvince + System.Environment.NewLine + "Hex Index: " + index.ToString(); // + System.Environment.NewLine + "Climate: " + climate;
                    if (worldGlobeMap.cells[index].tag != null)
                    {
                        if (worldGlobeMap.cells[index].index != player.cellLocation)
                        {
                            string landmarkName = mappedObjects[worldGlobeMap.cells[index].tag].objectName;
                            displayText = displayText + System.Environment.NewLine + "Landmark: " + landmarkName;
                        }
                    }

                    hexInfoPanel.SetActive(true);
                    hexInfo.text = displayText;
                }

                if (selectedObject != null)
                {
                    selectedObject.OnCellEnter(index);
                }
            } 
        }

        void HandleOnCellExit(int index)
        {
            Province province = worldGlobeMap.provinceHighlighted;
            if (province == null || cursorOverUI)
                hexInfoPanel.SetActive(false);
        }

        public void DeselectObject()
        {
            selectedObject = null;
        }

        /// <summary> 
        /// Get all cells within a certain range (measured in cells) of a target cell
        /// Returns an array of lists, with List0 containing all cells within range
        /// and ListX containing the cells X number of cells away from the target cell
        /// </summary>
        public List<int>[] GetCellsInRange(int startCell, int range = 0)
        {
            //NOTE: this function uses the canCross flag of cells to track which cells
            //it has checked and assumes all cells will start with it false

            if (range < 0 || startCell < 0 || map.cells.Count() < startCell)
            {
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
            map.cells[startCell].flag = true;

            if(range > 0)
            {
                //Add the neighbors of the start cell to List1
                //And add them to List0
                distance++;
                cells[distance] = new List<int>();
                foreach (Cell neighbour in map.GetCellNeighbours(startCell))
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
                    foreach (Cell neighbour in map.GetCellNeighbours(cell))
                    {
                        if (neighbour.index == 36285)
                            debug = true;

                        if (!neighbour.flag)
                        {
                            if (neighbour.index == 36285)
                                debug = true;

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
                map.cells[cell].flag = false;
            }
            return cells;
        }

        public List<int>[] GetProvincesInRange(int startCell, List<int>[] cellRange)
        {
            //NOTE: this function uses the canCross flag of cells to track which cells
            //it has checked and assumes all cells will start with it false

            int range = cellRange.Length;

            if (range < 0 || startCell < 0 || map.cells.Count() < startCell)
            {
                Debug.LogWarning("Invalid input for GetProvincesInRange");
                return null;
            }

            int distance = 0;                                          //distance measures how many rings of hexes we've moved out
            List<int>[] provinces = new List<int>[range + 1];      //provinces is an array of lists with each list the provinces that can be reached at that distance.  
            List<int> foundProvinces = new List<int>();
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
                        if (map.FindPath(startCell, cellIndex) != null)
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

        public List<string>[] GetLandmarksInRange(int startCell, List<int>[] cellRange)
        {
            int range = cellRange.Length;

            if (range < 0 || startCell < 0 || map.cells.Count() < startCell)
            {
                Debug.LogWarning("Invalid input for GetCellsInRange");
                return null;
            }

            int distance = 0;
            List<string>[] landmarks = new List<string>[range + 1];
            landmarks[0] = new List<string>();
            string landmarkIndex;

            bool startHex = true;
            foreach (List<int> hexRing in cellRange)
            {
                if (startHex)
                {
                    //Get landmark at start hex
                    landmarkIndex = worldGlobeMap.cells[startCell].tag;
                    if (landmarkIndex != null && startCell != player.cellLocation)
                    {
                        landmarks[0].Add(landmarkIndex);
                    }
                    startHex = false;
                }
                else
                {
                    distance++;
                    landmarks[distance] = new List<string>();
                    foreach(int cellIndex in hexRing)
                    {
                        if (cellIndex == 36285)
                            debug = true;

                        landmarkIndex = worldGlobeMap.cells[cellIndex].tag;
                        if (landmarkIndex != null && cellIndex != player.cellLocation)
                        {
                            landmarks[distance].Add(landmarkIndex);
                        }
                    }
                }
            }
            return landmarks;

        }

        public List<int> GetProvicesInCell(int cellIndex)
        {
            List<int> provinces = new List<int>();
            int provinceIndex;
            int countryIndex;
            int neighborIndex;

            provinceIndex = worldGlobeMap.GetProvinceIndex(worldGlobeMap.cells[cellIndex].sphereCenter);
            //Check if hex is centered on a province
            if (provinceIndex == -1)
            {
                //Get closest province if hex is not centered on one
                provinceIndex = worldGlobeMap.GetProvinceNearPoint(worldGlobeMap.cells[cellIndex].sphereCenter);
            }
            //Check country of province
            //countryIndex = worldGlobeMap.provinces[provinceIndex].countryIndex;
            //if (map.countries[countryIndex].name == "United States of America" || map.countries[countryIndex].name == "Canada")
           // {
                //Add province to list
                provinces.Add(provinceIndex);
           // }
            //Check to see if neighbours of province overlap with cell
            List<Province> provinceNeighbours = worldGlobeMap.ProvinceNeighbours(provinceIndex);
            bool provinceOverlaps;
            foreach (Province neighbor in provinceNeighbours)
            {
                provinceOverlaps = false;
                countryIndex = neighbor.countryIndex;
                if (map.countries[countryIndex].name == "United States of America" || map.countries[countryIndex].name == "Canada")
                {
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
                    if (provinceOverlaps)
                    {
                        //Check if neighbour has already been found
                        neighborIndex = worldGlobeMap.GetProvinceIndex(countryIndex, neighbor.name);
                        provinces.Add(neighborIndex);
                    }
                }
            }
            return provinces;
        }

        Landmark GetLandmarkInCell(int cellIndex)
        {
            Landmark landmark = null;
            string objectInCell = worldGlobeMap.cells[cellIndex].tag;
            if (objectInCell != null && objectInCell != "")
            {
                landmark = culturalLandmarks[objectInCell];
            }

            return landmark; 
        }

        void ApplyGlobeSettings()
        {
            Debug.Log("Applying Globe Settings");
            if (File.Exists(Application.dataPath + "/student.txt"))
            {
                //Load Settings
                string savedMapSettings = File.ReadAllText(Application.dataPath + "/student.txt");
                SaveObject loadedMapSettings = JsonUtility.FromJson<SaveObject>(savedMapSettings);
                bool[] provinceSettings = new bool[NUMBER_OF_PROVINCE_ATTRIBUTES];
                provinceSettings[POLITICAL_PROVINCE] = loadedMapSettings.provinces;
                provinceSettings[TERRAIN] = loadedMapSettings.terrain;
                provinceSettings[CLIMATE] = loadedMapSettings.climate;
                //Add loop for all countries here
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
                                                if (!loadedMapSettings.provinces)
                                                    neighbor.attrib["PoliticalProvince"] = "";
                                                if (!loadedMapSettings.terrain)
                                                    neighbor.attrib["Terrain"] = "";
                                                if (!loadedMapSettings.climate)
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
                                    playerCharacter = playerObject.GetComponent(typeof(PlayerCharacter)) as PlayerCharacter;
                                    int startingCellIndex = worldGlobeMap.GetCellIndex(mountPoint.localPosition);
                                    playerCharacter.cellLocation = startingCellIndex;
                                    playerCharacter.latlon = worldGlobeMap.cells[startingCellIndex].latlon;
                                    Vector3 startingLocation = worldGlobeMap.cells[startingCellIndex].sphereCenter;
                                    playerCharacter.vectorLocation = startingLocation;
                                    worldGlobeMap.AddMarker(playerObject, startingLocation, playerCharacter.size, false, 0.0f, true, true);
                                    string playerID = playerCharacter.GetInstanceID().ToString();
                                    worldGlobeMap.cells[startingCellIndex].tag = playerID;
                                    mappedObjects.Add(playerID, playerCharacter);
                                }
                                if (mountPoint.type == CULTURAL_POINT && loadedMapSettings.culturalLandmarks)
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
                                    worldGlobeMap.AddMarker(modelClone, mountPoint.localPosition, 0.001f, false, 0.0f, true, true);
                                    string landmarkID = landmarkComponent.GetInstanceID().ToString();
                                    worldGlobeMap.cells[landmarkComponent.cellIndex].tag = landmarkID;
                                    mappedObjects.Add(landmarkID, landmarkComponent);
                                    culturalLandmarks.Add(landmarkID, landmarkComponent);
                                    culturalLandmarksByName.Add(landmarkComponent.objectName, landmarkComponent);
                                }
                            }

                            #endregion
                    }

                }
            }
        }

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

            currentRegion = northAmericaUSSouthEast;
        }

        public void GenerateTourist()
        {
            InventoryTourist tourist = Instantiate(touristPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            tourist.transform.parent = gameObject.transform.Find("Canvas/InventoryPanel");
            tourist.inventoryIcon = Resources.Load<Sprite>(touristImageFiles[touristImageIndex]);
            touristImageIndex++;
            if (touristImageIndex >= NUMBER_OF_TOURIST_IMAGES)
                touristImageIndex = 0;
            player.AddItem(tourist, 0);
            //Check if a region switch is needed
            touristsInCurrentRegion++;
            int rand = Random.Range(MIN_TIME_IN_REGION, MAX_TIME_IN_REGION);
            if(touristsInCurrentRegion >= rand)
            {
                int newRegionNeighbourIndex = Random.Range(0, currentRegion.neighbouringRegions.Count - 1);
                currentRegion = currentRegion.neighbouringRegions[newRegionNeighbourIndex];
                touristsInCurrentRegion = 0;
            }
        }

        public void UpdateScore(int scoreModification)
        {
            score = score + scoreModification;
            scoreInfo.text = "Score: " + score + System.Environment.NewLine + "Remaining Movement: " + turnsRemaining;
        }

        public void UpdateTurns(int turnModification)
        {
            turnsRemaining = turnsRemaining + turnModification;
            scoreInfo.text = "Score: " + score + System.Environment.NewLine + "Remaining Movement: " + turnsRemaining;
            if (turnsRemaining <= 0)
                GameOver();
        }

        public void GameOver()
        {
            gameOverPanel.SetActive(true);
            inventoryPanel.SetActive(false);
            dialogPanel.SetActive(false);
            cursorOverUI = true;
            menuOpen = true;
            gameOverMessage.text = "Time's Up!" + System.Environment.NewLine + "Your Score Was: " + score;
        }

        public void GameReset()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void ExitGame()
        {
            Application.Quit();
        }
    }
}
