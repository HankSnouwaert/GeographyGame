using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace WPM
{

    public class UIElement : MonoBehaviour, IUIElement
    {
        protected GameManager gameManager;
        protected ICellClicker mouseCellClicker;
        protected GameObject uiObject;

        // Start is called before the first frame update
        public virtual void Awake()
        {
            gameManager = FindObjectOfType<GameManager>();
            mouseCellClicker = gameManager.cellManagerObject.GetComponent(typeof(ICellClicker)) as ICellClicker;
            uiObject = gameObject;
        }

        public void OpenUI()
        {
            uiObject.SetActive(true);
        }

        public virtual void CloseUI()
        {
            uiObject.SetActive(false);
            mouseCellClicker.ClosingUIPanel = true;
        }
        
        /*
        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            gameManager.CursorOverUI = true;
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            gameManager.CursorOverUI = false;

        }
        */
    }
}