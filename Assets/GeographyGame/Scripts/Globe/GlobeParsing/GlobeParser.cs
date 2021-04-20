using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WPM
{
    public class GlobeParser : MonoBehaviour, IGlobeParser
    {
        private GameManager gameManager;
        private WorldMapGlobe worldMapGlobe;
        public IProvinceParser ProvinceParser { get; set; }
        public ICountryParser CountryParser { get; set; }
        public ILandmarkParser LandmarkParser { get; set; }

        public void Awake()
        {
            ProvinceParser = GetComponent(typeof(IProvinceParser)) as IProvinceParser;
            CountryParser = GetComponent(typeof(ICountryParser)) as ICountryParser;
            LandmarkParser = GetComponent(typeof(ILandmarkParser)) as ILandmarkParser;
        }

        public void Start()
        {
            InterfaceFactory interfaceFactory = FindObjectOfType<InterfaceFactory>();
            gameManager = FindObjectOfType<GameManager>();
            worldMapGlobe = interfaceFactory.GlobeManager.WorldMapGlobe;
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
        public List<int>[] GetCellsInRange(int startCell, int range = 0)
        {
            if (range < 0 || startCell < 0 || worldMapGlobe.cells.Count() < startCell)
            {
                //This will need to be replaced with an error message
                Debug.LogWarning("Invalid input for GetCellsInRange");
                return null;
            }

            int distance = 0;                             //distance measures how many rings of hexes we've moved out
            List<int>[] cells = new List<int>[range + 1]; //cells is an array of lists with each list other than 0 containing 
                                                          //the ring of hexes at that distance.  List 0 contains
                                                          //all hexes at every distance
                                                          //Add the startCell to List0
            cells[0] = new List<int>();
            cells[0].Add(startCell);
            worldMapGlobe.cells[startCell].flag = true;

            if (range > 0)
            {
                //Add the neighbors of the start cell to List1
                //And add them to List0
                distance++;
                cells[distance] = new List<int>();
                foreach (Cell neighbour in worldMapGlobe.GetCellNeighbours(startCell))
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
                    foreach (Cell neighbour in worldMapGlobe.GetCellNeighbours(cell))
                    {
                        if (!neighbour.flag)
                        {
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
                worldMapGlobe.cells[cell].flag = false;
            }
            return cells;
        }

    }
}
