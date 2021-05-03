using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WPM
{
    public class GlobeParser : MonoBehaviour, IGlobeParser
    {
        //Internal Reference Interfaces
        private IGameManager gameManager;
        private IGlobeManager globeManager;
        private WorldMapGlobe worldMapGlobe;
        //Public Facing Interface References
        public IProvinceParser ProvinceParser { get; protected set; }
        public ICountryParser CountryParser { get; protected set; }
        public ILandmarkParser LandmarkParser { get; protected set; }
        //Error Checking
        private InterfaceFactory interfaceFactory;
        private IErrorHandler errorHandler;
        private bool componentMissing = false;

        public void Awake()
        {
            interfaceFactory = FindObjectOfType<InterfaceFactory>();
            if (interfaceFactory == null)
                gameObject.SetActive(false);
            try
            {
                ProvinceParser = GetComponent(typeof(IProvinceParser)) as IProvinceParser;
                CountryParser = GetComponent(typeof(ICountryParser)) as ICountryParser;
                LandmarkParser = GetComponent(typeof(ILandmarkParser)) as ILandmarkParser;
            }
            catch
            {
                componentMissing = true;
            }
            if (ProvinceParser == null || CountryParser == null || LandmarkParser == null)
                componentMissing = true;
        }

        public void Start()
        {
            gameManager = interfaceFactory.GameManager;
            errorHandler = interfaceFactory.ErrorHandler;
            globeManager = interfaceFactory.GlobeManager;
            if (gameManager == null || errorHandler == null || globeManager == null)
                gameObject.SetActive(false);
            else
            {
                if (componentMissing)
                    errorHandler.ReportError("Globe Parser component missing", ErrorState.restart_scene);

                worldMapGlobe = globeManager.WorldMapGlobe;
                if (worldMapGlobe == null)
                    errorHandler.ReportError("World Map Globe missing", ErrorState.restart_scene);
            }
            
        }

        /// <summary> 
        /// Get all cells within a certain range (measured in cells) of a target cell
        /// Inputs:
        ///     startCell:  Target cell the range is being measured from
        ///     range:      The range (in cells) out from startCell that the method increments through
        /// Outputs:
        ///     cells:  An array of lists, with List0 containing all cells within range
        ///             and ListX containing the cells X number of cells away from the target cell
        /// </summary>
        public List<Cell>[] GetCellsInRange(Cell startCell, int range = 0)
        {
            int startCellIndex = startCell.index;

            if (range < 0 || startCellIndex < 0 || worldMapGlobe.cells.Count() < startCellIndex)
            {
                //This will need to be replaced with an error message
                Debug.LogWarning("Invalid input for GetCellsInRange");
                return null;
            }

            int distance = 0;                               //distance measures how many rings of hexes we've moved out
            List<Cell>[] cells = new List<Cell>[range + 1]; //cells is an array of lists with each list other than 0 containing 
                                                            //the ring of hexes at that distance.  List 0 contains
                                                            //all hexes at every distance
                                                            //Add the startCell to List0
            cells[0] = new List<Cell>();
            cells[0].Add(startCell);
            startCell.flag = true;

            if (range > 0)
            {
                //Add the neighbors of the start cell to List1
                //And add them to List0
                distance++;
                cells[distance] = new List<Cell>();
                foreach (Cell neighbour in worldMapGlobe.GetCellNeighbours(startCell.index))
                {
                    cells[0].Add(neighbour);
                    cells[distance].Add(neighbour);
                    neighbour.flag = true;
                }
            }
            while (distance < range)
            {
                //Continue adding rings of hexes to List0 and creating a new
                //List for each ring until the distance checked equals the 
                //disired range
                distance++;
                cells[distance] = new List<Cell>();
                foreach (Cell cell in cells[distance - 1])
                {
                    foreach (Cell neighbour in worldMapGlobe.GetCellNeighbours(cell.index))
                    {
                        if (!neighbour.flag)
                        {
                            cells[0].Add(neighbour);
                            cells[distance].Add(neighbour);
                            neighbour.flag = true;
                        }
                    }
                }
            }
            //Lower all cell flags
            foreach (Cell cell in cells[0])
            {
                cell.flag = false;
            }
            return cells;
        }

    }
}
