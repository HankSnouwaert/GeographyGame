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

        private void Awake()
        {
            gameManager = FindObjectOfType<GameManager>();
            worldGlobeMap = gameManager.worldGlobeMap;
            GlobeParser = globeParserObject.GetComponent(typeof(IGlobeParser)) as IGlobeParser;
            CellCursorInterface = cellCursorInterfaceObject.GetComponent(typeof(ICellCursorInterface)) as ICellCursorInterface;
        }

    }
}
