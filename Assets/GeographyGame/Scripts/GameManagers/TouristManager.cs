using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPM
{
    public class TouristManager : MonoBehaviour, ITouristManager, ITurnBasedObject
    {
        [SerializeField]
        private GameObject touristPrefab;
        [SerializeField]
        private GameObject touristTutorialObject;
        public int ActiveTutorial { get; set; } = 0;

        //Protected Public Variables
        public TouristRegion CurrentRegion { get; protected set; }
        public int TouristSpawnRate { get; protected set; } = 10; //Number of rounds for a tourist to spawn
        public int TrackingTime { get; protected set; } = 10; //Number of rounds a tourist is remembered (Currently Unused)
        //Tourist Tracking Lists
        public List<int> RecentProvinceDestinations { get; set; } = new List<int>();  
        public List<string> RecentLandmarkDestinations { get; set; } = new List<string>();  
        public List<int> RecentCountryDestinations { get; set; } = new List<int>();

        //Public Method Executed Flags
        public bool TouristSelected { get; set; } = false;

        //private List<TouristRegion> regionsVisited = new List<TouristRegion>(); (Currently Unused)

        //Local Interface References
        private IGameManager gameManager;
        private IPlayerManager playerManager;
        private ITurnsManager turnsManager;
        private IUIManager uiManager;
        private IInventoryUI inventoryUI;
        private ITouristTutorial touristTutorial;
        //Internal Variables
        private List<TouristRegion> touristRegions = new List<TouristRegion>(); 
        private int touristCounter = 0;
        private int touristsInCurrentRegion = -2;  //This number is the starting number of tourists * -1
        private int touristImageIndex = 0;
        //Constants
        private const int MIN_TIME_IN_REGION = 5;  
        private const int MAX_TIME_IN_REGION = 10;
        private const int STARTING_NUMBER_OF_TOURISTS = 2;
        //Tourist Image Management
        private string[] touristImageFiles; 
        private const int NUMBER_OF_TOURIST_IMAGES = 8;
        //Error Checking
        private InterfaceFactory interfaceFactory;
        private IErrorHandler errorHandler;
        private bool componentMissing = false;

        private void Awake()
        {
            interfaceFactory = FindObjectOfType<InterfaceFactory>();
            if (interfaceFactory == null)
                gameObject.SetActive(false);

            if (touristPrefab == null)
                componentMissing = true;

            try
            {
                touristTutorial = touristTutorialObject.GetComponent(typeof(ITouristTutorial)) as ITouristTutorial;
                if (touristTutorial == null)
                    componentMissing = true;
            }
            catch
            {
                componentMissing = true;
            }

            InitTouristRegions();
            InitTouristImages();
        }

        private void Start()
        {
            gameManager = interfaceFactory.GameManager;
            errorHandler = interfaceFactory.ErrorHandler;
            uiManager = interfaceFactory.UIManager;
            if(gameManager == null || errorHandler == null || uiManager == null)
            {
                gameObject.SetActive(false);
            }
            else
            {
                if(componentMissing)
                    errorHandler.ReportError("Tourist Manager component missing", ErrorState.restart_scene);

                inventoryUI = uiManager.InventoryUI;
                if(inventoryUI == null)
                    errorHandler.ReportError("Inventory UI missing", ErrorState.restart_scene);
                turnsManager = gameManager.TurnsManager;
                if (turnsManager == null)
                    errorHandler.ReportError("Turns Manager missing", ErrorState.restart_scene);
                else
                {
                    if(turnsManager.TurnBasedObjects == null)
                        errorHandler.ReportError("Turn based objects list not initialized", ErrorState.restart_scene);
                    else
                        turnsManager.TurnBasedObjects.Add(this);
                }

                playerManager = gameManager.PlayerManager;
                if(playerManager == null)
                    errorHandler.ReportError("Player Manager missing", ErrorState.restart_scene);
            }
        }

        public void EndOfTurn(int turns)
        {
            for (int i = 0; i < turns; i++)
            {
                touristCounter++;
                if (touristCounter >= TouristSpawnRate)
                {
                    touristCounter = 0;
                    GenerateTourist();
                }
            }
        }

        /// <summary> 
        /// Called when a tourist needs to be generated and added to the player's inventory
        /// </summary>
        public void GenerateTourist()
        {
            //Get Player Character 
            IPlayerCharacter playerCharacter = playerManager.PlayerCharacter;
            if(playerCharacter == null)
            {
                errorHandler.ReportError("Player character missing", ErrorState.restart_scene);
                return;
            }
            //Intantiate tourist
            GameObject touristObject = Instantiate(touristPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            touristObject.transform.parent = inventoryUI.UIObject.transform;
            IInventoryTourist tourist = touristObject.GetComponent(typeof(IInventoryTourist)) as IInventoryTourist;
            if(tourist == null)
            {
                errorHandler.ReportError("Tourist component of tourist prefab missing", ErrorState.restart_scene);
                return;
            }
            //Give tourist its image
            tourist.InventoryIcon = Resources.Load<Sprite>(touristImageFiles[touristImageIndex]);
            touristImageIndex++;
            if (touristImageIndex >= NUMBER_OF_TOURIST_IMAGES)
                touristImageIndex = 0;
            //Add tourist to player's inventory
            playerCharacter.Inventory.AddItem(tourist, 0);
            if (ActiveTutorial == 1)
            {
                //Add Code Here to Change the Tourist's Destination
            }
            else
            {
                //Check if a region switch is needed
                touristsInCurrentRegion++;
                int rand = Random.Range(MIN_TIME_IN_REGION, MAX_TIME_IN_REGION);
                if (touristsInCurrentRegion >= rand)
                {
                    //Switch regions
                    try
                    {
                        int newRegionNeighbourIndex = Random.Range(0, CurrentRegion.neighbouringRegions.Count - 1);
                        CurrentRegion = CurrentRegion.neighbouringRegions[newRegionNeighbourIndex];
                        touristsInCurrentRegion = 0;
                    }
                    catch (System.Exception ex)
                    {
                        errorHandler.CatchException(ex, ErrorState.restart_scene);
                    }
                }
            }
        }

        public void InitiateTourists()
        {
            for (int i = 0; i < STARTING_NUMBER_OF_TOURISTS; i++)
            {
                GenerateTourist();
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
            northAmericaUSSouthEast.landmarks.Add("Lincoln Memorial");
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

        /// <summary> 
        /// Initializes array with file locations of tourist images
        /// </summary>
        private void InitTouristImages()
        {
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
    }
}
