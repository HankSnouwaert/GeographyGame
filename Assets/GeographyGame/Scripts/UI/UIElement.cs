using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace WPM
{

    abstract public class UIElement : MonoBehaviour, IUIElement
    {
        //Public Variables
        public GameObject UIObject { get; protected set; }
        public bool UIOpen { get; set; }
        //Private Interface References
        protected IGameManager gameManager;
        protected IGlobeManager globeManager;
        protected IUIManager uiManager;
        protected ICellClicker mouseCellClicker;
        protected ICellCursorInterface cellCursorInterface;
        //Error Checking
        protected InterfaceFactory interfaceFactory;
        protected IErrorHandler errorHandler;

        protected virtual void Awake()
        {
            interfaceFactory = FindObjectOfType<InterfaceFactory>();
            if (interfaceFactory == null)
                gameObject.SetActive(false);
            else
                UIObject = gameObject;
        }

        protected virtual void Start()
        {
            gameManager = interfaceFactory.GameManager;
            globeManager = interfaceFactory.GlobeManager;
            errorHandler = interfaceFactory.ErrorHandler;
            uiManager = interfaceFactory.UIManager;
            if (gameManager == null || globeManager == null || errorHandler == null || uiManager == null)
                gameObject.SetActive(false);
            else
            {
                cellCursorInterface = globeManager.CellCursorInterface;
                if(cellCursorInterface == null)
                    errorHandler.ReportError("Cell Cursor Interface Missing", ErrorState.restart_scene);
                else
                {
                    mouseCellClicker = cellCursorInterface.CellClicker;
                    if (mouseCellClicker == null)
                        errorHandler.ReportError("Mouse Cell Clicker Missing", ErrorState.restart_scene);
                }
            } 
        }

        public virtual void OpenUI()
        {
            UIObject.SetActive(true);
            UIOpen = true;
        }

        public virtual void CloseUI()
        {
            UIObject.SetActive(false);
            uiManager.ClosingUI = true;
            UIOpen = false;
        }
    }
}