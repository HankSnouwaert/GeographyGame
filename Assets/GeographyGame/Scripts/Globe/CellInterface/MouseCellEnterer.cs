using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPM
{
    /// <summary>
    ///  Used to handle instances of the mouse moving over a cell on the world globe map
    /// </summary>
    public class MouseCellEnterer : MonoBehaviour, ICellEnterer
    {
        //Internal Interface References
        private IGameManager gameManager;
        private IUIManager uiManager;
        private IGlobeManager globeManager;
        private ICellCursorInterface cellCursorInterface;
        private WorldMapGlobe worldMapGlobe;
        //Error Checking
        private InterfaceFactory interfaceFactory;
        private IErrorHandler errorHandler;
        private bool componentMissing = false;

        void Awake()
        {
            interfaceFactory = FindObjectOfType<InterfaceFactory>();
            if (interfaceFactory == null)
                gameObject.SetActive(false);

            try
            {
                cellCursorInterface = GetComponent(typeof(ICellCursorInterface)) as ICellCursorInterface;
            }
            catch
            {
                componentMissing = true;
            }
            if (cellCursorInterface == null)
                componentMissing = true;
        }

        void Start()
        {
            gameManager = interfaceFactory.GameManager;
            globeManager = interfaceFactory.GlobeManager;
            uiManager = interfaceFactory.UIManager;
            if (gameManager == null || globeManager == null || uiManager == null)
                gameObject.SetActive(false);
            else
            {
                if (componentMissing)
                    errorHandler.ReportError("Mouse Cell Enterer missing component", ErrorState.restart_scene);

                worldMapGlobe = globeManager.WorldMapGlobe;
                if (worldMapGlobe == null)
                    errorHandler.ReportError("World Map Globe missing", ErrorState.restart_scene);

                try
                {
                    globeManager.WorldMapGlobe.OnCellEnter += HandleOnCellEnter;
                }
                catch (System.Exception ex) 
                {
                    errorHandler.CatchException(ex, ErrorState.restart_scene);
                }
            }
        }

        /// <summary>
        ///  Called when the mouse cursor moves over a cell on the world globe map
        /// </summary>
        /// <param name="cellIndex"></param> The cell the mouse cursor moves over>
        /// <returns></returns> 
        public void HandleOnCellEnter(int cellIndex)
        {
            //Run any on cell enter method for the selected object
            if (!uiManager.CursorOverUI && !gameManager.GamePaused && gameManager.SelectedObject != null)
            {
                if (gameManager.HighlightedObject != null)
                    gameManager.SelectedObject.OnSelectableEnter(gameManager.HighlightedObject);
                try
                {
                    gameManager.SelectedObject.OnCellEnter(cellIndex);
                    cellCursorInterface.highlightedCellIndex = cellIndex;
                    cellCursorInterface.highlightedCell = worldMapGlobe.cells[cellIndex];
                }
                catch
                {
                    errorHandler.ReportError("Invalid cell index for cell cell enterer", ErrorState.close_window);
                }
            }
        }
    }
}
