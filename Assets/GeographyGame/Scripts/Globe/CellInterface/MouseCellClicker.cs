using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPM
{
    /// <summary>
    /// Used to handle instances of the mouse clicking a cell of the world globe map
    /// </summary>
    public class MouseCellClicker : MonoBehaviour, ICellClicker
    {
        //Internal Interface References
        private IGameManager gameManager;
        private IGlobeManager globeManager;
        private IUIManager uiManager;
        private ICellEnterer cellEnterer;
        //Public Variables
        public bool NewObjectSelected { get; set; } = false;
        //Error Checking
        private InterfaceFactory interfaceFactory;
        private IErrorHandler errorHandler;
        private bool componentMissing = false;

        void Awake()
        {
            interfaceFactory = FindObjectOfType<InterfaceFactory>();
            if (interfaceFactory == null)
                gameObject.SetActive(false);

            cellEnterer = GetComponent(typeof(ICellEnterer)) as ICellEnterer;
            if (cellEnterer == null)
                componentMissing = true;
        }

        void Start()
        {
            gameManager = interfaceFactory.GameManager;
            globeManager = interfaceFactory.GlobeManager;
            uiManager = interfaceFactory.UIManager;
            errorHandler = interfaceFactory.ErrorHandler;
            if (gameManager == null || globeManager == null || uiManager == null || errorHandler == null)
                gameObject.SetActive(false);
            else
            {
                try
                {
                    globeManager.WorldMapGlobe.OnCellClick += HandleOnCellClick;
                }
                catch(System.Exception ex)
                {
                    errorHandler.CatchException(ex, ErrorState.restart_scene);
                }
            }
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
                    gameManager.SelectedObject.OnCellClick(cellIndex);
            }
        }    
    }
}
