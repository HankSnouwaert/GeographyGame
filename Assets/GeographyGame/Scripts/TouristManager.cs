using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPM
{
    public class TouristManager : MonoBehaviour, ITouristManager, ITurnBasedObject
    {
        public TouristRegion CurrentRegion { get; set; }
        public int TouristSpawnRate { get; set; } = 10; //Number of rounds for a tourist to spawn
        public int TrackingTime { get; set; } = 10; //Number of rounds a tourist is remembered (Currently Unused)
        //Tourist Tracking Lists
        public List<int> RecentProvinceDestinations { get; set; } = new List<int>();  
        public List<string> RecentLandmarkDestinations { get; set; } = new List<string>();  
        public List<int> RecentCountryDestinations { get; set; } = new List<int>();  

        //private List<TouristRegion> regionsVisited = new List<TouristRegion>(); (Currently Unused)

        private InventoryTourist touristPrefab;
        private GameManager gameManager;
        private IErrorHandler errorHandler;
        private ITurnsManager turnsManager;
        private List<TouristRegion> touristRegions = new List<TouristRegion>(); 
        private int touristCounter = 0;
        private int touristsInCurrentRegion = -2;  //This number is the starting number of tourists * -1
        private int touristImageIndex = 0;
        private const int MIN_TIME_IN_REGION = 5;  
        private const int MAX_TIME_IN_REGION = 10;
        //Tourist Image Management
        private string[] touristImageFiles; 
        private const int NUMBER_OF_TOURIST_IMAGES = 8; 
        

        private void Awake()
        {
            gameManager = FindObjectOfType<GameManager>();
            turnsManager = gameManager.TurnsManager;
            turnsManager.TurnBasedObjects.Add(this);
            touristPrefab = Resources.Load<InventoryTourist>("Prefabs/Inventory/InventoryTourist");
            InitTouristRegions();
            //Set Tourist Images
            touristImageFiles = new string[8];
            touristImageFiles[0] = "Images/Tourist1";
            touristImageFiles[1] = "Images/Tourist2";
            touristImageFiles[2] = "Images/Tourist3";
            touristImageFiles[3] = "Images/Tourist4";
            touristImageFiles[4] = "Images/Tourist5";
            touristImageFiles[5] = "Images/Tourist6";
            touristImageFiles[6] = "Images/Tourist7";
            touristImageFiles[7] = "Images/Tourist8";
        }

        private void Start()
        {
            errorHandler = FindObjectOfType<InterfaceFactory>().ErrorHandler;
        }

        public void EndOfTurn(int turns)
        {
            for (int i = 0; i < turns; i++)
            {
                touristCounter++;
                if (touristCounter >= TouristSpawnRate)
                {
                    touristCounter = 0;
                    try
                    {
                        GenerateTourist();
                    }
                    catch (System.Exception ex)
                    {
                        errorHandler.catchException(ex);
                    }
                }
            }
        }

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
            gameManager.Player.AddItem(tourist, 0);
            //Check if a region switch is needed
            touristsInCurrentRegion++;
            int rand = Random.Range(MIN_TIME_IN_REGION, MAX_TIME_IN_REGION);
            if (touristsInCurrentRegion >= rand)
            {
                //Switch regions
                int newRegionNeighbourIndex = Random.Range(0, CurrentRegion.neighbouringRegions.Count - 1);
                CurrentRegion = CurrentRegion.neighbouringRegions[newRegionNeighbourIndex];
                touristsInCurrentRegion = 0;
            }
        }

        /// <summary>
        ///  Instantiate all tourist regions and set initial region
        /// </summary>
        private void InitTouristRegions()
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
            northAmericaNorthEast.landmarks.Add("Statue Of Liberty");
            northAmericaNorthEast.landmarks.Add("CN Tower");
            northAmericaNorthEast.landmarks.Add("Parliament Hill");
            northAmericaNorthEast.landmarks.Add("Fairmont Le Château Frontenac");
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
            northAmericaUSMidWestMidAtlantic.landmarks.Add("Statue Of Liberty");
            northAmericaUSMidWestMidAtlantic.landmarks.Add("Washington Monument");
            northAmericaUSMidWestMidAtlantic.landmarks.Add("Lincoln Memorial");
            northAmericaUSMidWestMidAtlantic.landmarks.Add("Gateway Arch");
            northAmericaUSMidWestMidAtlantic.landmarks.Add("CN Tower");
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
            northAmericaUSSouthEast.landmarks.Add("Washington Monument");
            northAmericaUSMidWestMidAtlantic.landmarks.Add("Lincoln Memorial");
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
            northAmericaSouthWest.landmarks.Add("Hoover Dam");
            northAmericaSouthWest.landmarks.Add("Golden Gate Bridge");
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
            northAmericaNorthWestUS.landmarks.Add("Space Needle");
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
            northAmericaNorthWestUS.landmarks.Add("Space Needle");
            #endregion

            #region Create Central America
            TouristRegion northAmericaCentralAmerica = new TouristRegion();
            touristRegions.Add(northAmericaCentralAmerica);
            northAmericaCentralAmerica.regionName = "North America: Central America";
            //Provinces
            northAmericaCentralAmerica.provinces.Add(3892); //Texas
                                                            //Landmarks
            northAmericaCentralAmerica.landmarks.Add("Castillo of Chichen Itza");
            northAmericaCentralAmerica.landmarks.Add("Ruins of Tikal");
            northAmericaCentralAmerica.landmarks.Add("Angel of Mexican Independence");
            northAmericaCentralAmerica.landmarks.Add("Panama Canal");
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

    }
}
