using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace WPM
{
    public class UIManager : MonoBehaviour, IUIManager
    {
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

        public IMouseOverInfoUI MouseOverInfoUI { get; set; }
        public INavigationUI NavigationUI { get; set; }
        public IDropOffUI DropOffUI { get; set; }
        public IScoreUI ScoreUI { get; set; }
        public ITurnsUI TurnsUI { get; set; }
        public IGameOverUI GameOverUI { get; set; }
        public IGameMenuUI GameMenuUI { get; set; }
        public IInventoryPopUpUI InventoryPopUpUI { get; set; }

        public bool CursorOverUI { get; set; }
        public bool ClosingUI { get; set; } = false;

        private GameManager gameManager;
        private GlobeManager globeManager;
        private ICellCursorInterface cellCursorInterface;


        // Start is called before the first frame update
        void Awake()
        {
            //ErrorUI = errorUIObject.GetComponent(typeof(IErrorUI)) as IErrorUI;
            MouseOverInfoUI = mouseOverInfoUIObject.GetComponent(typeof(IMouseOverInfoUI)) as IMouseOverInfoUI;
            NavigationUI = navigationUIObject.GetComponent(typeof(INavigationUI)) as INavigationUI;
            DropOffUI = dropOffUIObject.GetComponent(typeof(IDropOffUI)) as IDropOffUI;
            ScoreUI = scoreUIObject.GetComponent(typeof(IScoreUI)) as IScoreUI;
            TurnsUI = turnsUIObject.GetComponent(typeof(ITurnsUI)) as ITurnsUI;
            GameOverUI = gameOverUIObject.GetComponent(typeof(IGameOverUI)) as IGameOverUI;
            GameMenuUI = gameMenuUIObject.GetComponent(typeof(IGameMenuUI)) as IGameMenuUI;
            InventoryPopUpUI = inventoryPopUpUIObject.GetComponent(typeof(IInventoryPopUpUI)) as IInventoryPopUpUI;
            gameManager = FindObjectOfType<GameManager>();
            globeManager = FindObjectOfType<GlobeManager>(); 
        }

        void Start()
        {
            cellCursorInterface = globeManager.CellCursorInterface;
        }

        void Update()
        {
            CursorOverUI = CheckForMouseOverUI();
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

        public bool CheckForMouseOverUI()
        {
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = Input.mousePosition;

            List<RaycastResult> raycastResultList = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, raycastResultList);
            for (int i = 0; i < raycastResultList.Count; i++)
            {
                if (raycastResultList[i].gameObject.GetComponent<IUIElement>() == null)
                {
                    raycastResultList.RemoveAt(i);
                    i--;
                }
            }
            return raycastResultList.Count > 0;
        }

        public void GameOver()
        {
            mouseOverInfoUIObject.SetActive(false);
            inventoryUIObject.SetActive(false);
            inventoryPopUpUIObject.SetActive(false);
            GameOverUI.OpenUI();
        }

        public void ExitCurrentUI()
        {
            if (gameMenuUIObject.activeSelf)
            {
                GameMenuUI.RestartGameSelected();
            }
        }
    }
}
