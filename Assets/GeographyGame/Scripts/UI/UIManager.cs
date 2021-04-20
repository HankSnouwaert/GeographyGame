using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace WPM
{
    public class UIManager : MonoBehaviour, IUIManager
    {
        [Header("Child Objects")]
        [SerializeField]
        private GameObject mouseOverInfoUIObject;
        [SerializeField]
        private GameObject inventoryUIObject;
        [SerializeField]
        private GameObject navigationUIObject;
        [SerializeField]
        private GameObject dropOffUIObject;
        [SerializeField]
        private GameObject scoreUIObject; 
        [SerializeField]
        private GameObject turnsUIObject;
        [SerializeField]
        private GameObject gameOverUIObject;
        [SerializeField]
        private GameObject gameMenuUIObject; 
        [SerializeField]
        private GameObject inventoryPopUpUIObject;
        //Child Interfaces
        public IMouseOverInfoUI MouseOverInfoUI { get; protected set; }
        public INavigationUI NavigationUI { get; protected set; }
        public IDropOffUI DropOffUI { get; protected set; }
        public IScoreUI ScoreUI { get; protected set; }
        public ITurnsUI TurnsUI { get; protected set; }
        public IGameOverUI GameOverUI { get; protected set; }
        public IGameMenuUI GameMenuUI { get; protected set; }
        public IInventoryPopUpUI InventoryPopUpUI { get; protected set; }
        //Public Flags
        public bool CursorOverUI { get; set; } = false;
        public bool ClosingUI { get; set; } = false;
        //Local Interface References
        private IGameManager gameManager;
        private IGlobeManager globeManager;
        private ICellCursorInterface cellCursorInterface;
        //Error Checking
        private IErrorHandler errorHandler;
        private InterfaceFactory interfaceFactory;
        private bool componentMissing = false;

        void Awake()
        {
            interfaceFactory = FindObjectOfType<InterfaceFactory>();
            if (interfaceFactory == null)
                gameObject.SetActive(false);
            try
            {
                MouseOverInfoUI = mouseOverInfoUIObject.GetComponent(typeof(IMouseOverInfoUI)) as IMouseOverInfoUI;
                if (MouseOverInfoUI == null)
                    componentMissing = true;

                NavigationUI = navigationUIObject.GetComponent(typeof(INavigationUI)) as INavigationUI;
                if (NavigationUI == null)
                    componentMissing = true;

                DropOffUI = dropOffUIObject.GetComponent(typeof(IDropOffUI)) as IDropOffUI;
                if (DropOffUI == null)
                    componentMissing = true;

                ScoreUI = scoreUIObject.GetComponent(typeof(IScoreUI)) as IScoreUI;
                if (ScoreUI == null)
                    componentMissing = true;

                TurnsUI = turnsUIObject.GetComponent(typeof(ITurnsUI)) as ITurnsUI;
                if (TurnsUI == null)
                    componentMissing = true;

                GameOverUI = gameOverUIObject.GetComponent(typeof(IGameOverUI)) as IGameOverUI;
                if (GameOverUI == null)
                    componentMissing = true;

                GameMenuUI = gameMenuUIObject.GetComponent(typeof(IGameMenuUI)) as IGameMenuUI;
                if (GameMenuUI == null)
                    componentMissing = true;

                InventoryPopUpUI = inventoryPopUpUIObject.GetComponent(typeof(IInventoryPopUpUI)) as IInventoryPopUpUI;
                if (InventoryPopUpUI == null)
                    componentMissing = true;

                //Inventory UI needs to be added here when I refactor it
                if (inventoryUIObject == null)
                    componentMissing = true;
            }
            catch
            {
                componentMissing = true;
            }
        }

        void Start()
        {
            gameManager = interfaceFactory.GameManager;
            globeManager = interfaceFactory.GlobeManager;
            errorHandler = interfaceFactory.ErrorHandler;
            if (gameManager == null || globeManager == null || errorHandler == null)
                gameObject.SetActive(false);
            else
            {
                cellCursorInterface = globeManager.CellCursorInterface;
                if (cellCursorInterface == null)
                    errorHandler.ReportError("CellCurserInterface no found", ErrorState.restart_scene);
            }
            
        }

        void Update()
        {
            CursorOverUI = CheckForMouseOverUI();
            //Make sure the player doesn't click through a UI while closing it
            if (ClosingUI)
            {
                ClosingUI = false;
                int currentCellIndex = cellCursorInterface.highlightedCellIndex;
                if(currentCellIndex != -1)
                    cellCursorInterface.CellEnterer.HandleOnCellEnter(currentCellIndex);
                CursorOverUI = true;
            }
            MouseOverInfoUI.UpdateUI();

            //Check if player is clicking out of a popup
            if (Input.GetMouseButton(0) && InventoryPopUpUI.TempPopUp == true)
                InventoryPopUpUI.ClearPopUp(false);
        }

        public void GameOver()
        {
            mouseOverInfoUIObject.SetActive(false);
            inventoryUIObject.SetActive(false);
            inventoryPopUpUIObject.SetActive(false);
            GameOverUI.OpenUI();
        }
        
        /// <summary>
        ///  Determines if the cursor is over a UI elements
        ///  NOTE: Most of this code was copyed from an online example
        /// </summary>
        private bool CheckForMouseOverUI()
        {
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            if (pointerEventData == null)
            {
                errorHandler.ReportError("Invalid Pointer Event Data", ErrorState.close_window);
                return false;
            }
            pointerEventData.position = Input.mousePosition;
   
            if (pointerEventData.position == null)
            {
                errorHandler.ReportError("Invalid Pointer Event Data", ErrorState.close_window);
                return false;
            }

            List<RaycastResult> raycastResultList = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, raycastResultList);
            try
            {
                for (int i = 0; i < raycastResultList.Count; i++)
                {
                    if (raycastResultList[i].gameObject.GetComponent<IUIElement>() == null)
                    {
                        raycastResultList.RemoveAt(i);
                        i--;
                    }
                }
            }
            catch
            {
                errorHandler.ReportError("Error Checking for Mouse Over UI", ErrorState.close_window);
                return false;
            }
            return raycastResultList.Count > 0;
        }

        
    }
}
