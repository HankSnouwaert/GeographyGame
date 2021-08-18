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
        [SerializeField]
        private GameObject tutorialUIObject;
        //Public Game Objects
        public Canvas MainCanvas { get; protected set; }
        //Child Interfaces
        public IMouseOverInfoUI MouseOverInfoUI { get; protected set; }
        public IInventoryUI InventoryUI { get; protected set; }
        public INavigationUI NavigationUI { get; protected set; }
        public IDropOffUI DropOffUI { get; protected set; }
        public IScoreUI ScoreUI { get; protected set; }
        public ITurnsUI TurnsUI { get; protected set; }
        public IGameOverUI GameOverUI { get; protected set; }
        public IGameMenuUI GameMenuUI { get; protected set; }
        public IInventoryPopUpUI InventoryPopUpUI { get; protected set; }
        public ITutorialUI TutorialUI { get; protected set; }
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

        #region For converting text anchors to Vector2s
        static float upper = 1f;
        static float middle = 0.5f;
        static float lower = 0f;

        static float right = 1f;
        static float center = 0.5f;
        static float left = 0f;
        #endregion

        void Awake()
        {
            interfaceFactory = FindObjectOfType<InterfaceFactory>();
            if (interfaceFactory == null)
                gameObject.SetActive(false);
            try
            {
                MainCanvas = gameObject.GetComponent(typeof(Canvas)) as Canvas;
                if (MainCanvas == null)
                    componentMissing = true;

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

                InventoryUI = inventoryUIObject.GetComponent(typeof(IInventoryUI)) as IInventoryUI;
                if (inventoryUIObject == null)
                    componentMissing = true;

                TutorialUI = tutorialUIObject.GetComponent(typeof(ITutorialUI)) as ITutorialUI;
                if (tutorialUIObject == null)
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
                if (componentMissing)
                {
                    errorHandler.ReportError("UI component missing", ErrorState.restart_scene);
                }

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

        public Vector2 ToViewportCoords(TextAnchor textAnchor)
        {

            switch (textAnchor)
            {
                case TextAnchor.UpperLeft:
                    return new Vector2(left, upper);
                case TextAnchor.UpperCenter:
                    return new Vector2(center, upper);
                case TextAnchor.UpperRight:
                    return new Vector2(right, upper);

                case TextAnchor.MiddleLeft:
                    return new Vector2(left, middle);
                case TextAnchor.MiddleCenter:
                    return new Vector2(center, middle);
                case TextAnchor.MiddleRight:
                    return new Vector2(right, middle);

                case TextAnchor.LowerLeft:
                    return new Vector2(left, lower);
                case TextAnchor.LowerCenter:
                    return new Vector2(center, lower);
                case TextAnchor.LowerRight:
                    return new Vector2(right, lower);

                default:
                    throw new System.NotImplementedException("Didn't account for " + textAnchor.ToString());

            }
        }

        public void ApplyAnchorPreset(RectTransform rectTransform,
                                                TextAnchor presetToApply,
                                                bool alsoSetPivot = false,
                                                bool alsoSetPosition = false)
        {

            Vector2 anchorsToSet = presetToApply.ToViewportCoords();
            rectTransform.SetAnchors(anchorsToSet, anchorsToSet);

            if (alsoSetPivot)
                rectTransform.SetPivot(anchorsToSet);

            if (alsoSetPosition)
                rectTransform.PositionRelativeToParent(anchorsToSet);

        }
    }
}
