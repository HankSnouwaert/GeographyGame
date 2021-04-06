using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPM
{
    public class GlobeManager : MonoBehaviour, IGlobeManager
    {
        private GameManager gameManager;
        public WorldMapGlobe worldGlobeMap;
        public IGlobeParser GlobeParser { get; set; }
        public GameObject globeParserObject;
        public ICellCursorInterface CellCursorInterface { get; set; }
        public GameObject cellCursorInterfaceObject;

        public IGlobeInitializer GlobeInitializer { get; set; }
        public GameObject globeInitializerObject;

        private void Awake()
        {
            gameManager = FindObjectOfType<GameManager>();
            worldGlobeMap = gameManager.worldGlobeMap;
            GlobeParser = globeParserObject.GetComponent(typeof(IGlobeParser)) as IGlobeParser;
            CellCursorInterface = cellCursorInterfaceObject.GetComponent(typeof(ICellCursorInterface)) as ICellCursorInterface;
            GlobeInitializer = globeInitializerObject.GetComponent(typeof(IGlobeInitializer)) as IGlobeInitializer;
        }

        private void Start()
        {
            try
            {
                GlobeInitializer.ApplyGlobeSettings();
            }
            catch (System.Exception ex)
            {
                //errorState = ErrorState.close_application;
                //DisplayError(ex.Message, ex.StackTrace);
            }
        }
    }
}
