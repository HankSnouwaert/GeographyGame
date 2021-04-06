using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WPM
{
    public class GlobeInitiator : MonoBehaviour
    {
        private GameManager gameManager;
        private WorldMapGlobe worldGlobeMap;
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
            worldGlobeMap = gameManager.worldGlobeMap;
        }

        void ApplyGlobeSettings()
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
                            gameManager.mappedObjects.Add(playerID, gameManager.player);
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
                            gameManager.mappedObjects.Add(landmarkID, landmarkComponent);
                            gameManager.culturalLandmarks.Add(landmarkID, landmarkComponent);
                            gameManager.CulturalLandmarksByName.Add(landmarkComponent.objectName, landmarkComponent);
                        }
                    }

                    #endregion
                }

            }
            //}
        }

        /// <summary>
        ///  Instantiate all tourist regions and set initial region
        /// </summary>
        void InitTouristRegions()
        {
            #region Create North East Region
            TouristRegion northAmericaNorthEast = new TouristRegion();
            gameManager.touristRegions.Add(northAmericaNorthEast);
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
            gameManager.touristRegions.Add(northAmericaUSMidWestMidAtlantic);
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
            gameManager.touristRegions.Add(northAmericaUSSouthEast);
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
            gameManager.touristRegions.Add(northAmericaSouthWest);
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
            gameManager.touristRegions.Add(northAmericaNorthWestUS);
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
            gameManager.touristRegions.Add(northAmericaNorthWest);
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
            gameManager.touristRegions.Add(northAmericaCentralAmerica);
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

            gameManager.CurrentRegion = northAmericaUSSouthEast;
        }
    }
}
