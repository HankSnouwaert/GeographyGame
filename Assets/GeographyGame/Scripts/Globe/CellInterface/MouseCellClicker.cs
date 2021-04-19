using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPM
{
    /// <summary>
    /// Used to handle instances of the mouse clicking a cell of the world gloobe map
    /// </summary>
    public class MouseCellClicker : MonoBehaviour, ICellClicker
    {
        private GameManager gameManager;
        private IGlobeManager globeManager;
        private IUIManager uiManager;
        private IErrorHandler errorHandler;
        public ICellEnterer cellEnterer;
        public bool NewObjectSelected { get; set; } = false;

        void Awake()
        {
            cellEnterer = GetComponent(typeof(ICellEnterer)) as ICellEnterer;
        }

        void Start()
        {
            InterfaceFactory interfaceFactory = FindObjectOfType<InterfaceFactory>();
            gameManager = FindObjectOfType<GameManager>();
            globeManager = interfaceFactory.GlobeManager;
            uiManager = interfaceFactory.UIManager;
            errorHandler = interfaceFactory.ErrorHandler;
            globeManager.WorldGlobeMap.OnCellClick += HandleOnCellClick;
        }

        /// <summary>
        /// Called when a cell on the globe is selected via click
        /// </summary>
        /// <param name="cellIndex"></param> Index of the cell being clicked
        public void HandleOnCellClick(int cellIndex)
        {
            //Check that a hex is being clicked while an object is selected
            if (!uiManager.CursorOverUI && !gameManager.GamePaused && gameManager.SelectedObject != null)
            {
                //Make sure your not clicking on a new object
                if (NewObjectSelected)
                    NewObjectSelected = false;
                else
                    try
                    {
                        //Error Test
                        //errorHandler.reportError("Error: Erin is too cute", ErrorState.close_window);
                        gameManager.SelectedObject.OnCellClick(cellIndex);
                    }
                    catch (System.Exception ex)
                    {
                        errorHandler.CatchException(ex);
                    }
            }

        }    
    }
}
