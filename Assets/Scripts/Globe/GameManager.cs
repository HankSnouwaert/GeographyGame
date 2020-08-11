using UnityEngine;
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
        public GameObject landmarkPrefab;
        WorldMapGlobe map;
        List<Landmark> culturalLandmarks = null;
        List<Landmark> naturalLandmarks = null;

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
                         "Province: " + name + System.Environment.NewLine + "Political Province: " + politicalProvince +
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
                
                #region Merge Provinces
                if (countryNameIndex >= 0)
                {
                    Province[] provinces = worldGlobeMap.countries[countryNameIndex].provinces;
                    int index = 0;
                    Province province;
                    while (index < provinces.Length)
                    {
                        province = provinces[index];
                        string politicalProvince = province.attrib["PoliticalProvince"];
                        int provinceIndex = worldGlobeMap.GetProvinceIndex(countryNameIndex, province.name);
                        List<Province> neighbors = worldGlobeMap.ProvinceNeighboursOfMainRegion(provinceIndex);
                        foreach (Province neighbor in neighbors)
                        {
                            string neighborPoliticalProvince = neighbor.attrib["PoliticalProvince"];
                            if (neighborPoliticalProvince != "" && neighborPoliticalProvince == politicalProvince)
                            {
                                worldGlobeMap.ProvinceTransferProvinceRegion(provinceIndex, neighbor.mainRegion, true);
                                List<Province> provinceList = provinces.ToList();
                                provinceList.Remove(neighbor);
                                provinces = provinceList.ToArray();
                                province.attrib["Climate"] = "";
                                province.name = politicalProvince;
                            }
                        }
                        index++;
                    }
                }
                worldGlobeMap.drawAllProvinces = false;
                #endregion
                #region Intantiate Landmarks
                //worldGlobeMap.ReloadMountPointsData();
                List<MountPoint> USmountPoints = new List<MountPoint>();
                int mountPointCount = worldGlobeMap.GetMountPoints(countryNameIndex, USmountPoints);
                
                foreach (MountPoint mountPoint in USmountPoints)
                {
                    if (mountPoint.type == 0)
                    {
                        GameObject landmark = Instantiate(landmarkPrefab);
                        Landmark landmarkComponent = landmark.GetComponent(typeof(Landmark)) as Landmark;
                        landmarkComponent.mountPoint = mountPoint;
                        //GameObject model = (GameObject)Resources.Load("Prefabs/Landmark", typeof(GameObject));
                        //landmark.model = model;
                        //landmark.transform.localScale = Vector3.one * 0.1f;
                        //Change this to move existing game object
                        worldGlobeMap.AddMarker(landmark, mountPoint.localPosition, 0.02f, false, 0.0f, true, true);
                    }
                }
                
                #endregion
            
            }
        }

        private void Update()
        {
            bool debug = worldGlobeMap.showFrontiers;
        }
    }
}
