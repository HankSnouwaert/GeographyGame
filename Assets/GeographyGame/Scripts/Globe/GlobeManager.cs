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
        private GameObject globeInitializerObject;
        [SerializeField]
        private GameObject globeInfoObject;
        //Child Interfaces
        public IGlobeParser GlobeParser { get; protected set; }
        public ICellCursorInterface CellCursorInterface { get; protected set; }
        public IGlobeInitializer GlobeInitializer { get; protected set; }
        public IGlobeInfo GlobeInfo { get; protected set; }
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

                GlobeInitializer = globeInitializerObject.GetComponent(typeof(IGlobeInitializer)) as IGlobeInitializer;
                if (GlobeInitializer == null)
                    componentMissing = true;

                GlobeInfo = globeInfoObject.GetComponent(typeof(IGlobeInfo)) as IGlobeInfo;
                if (GlobeInfo == null)
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

                GlobeInitializer.ApplyGlobeSettings();
            }
        }
    }
}
