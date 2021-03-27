using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace WPM
{
    public class UIManager : MonoBehaviour, IUIManager
    {
        [SerializeField] private GameObject errorUIObject;
        public IErrorUI ErrorUI { get; set; }
        [SerializeField] private GameObject mouseOverInfoUIObject;
        public IMouseOverInfoUI MouseOverInfoUI { get; set; }
        [SerializeField] private GameObject inventoryUIObject;

        public bool CursorOverUI { get; set; }
        public bool ClosingUI { get; set; } = false;
        private GameManager gameManager;
        private ICellManager cellManager;
        


        // Start is called before the first frame update
        void Awake()
        {
            ErrorUI = errorUIObject.GetComponent(typeof(IErrorUI)) as IErrorUI;
            MouseOverInfoUI = mouseOverInfoUIObject.GetComponent(typeof(IMouseOverInfoUI)) as IMouseOverInfoUI;
            gameManager = FindObjectOfType<GameManager>();
            cellManager = gameManager.cellManagerObject.GetComponent(typeof(ICellManager)) as ICellManager;
        }

        void Update()
        {
            CursorOverUI = CheckForMouseOverUI();
            if (ClosingUI)
            {
                ClosingUI = false;
                int currentCellIndex = cellManager.highlightedCellIndex;
                if(currentCellIndex != -1)
                    cellManager.CellEnterer.HandleOnCellEnter(currentCellIndex);
                CursorOverUI = true;
            }
            MouseOverInfoUI.UpdateUI();
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
            errorUIObject.SetActive(false);
            mouseOverInfoUIObject.SetActive(false);
            inventoryUIObject.SetActive(false);
        }

    }
}
