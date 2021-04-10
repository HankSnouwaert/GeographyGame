﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace WPM
{

    public class UIElement : MonoBehaviour, IUIElement
    {
        protected GameManager gameManager;
        protected GlobeManager globeManager;
        protected IUIManager uiManager;
        protected ICellClicker mouseCellClicker;
        protected GameObject uiObject;

        // Start is called before the first frame update
        public virtual void Awake()
        {
            gameManager = FindObjectOfType<GameManager>();
            globeManager = FindObjectOfType<GlobeManager>();
            uiManager = gameManager.uiManagerObject.GetComponent(typeof(IUIManager)) as IUIManager;
            uiObject = gameObject;
        }

        public virtual void Start()
        {
            mouseCellClicker = globeManager.CellCursorInterface.CellClicker;
        }

        public virtual void OpenUI()
        {
            uiObject.SetActive(true);
        }

        public virtual void CloseUI()
        {
            uiObject.SetActive(false);
            uiManager.ClosingUI = true;
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