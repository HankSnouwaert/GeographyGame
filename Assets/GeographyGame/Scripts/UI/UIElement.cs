using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace WPM
{

    abstract public class UIElement : MonoBehaviour, IUIElement
    {
        protected GameManager gameManager;
        protected GlobeManager globeManager;
        protected IErrorHandler errorHandler;
        protected IUIManager uiManager;
        protected ICellClicker mouseCellClicker;
        protected GameObject uiObject;
        public bool UIOpen { get; set; }

        // Start is called before the first frame update
        protected virtual void Awake()
        {
            gameManager = FindObjectOfType<GameManager>();
            globeManager = FindObjectOfType<GlobeManager>();
            uiObject = gameObject;
        }

        protected virtual void Start()
        {
            mouseCellClicker = globeManager.CellCursorInterface.CellClicker;
            uiManager = FindObjectOfType<InterfaceFactory>().UIManager;
        }

        public virtual void OpenUI()
        {
            uiObject.SetActive(true);
            UIOpen = true;
        }

        public virtual void CloseUI()
        {
            uiObject.SetActive(false);
            uiManager.ClosingUI = true;
            UIOpen = false;
        }
    }
}