using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPM
{
    /// <summary>
    ///  Used to handle instances of the mouse moving over a cell on the world globe map
    /// </summary>
    public class MouseCellExiter : MonoBehaviour, ICellExiter
    {
        private GameManager gameManager;
        private IUIManager uiManager;
        private IGlobeManager globeManager;

        void Start()
        {
            InterfaceFactory interfaceFactory = FindObjectOfType<InterfaceFactory>();
            gameManager = FindObjectOfType<GameManager>();
            globeManager = interfaceFactory.GlobeManager;
            globeManager.WorldMapGlobe.OnCellExit += HandleOnCellExit;
            uiManager = FindObjectOfType<InterfaceFactory>().UIManager;
        }

        /// <summary>
        ///  Called when the mouse cursor moves out of a cell on the world globe map
        /// </summary>
        /// <param name="cellIndex"></param> The cell the mouse cursor out of over>
        /// <returns></returns> 
        public void HandleOnCellExit(int cellIndex)
        {
            /*
            if (!uiManager.CursorOverUI && !gameManager.GamePaused)
            {
                Province province = gameManager.worldGlobeMap.provinceHighlighted;
                if (province == null || uiManager.CursorOverUI)
                    uiManager.mouseOverInfoUIObject.SetActive(false);
            }
            */
        }
    }
}

