﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WPM
{
    public class GlobeInitializer : MonoBehaviour, IGlobeInitializer
    {
        private GameManager gameManager;
        private WorldMapGlobe worldGlobeMap;
        private GlobeManager globeManager;
        private IGlobeInfo globeInfo;
        readonly string startingCountry = "United States of America";
        readonly string startingProvince = "North Carolina";
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

        private void Awake()
        {
            gameManager = FindObjectOfType<GameManager>();
            globeManager = FindObjectOfType<GlobeManager>();
            worldGlobeMap = globeManager.worldGlobeMap;
        }
        private void Start()
        {
            globeInfo = globeManager.GlobeInfo;
        }

        public void ApplyGlobeSettings()
        {
            Debug.Log("Applying Globe Settings");
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

            foreach (Country country in worldGlobeMap.countries)
            {
                if (country.continent == "North America")
                {
                    int countryNameIndex = worldGlobeMap.GetCountryIndex(country.name);
                    MergeProvinces(country, provinceSettings);
                    IntantiateMappables(country);
                }

            }
            //}
        }

        private void MergeProvinces(Country country, bool[] provinceSettings)
        {
            int countryNameIndex = worldGlobeMap.GetCountryIndex(country.name);
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
        }

        private void IntantiateMappables(Country country)
        {
            int countryNameIndex = worldGlobeMap.GetCountryIndex(country.name);
            //worldGlobeMap.ReloadMountPointsData();
            List<MountPoint> countryMountPoints = new List<MountPoint>();
            int mountPointCount = worldGlobeMap.GetMountPoints(countryNameIndex, countryMountPoints);

            foreach (MountPoint mountPoint in countryMountPoints)
            {
                if (mountPoint.type == START_POINT && mountPoint.provinceIndex == worldGlobeMap.GetProvinceIndex(startingCountry, startingProvince))
                {
                    GameObject playerObject = Instantiate(gameManager.playerPrefab);
                    gameManager.player = playerObject.GetComponent(typeof(PlayerCharacter)) as PlayerCharacter;
                    int startingCellIndex = worldGlobeMap.GetCellIndex(mountPoint.localPosition);
                    gameManager.player.cellLocation = startingCellIndex;
                    gameManager.player.latlon = worldGlobeMap.cells[startingCellIndex].latlon;
                    Vector3 startingLocation = worldGlobeMap.cells[startingCellIndex].sphereCenter;
                    gameManager.player.vectorLocation = startingLocation;
                    float playerSize = gameManager.player.GetSize();
                    worldGlobeMap.AddMarker(playerObject, startingLocation, playerSize, false, 0.0f, true, true);
                    string playerID = gameManager.player.GetInstanceID().ToString();
                    worldGlobeMap.cells[startingCellIndex].occupants.Add(gameManager.player);
                    globeInfo.MappedObjects.Add(playerID, gameManager.player);
                }
                if (mountPoint.type == CULTURAL_POINT) //&& loadedMapSettings.culturalLandmarks)
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
                    globeInfo.MappedObjects.Add(landmarkID, landmarkComponent);
                    globeInfo.CulturalLandmarks.Add(landmarkID, landmarkComponent);
                    globeInfo.CulturalLandmarksByName.Add(landmarkComponent.objectName, landmarkComponent);
                }
            }
        }

    }

}
