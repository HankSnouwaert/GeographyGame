﻿using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SpeedTutorMainMenuSystem;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace WPM
{
    public class GameManager : MonoBehaviour
    {
        [Header("Player Components")]
        public WorldMapGlobe worldGlobeMap;
        public GameObject playerPrefab;
        public EventSystem eventSystem;
        public GameObject dialogPanel;
        public GameObject hexInfoPanel;
        private Text hexInfo;
        private InventoryGUI inventoryGUI;
        private InventoryTourist touristPrefab;
        private PlayerCharacter player;
        public int globalTurnCounter = 0;
        private int touristCounter = 0;
        public bool cursorOverUI = false;
        private int touristSpawnRate = 10;
        PlayerCharacter playerCharacter;
        public SelectableObject selectedObject = null;
        public Dictionary<string, MappableObject> mappedObjects = new Dictionary<string, MappableObject>();
        public List<int> recentProvinceDestinations = new List<int>();
        public List<string> recentLandmarkDestinations = new List<string>();
        public List<int> recentCountryDestinations = new List<int>();
        public int trackingTime = 10;
        WorldMapGlobe map;
        public Dictionary<string, Landmark> culturalLandmarks = new Dictionary<string, Landmark>();
        string startingCountry = "United States of America";
        string startingProvince = "North Carolina";
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

            // Setup grid events
            map.OnCellEnter += HandleOnCellEnter;
            map.OnCellExit += HandleOnCellExit;
            map.OnCellClick += HandleOnCellClick;

            //Get Prefabs
            touristPrefab = Resources.Load<InventoryTourist>("Prefabs/Inventory/InventoryTourist");

            //Get scene objects
            inventoryGUI = FindObjectOfType<InventoryGUI>();
            player = FindObjectOfType<PlayerCharacter>();
            dialogPanel = GameObject.Find("/Canvas/DialogPanel");
            dialogPanel.SetActive(false);
            hexInfoPanel = GameObject.Find("/Canvas/HexInfoPanel");
            hexInfoPanel.SetActive(false);
            Transform textObject = hexInfoPanel.transform.GetChild(0);
            hexInfo = textObject.gameObject.GetComponent(typeof(Text)) as Text;

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
            if (!cursorOverUI)
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
            if (!cursorOverUI && worldGlobeMap.lastHighlightedCellIndex >= 0)
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
                        provincesInHex = GetProvicesInCell(cellIndex);
                        foreach (int provinceIndex in provincesInHex)
                        {
                            if (!foundProvinces.Contains(provinceIndex))
                            {
                                foundProvinces.Add(provinceIndex);
                                provinces[distance].Add(provinceIndex);
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

        List<int> GetProvicesInCell(int cellIndex)
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
                            List<MountPoint> USmountPoints = new List<MountPoint>();
                            int mountPointCount = worldGlobeMap.GetMountPoints(countryNameIndex, USmountPoints);

                            foreach (MountPoint mountPoint in USmountPoints)
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
                                }
                            }

                            #endregion
                    }

                }
            }
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
        }

    }
}
