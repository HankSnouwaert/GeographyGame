﻿using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SpeedTutorMainMenuSystem;

namespace WPM
{
    public class GameManager : MonoBehaviour
    {
        [Header("Player Components")]
        public WorldMapGlobe worldGlobeMap; 
        WorldMapGlobe map;
        bool globeInitialized = true;

        enum SELECTION_MODE
        {
            NONE = 0,
            CUSTOM_PATH = 1,
            CUSTOM_COST = 2
        }

        GUIStyle labelStyle, labelStyleShadow, buttonStyle, sliderStyle, sliderThumbStyle;
        SELECTION_MODE selectionMode = SELECTION_MODE.NONE;
        int selectionState;
        // 0 = selecting first cell, 1 = selecting second cell
        int firstCell;
        // the cell index of the first selected cell when setting edge cost between two neighbour cells

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
            map.OnCellEnter += (int cellIndex) => Debug.Log("Entered cell: " + cellIndex);
            map.OnCellExit += (int cellIndex) => Debug.Log("Exited cell: " + cellIndex);
            //map.OnCellClick += HandleOnCellClick;
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
                         "Province: " + name + System.Environment.NewLine + "Province: " + politicalProvince +
                         System.Environment.NewLine + "Climate: " + climate);
                }
               
            }
        }

    void ApplyGlobeSettings()
        {
            Debug.Log("Applying Globe Settings");
            if (File.Exists(Application.dataPath + "/student.txt"))
            {
                string savedMapSettings = File.ReadAllText(Application.dataPath + "/student.txt");
                SaveObject loadedMapSettings = JsonUtility.FromJson<SaveObject>(savedMapSettings);
                worldGlobeMap.showFrontiers = loadedMapSettings.climate;
                
                int countryNameIndex = worldGlobeMap.GetCountryIndex("United States of America");
                if (countryNameIndex >= 0)
                {
                    Province[] provinces = worldGlobeMap.countries[countryNameIndex].provinces;
                    foreach (Province province in provinces)
                    {
                        string politicalProvince = province.attrib["PoliticalProvince"];
                        int provinceIndex = worldGlobeMap.GetProvinceIndex(countryNameIndex, province.name);
                        List<Province> neighbors = worldGlobeMap.ProvinceNeighboursOfMainRegion(provinceIndex);
                        foreach (Province neighbor in neighbors)
                        {
                            string neighborPoliticalProvince = neighbor.attrib["PoliticalProvince"];
                            if (neighborPoliticalProvince != "" && neighborPoliticalProvince == politicalProvince)
                            {
                                //worldGlobeMap.ProvinceTransferProvinceRegion(provinceIndex, neighbor.mainRegion, true);
                                province.attrib["Climate"] = "";
                            }
                        }
                    }
                }
                

            }
        }

        private void Update()
        {
            bool debug = worldGlobeMap.showFrontiers;
        }
    }
}
