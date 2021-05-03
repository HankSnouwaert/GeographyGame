using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPM
{
    abstract public class MappableObject : SelectableObject, IMappableObject
    {
        //Public Variables
        public Cell CellLocation { get; set; }
        public int CellIndex { get; set; }
        public Vector3 VectorLocation { get; set; }
        public Vector2[] Latlon { get; set; }
        public List<Country> CountriesOccupied { get; set; } = new List<Country>();
        public List<Province> ProvincesOccupied { get; set; } = new List<Province>();
        public List<string> PoliticalProvincesOccupied { get; set; } = new List<string>();
        public List<string> ClimatesOccupied { get; set; } = new List<string>();
        //Internal Interface References
        protected WorldMapGlobe worldMapGlobe;
        protected IGlobeManager globeManager;
        protected IUIManager uiManager;
        protected IGlobeParser globeParser;
        protected ICountryParser countryParser;
        protected IProvinceParser provinceParser;

        protected override void Start()
        {
            base.Start();
            if (gameObject.activeSelf)
            {
                globeManager = interfaceFactory.GlobeManager;
                uiManager = interfaceFactory.UIManager;
                if (globeManager == null || uiManager == null)
                    gameObject.SetActive(false);
                else
                {
                    worldMapGlobe = globeManager.WorldMapGlobe;
                    if (worldMapGlobe == null)
                        errorHandler.ReportError("World Map Globe Missing", ErrorState.restart_scene);

                    globeParser = globeManager.GlobeParser;
                    if (globeParser == null)
                        errorHandler.ReportError("Globe Parser missing", ErrorState.restart_scene);
                    else
                    {
                        countryParser = globeParser.CountryParser;
                        if(countryParser == null)
                            errorHandler.ReportError("Country Parser missing", ErrorState.restart_scene);
                        provinceParser = globeParser.ProvinceParser;
                        if(provinceParser == null)
                            errorHandler.ReportError("Province Parser missing", ErrorState.restart_scene);
                    }
                }
            }   
        }

        protected virtual void OnMouseDown()
        {
            if (selectionEnabled && !uiManager.CursorOverUI)
            {
                if (gameManager.SelectedObject == null)
                    Select();
                else
                {
                    if (gameManager.SelectedObject == (ISelectableObject)this)
                        Deselect();
                    else
                        gameManager.SelectedObject.OtherObjectSelected(this);
                }
            }
        }

        protected virtual void OnMouseEnter()
        {
            if (!uiManager.CursorOverUI && selectionEnabled)
            {
                gameManager.HighlightedObject = this;
            }
        }

        protected virtual void OnMouseExit()
        {
            if (selectionEnabled && !uiManager.CursorOverUI)
            {
                gameManager.HighlightedObject = null;
            }
        }

        public virtual void UpdateLocation(int newCellIndex)
        {
            if (worldMapGlobe.cells[newCellIndex] == null)
                errorHandler.ReportError("Cell does not exist", ErrorState.close_window);
            else
            {
                if (CellLocation != null)
                    CellLocation.occupants.Remove(this);
                CellLocation = worldMapGlobe.cells[newCellIndex];
                CellLocation.occupants.Add(this);
                VectorLocation = CellLocation.sphereCenter;
                Latlon = CellLocation.latlon;

                UpdateCountriesOccupied();
                UpdateProvincesOccupied();
                UpdatePoliticalProvincesOccupied();
                UpdateClimatesOccupied();
            }
            
        }

        private void UpdateCountriesOccupied()
        {
            if(CellLocation != null)
            {
                CountriesOccupied.Clear();
                List<Country> countries = countryParser.GetCountriesInCell(CellLocation);
                foreach (Country country in countries)
                {
                    CountriesOccupied.Add(country);
                }
            }
        }

        private void UpdateProvincesOccupied()
        {
            if (CellLocation != null)
            {
                ProvincesOccupied.Clear();
                List<Province> provinces = provinceParser.GetProvicesInCell(CellLocation);
                foreach (Province province in provinces)
                {
                    ProvincesOccupied.Add(province);
                }
            }
        }

        private void UpdatePoliticalProvincesOccupied()
        {
            PoliticalProvincesOccupied.Clear();
            foreach (Province province in ProvincesOccupied)
            {
                if(!PoliticalProvincesOccupied.Contains(province.attrib["PoliticalProvince"]))
                    PoliticalProvincesOccupied.Add(province.attrib["PoliticalProvince"]);
            }
        }

        private void UpdateClimatesOccupied()
        {
            ClimatesOccupied.Clear();
            foreach (Province province in ProvincesOccupied)
            {
                if (!ClimatesOccupied.Contains(province.attrib["ClimateGroup"]))
                    ClimatesOccupied.Add(province.attrib["ClimateGroup"]);
            }
        }
    }
}
