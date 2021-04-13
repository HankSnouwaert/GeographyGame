using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPM
{
    public class MappableObject : SelectableObject
    {
        public int cellLocation;
        public Vector3 vectorLocation;
        public Vector2[] latlon;
        public List<Country> countriesOccupied = new List<Country>();
        public List<Province> provincesOccupied = new List<Province>();
        public List<string> politicalProvincesOccupied = new List<string>();
        public List<string> climatesOccupied = new List<string>();
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
            if(cellLocation != -1)
                map.cells[cellLocation].occupants.Remove(this);
            cellLocation = newCellIndex;
            map.cells[cellLocation].occupants.Add(this);
            vectorLocation = map.cells[cellLocation].sphereCenter;
            latlon = map.cells[cellLocation].latlon;

            UpdateCountriesOccupied();
            UpdateProvincesOccupied();
            UpdatePoliticalProvincesOccupied();
            UpdateClimatesOccupied();
        }

        private void UpdateCountriesOccupied()
        {
            countriesOccupied.Clear();
            List<int> countryIndexes = globeParser.CountryParser.GetCountriesInCell(cellLocation);
            foreach (int countryIndex in countryIndexes)
            {
                countriesOccupied.Add(globeManager.WorldGlobeMap.countries[countryIndex]);
            }
        }
        private void UpdateProvincesOccupied()
        {
            provincesOccupied.Clear();
            List<int> provinceIndexes = globeParser.ProvinceParser.GetProvicesInCell(cellLocation);
            foreach (int provinceIndex in provinceIndexes)
            {
                provincesOccupied.Add(globeManager.WorldGlobeMap.provinces[provinceIndex]);
            }
        }
        private void UpdatePoliticalProvincesOccupied()
        {
            politicalProvincesOccupied.Clear();
            foreach (Province province in provincesOccupied)
            {
                politicalProvincesOccupied.Add(province.attrib["PoliticalProvince"]);
            }
        }
        private void UpdateClimatesOccupied()
        {
            climatesOccupied.Clear();
            foreach (Province province in provincesOccupied)
            {
                climatesOccupied.Add(province.attrib["ClimateGroup"]);
            }
        }
    }
}
