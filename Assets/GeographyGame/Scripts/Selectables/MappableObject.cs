using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPM
{
    public class MappableObject : SelectableObject, IMappableObject
    {
        public int CellLocation { get; set; }
        public Vector3 VectorLocation { get; set; }
        public Vector2[] Latlon { get; set; }
        public List<Country> CountriesOccupied { get; set; } = new List<Country>();
        public List<Province> ProvincesOccupied { get; set; } = new List<Province>();
        public List<string> PoliticalProvincesOccupied { get; set; } = new List<string>();
        public List<string> ClimatesOccupied { get; set; } = new List<string>();
        protected GlobeManager globeManager;
        protected IGlobeParser globeParser;

        public override void Awake()
        {
            base.Awake();
            globeManager = FindObjectOfType<GlobeManager>();
            globeParser = globeManager.GlobeParser;
        }

        public virtual void UpdateLocation(int newCellIndex)
        {
            if(CellLocation != -1)
                map.cells[CellLocation].occupants.Remove(this);
            CellLocation = newCellIndex;
            map.cells[CellLocation].occupants.Add(this);
            VectorLocation = map.cells[CellLocation].sphereCenter;
            Latlon = map.cells[CellLocation].latlon;

            UpdateCountriesOccupied();
            UpdateProvincesOccupied();
            UpdatePoliticalProvincesOccupied();
            UpdateClimatesOccupied();
        }

        private void UpdateCountriesOccupied()
        {
            CountriesOccupied.Clear();
            List<int> countryIndexes = globeParser.CountryParser.GetCountriesInCell(CellLocation);
            foreach (int countryIndex in countryIndexes)
            {
                CountriesOccupied.Add(globeManager.WorldMapGlobe.countries[countryIndex]);
            }
        }
        private void UpdateProvincesOccupied()
        {
            ProvincesOccupied.Clear();
            List<int> provinceIndexes = globeParser.ProvinceParser.GetProvicesInCell(CellLocation);
            foreach (int provinceIndex in provinceIndexes)
            {
                ProvincesOccupied.Add(globeManager.WorldMapGlobe.provinces[provinceIndex]);
            }
        }
        private void UpdatePoliticalProvincesOccupied()
        {
            PoliticalProvincesOccupied.Clear();
            foreach (Province province in ProvincesOccupied)
            {
                PoliticalProvincesOccupied.Add(province.attrib["PoliticalProvince"]);
            }
        }
        private void UpdateClimatesOccupied()
        {
            ClimatesOccupied.Clear();
            foreach (Province province in ProvincesOccupied)
            {
                ClimatesOccupied.Add(province.attrib["ClimateGroup"]);
            }
        }
    }
}
