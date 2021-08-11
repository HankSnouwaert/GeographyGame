using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPM
{
    public class GlobeManager : MonoBehaviour, IGlobeManager
    {
        [Header("Child Objects")]
        [SerializeField]
        private GameObject globeParserObject;
        [SerializeField]
        private GameObject cellCursorInterfaceObject;
        [SerializeField]
        private GameObject globeEditorObject;
        [SerializeField]
        private GameObject mappablesManagerObject;
        //Child Interfaces
        public IGlobeParser GlobeParser { get; protected set; }
        public ICellCursorInterface CellCursorInterface { get; protected set; }
        public IGlobeEditor GlobeEditor { get; protected set; }
        public IMappablesManager MappablesManager { get; protected set; }
        //World Globe Map doesn't use an interface
        public WorldMapGlobe WorldMapGlobe { get; protected set; }
        //Local Interface References
        private IGameManager gameManager;
        private IErrorHandler errorHandler;
        //Error Checking
        private InterfaceFactory interfaceFactory;
        private bool componentMissing = false;
        private bool worldMapGlobeMissing = false;

        private void Awake()
        {
            interfaceFactory = FindObjectOfType<InterfaceFactory>();
            if (interfaceFactory == null)
                gameObject.SetActive(false);
            try
            {
                WorldMapGlobe = FindObjectOfType<WorldMapGlobe>();
                if (WorldMapGlobe == null)
                    worldMapGlobeMissing = true;

                GlobeParser = globeParserObject.GetComponent(typeof(IGlobeParser)) as IGlobeParser;
                if (GlobeParser == null)
                    componentMissing = true;

                CellCursorInterface = cellCursorInterfaceObject.GetComponent(typeof(ICellCursorInterface)) as ICellCursorInterface;
                if (CellCursorInterface == null)
                    componentMissing = true;

                GlobeEditor = globeEditorObject.GetComponent(typeof(IGlobeEditor)) as IGlobeEditor;
                if (GlobeEditor == null)
                    componentMissing = true;

                MappablesManager = mappablesManagerObject.GetComponent(typeof(IMappablesManager)) as IMappablesManager;
                if (MappablesManager == null)
                    componentMissing = true;
            }
            catch
            {
                componentMissing = true;
            }
        }

        private void Start()
        {
            errorHandler = interfaceFactory.ErrorHandler;
            gameManager = interfaceFactory.GameManager;
            if (errorHandler == null || gameManager == null)
                gameObject.SetActive(false);
            else
            {
                if (worldMapGlobeMissing)
                    errorHandler.ReportError("World Map Globe Missing", ErrorState.close_application);
                if (componentMissing)
                    errorHandler.ReportError("Component Missing", ErrorState.restart_scene);
                ApplyGlobeSettings();
            }
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                WorldMapGlobe.dragOnScreenEdges = false;
            else
                WorldMapGlobe.dragOnScreenEdges = true;
        }

        private void ApplyGlobeSettings()
        {
            foreach (Country country in WorldMapGlobe.countries)
            {
                if (country.continent == "North America")
                {
                    GlobeEditor.MergeProvincesInCountry(country, GlobeEditor.ProvinceSettings);
                    MappablesManager.IntantiateMappables(country);
                }
            }
        }
    }
}
