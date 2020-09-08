﻿using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SpeedTutorMainMenuSystem;
using System.Linq;

namespace WPM
{
    public class GameManager : MonoBehaviour
    {
        [Header("Player Components")]
        public WorldMapGlobe worldGlobeMap;
        public GameObject playerPrefab;
        PlayerCharacter playerCharacter;
        public SelectableObject selectedObject = null;
        Dictionary<string, MappableObject> mappedObjects = new Dictionary<string, MappableObject>();
        WorldMapGlobe map;
        List<Landmark> culturalLandmarks = null;
        List<Landmark> naturalLandmarks = null;
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
        enum SELECTION_MODE
        {
            NONE = 0,
            CUSTOM_PATH = 1,
            CUSTOM_COST = 2
        }

        GUIStyle labelStyle, labelStyleShadow, buttonStyle, sliderStyle, sliderThumbStyle;
        SELECTION_MODE selectionMode = SELECTION_MODE.NONE;
        int selectionState = 0;
        // 0 = selecting first cell, 1 = selecting second cell
        int firstCell;
        // the cell index of the first selected cell when setting edge cost between two neighbour cells

        void Start()
        {
            Debug.Log("Globe Loaded");
            ApplyGlobeSettings();
            //PlayerCharacter playerCharacter = playerPrefab.GetComponent(typeof(PlayerCharacter)) as PlayerCharacter;
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
            map.OnCellExit += (int cellIndex) => Debug.Log("Exited cell: " + cellIndex);
            map.OnCellClick += HandleOnCellClick;
        }

        void OnGUI()
        {
            if (worldGlobeMap.lastHighlightedCellIndex >= 0)
            {
                Province province = worldGlobeMap.provinceHighlighted;
                Country country = worldGlobeMap.countryHighlighted;
                if (province != null)
                {
                    string name = province.name;
                    string politicalProvince = province.attrib["PoliticalProvince"];
                    string climate = province.attrib["Climate"];
                    GUI.Label(new Rect(10, 10, 200, 500), "Current cell: " + map.lastHighlightedCellIndex + System.Environment.NewLine +
                         "Province: " + name + System.Environment.NewLine + "Political Province: " + politicalProvince +
                         System.Environment.NewLine + "Climate: " + climate);
                }
               
            }
        }

        void HandleOnCellClick(int cellIndex)
        {
            Debug.Log("Clicked cell: " + cellIndex);
            if (selectedObject == null)
            {
                if (worldGlobeMap.cells[cellIndex].tag != null)
                {
                    //A new selected object is being selected
                    selectedObject = mappedObjects[worldGlobeMap.cells[cellIndex].tag];
                    selectedObject.selected = true;
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
                if (selectedObject.selected == false)
                {
                    //The selected object deselected itself
                    selectedObject = null;
                }
            }
        }

        void HandleOnCellEnter(int index)
        {
            if (selectedObject != null)
            {
                selectedObject.OnCellEnter(index);
            }
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
                int countryNameIndex = worldGlobeMap.GetCountryIndex("United States of America");
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
                        if (province.attrib["PoliticalProvince"] == "Georgia" || province.attrib["PoliticalProvince"] == "Georgia2" || province.attrib["PoliticalProvince"] == "Georgia3")
                        {
                            string name = province.name;
                            int regionIndex = province.mainRegionIndex;
                        }
                        if (province.attrib["PoliticalProvince"] == "Florida")
                        {
                            string name = province.name;
                            int regionIndex = province.mainRegionIndex;
                        }
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
                                if (mergeNeighbor)
                                {
                                    //Merge provinces
                                    worldGlobeMap.ProvinceTransferProvinceRegion(provinceIndex, neighbor.mainRegion, true);
                                    List<Province> provinceList = provinces.ToList();
                                    provinceList.Remove(neighbor);
                                    provinces = provinceList.ToArray();
                                    //Clear unused attributes
                                    if (loadedMapSettings.provinces) neighbor.attrib["PoliticalProvince"] = "";
                                    if (loadedMapSettings.terrain) neighbor.attrib["Terrain"] = "";
                                    if (loadedMapSettings.climate) neighbor.attrib["Climate"] = "";
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
                        Vector3 startingLocation = worldGlobeMap.cells[startingCellIndex].sphereCenter;
                        playerCharacter.vectorLocation = startingLocation;
                        worldGlobeMap.AddMarker(playerObject, startingLocation, 0.005f, false, 0.0f, true, true);
                        string playerID = playerCharacter.GetInstanceID().ToString();
                        worldGlobeMap.cells[startingCellIndex].tag = playerID;
                        mappedObjects.Add(playerID, playerCharacter);
                        //worldGlobeMap.cells[startingCellIndex].tag = CELL_PLAYER;
                    }
                    if (mountPoint.type == CULTURAL_POINT && loadedMapSettings.culturalLandmarks)
                    {
                        string mountPointName = mountPoint.name;
                        mountPointName = mountPointName.Replace(" ", "");
                        var model = Resources.Load<GameObject>("Prefabs/Landmarks/" + mountPointName);
                        var modelClone = Instantiate(model);
                        Landmark landmarkComponent = modelClone.GetComponent(typeof(Landmark)) as Landmark;
                        landmarkComponent.mountPoint = mountPoint;
                        worldGlobeMap.AddMarker(modelClone, mountPoint.localPosition, 0.01f, false, 0.0f, true, true);
                    }
                }

                #endregion
            }
        }

    }
}
