using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WPM
{
    public class GlobeEditor : MonoBehaviour, IGlobeEditor
    {
        //Internal Interface References
        public GameObject playerPrefab;
        private IGameManager gameManager;
        private WorldMapGlobe worldMapGlobe;
        private IGlobeManager globeManager;
        private IPlayerManager playerManager;
        //Public Variables
        public bool[] ProvinceSettings { get; protected set; }  = new bool[NUMBER_OF_PROVINCE_ATTRIBUTES];
        //Private Variables
        private bool started = false;
        //MACROS
        //Province Attributes
        public const int NUMBER_OF_PROVINCE_ATTRIBUTES = 3;
        public const int POLITICAL_PROVINCE = 0;
        public const int TERRAIN = 1;
        public const int CLIMATE = 2;
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
                playerManager = gameManager.PlayerManager;
                if (playerManager == null)
                    errorHandler.ReportError("Player Manager missing", ErrorState.restart_scene);

                SetGlobeSettings();
                started = true;
            }
        }

        private void SetGlobeSettings()
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
            ProvinceSettings[POLITICAL_PROVINCE] = true;
            ProvinceSettings[TERRAIN] = false;
            ProvinceSettings[CLIMATE] = false;
            worldMapGlobe.drawAllProvinces = false;
            //}
        }

        public void MergeProvincesInCountry(Country country, bool[] provinceSettings)
        {
            if (!started)
                Start();

            int countryIndex = worldMapGlobe.GetCountryIndex(country.name);
            Province[] countryProvinces = worldMapGlobe.countries[countryIndex].provinces;
            if (countryIndex < 0 || countryProvinces == null)
            {
                errorHandler.ReportError("Attempted to merge invalid provinces in invalid country", ErrorState.close_window);
                return;
            }
            
            //Get all provinces for country and loop through them
            List<Province> countryProvinceList = countryProvinces.ToList();
            List<Province> removedProvinces = new List<Province>();
            Province province;
            for(int i = 0; i < countryProvinceList.Count; i++)
            {
                //Get province attributes
                province = countryProvinceList[i];
                removedProvinces = MergeProvinceWithNeighbors(province, provinceSettings, false);
                if (removedProvinces.Count > 0)
                {
                    foreach (Province removedProvince in removedProvinces)
                    {
                        countryProvinceList.Remove(removedProvince);
                    }
                }
            }  
        }

        public List<Province> MergeProvinceWithNeighbors(Province province, bool[] provinceSettings, bool mergeAcrossCountries)
        {
            if (!started)
                Start();

            int countryIndex = province.countryIndex;
           
            //Get all neighbors for province and loop through them
            int provinceIndex = worldMapGlobe.GetProvinceIndex(countryIndex, province.name);

            List<Province> neighbors = worldMapGlobe.ProvinceNeighboursOfMainRegion(provinceIndex);

            List<Province> removedProvinces = new List<Province>();

            bool mergeProvinces;

            foreach (Province neighbor in neighbors)
            {
                //This is a temp fix for provinces that haven't had their political name put in yet
                if (neighbor.attrib["PoliticalProvince"] == "")
                    neighbor.attrib["PoliticalProvince"] = neighbor.name;

                //Check that neighbor is in same country
                if (neighbor.countryIndex == countryIndex || mergeAcrossCountries)
                {
                    mergeProvinces = CheckIfProvincesShouldBeMerged(province, neighbor, provinceSettings);
                    if (mergeProvinces)
                    {
                        MergeProvinces(province, neighbor);
                        removedProvinces.Add(neighbor);
                    }  
                }
            }
            return removedProvinces;
        }

        /// <summary> 
        /// Given two provinces and a set of province settings, return a flag indicating if those provinces should be merged
        /// </summary>
        /// <param name="province1"> The first province for comparision </param>
        /// <param name="province2"> The second province for comparision </param>
        /// <param name="provinceSettings"> A list of flags indicating which attributes should be used for comparison</param>
        /// <returns> A flag indicating if the provinces should be merged </returns>
        private bool CheckIfProvincesShouldBeMerged(Province province1, Province province2, bool[] provinceSettings)
        {
            string[] province1Attributes = GetProvinceAttributes(province1);
            string[] province2Attributes = GetProvinceAttributes(province2);

            if(province1Attributes.Length != NUMBER_OF_PROVINCE_ATTRIBUTES || province1Attributes.Length != NUMBER_OF_PROVINCE_ATTRIBUTES)
            {
                errorHandler.ReportError("Province has invalid number of provinces", ErrorState.close_window);
                return false;
            }

            //Default to assuming the neighbor will be merged
            bool mergeNeighbor = true;

            //Loop through all attributes a province can have
            int i = 0;
            bool attributeSet;
            while (i < NUMBER_OF_PROVINCE_ATTRIBUTES)
            {
                //If the attribute is being used AND is different between the province and its neighbor, 
                // OR the attributes haven't been set for either province, abort the merge
                attributeSet = province1Attributes[i] != "" && province2Attributes[i] != "";
                if ((province1Attributes[i] != province2Attributes[i] && provinceSettings[i]) || !attributeSet)
                {
                    mergeNeighbor = false;
                }
                i++;
            }
            return mergeNeighbor;
        }

        /// <summary> 
        /// Merge a neighboring province into a given province
        /// </summary>
        /// <param name="mainProvince"> The province absorbing its neighbor </param>
        /// <param name="neighbor"> The province being absorbed </param>
        private void MergeProvinces(Province mainProvince, Province neighbor)
        {
            try
            {
                int provinceIndex = worldMapGlobe.GetProvinceIndex(mainProvince.countryIndex, mainProvince.name);

                //Merge provinces
                worldMapGlobe.ProvinceTransferProvinceRegion(provinceIndex, neighbor.mainRegion, true);
                //Clear unused attributes
                //if (!loadedMapSettings.provinces)
                //    neighbor.attrib["PoliticalProvince"] = "";
                //if (!loadedMapSettings.terrain)
                neighbor.attrib["Terrain"] = "";
                //if (!loadedMapSettings.climate)
                neighbor.attrib["Climate"] = "";
            }
            catch(System.Exception ex)
            {
                errorHandler.CatchException(ex, ErrorState.close_window);
            }
        }

        public string[] GetProvinceAttributes(Province province)
        {
            string[] provinceAttributes = new string[NUMBER_OF_PROVINCE_ATTRIBUTES];

            provinceAttributes[POLITICAL_PROVINCE] = province.attrib["PoliticalProvince"];
            if (provinceAttributes[POLITICAL_PROVINCE] == null) provinceAttributes[POLITICAL_PROVINCE] = "";
            provinceAttributes[TERRAIN] = province.attrib["Terrain"];
            if (provinceAttributes[TERRAIN] == null) provinceAttributes[TERRAIN] = "";
            provinceAttributes[CLIMATE] = province.attrib["Climate"];
            if (provinceAttributes[CLIMATE] == null) provinceAttributes[CLIMATE] = "";

            return provinceAttributes;
        }

        

    }

}
