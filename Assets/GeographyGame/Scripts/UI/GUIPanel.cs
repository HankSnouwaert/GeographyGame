using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace WPM
{

    public class GUIPanel : MonoBehaviour//, IPointerEnterHandler, IPointerExitHandler
    {
        protected GameManager gameManager;
        protected MouseCellClicker mouseCellClicker;
        protected GameObject panel;

        // Start is called before the first frame update
        public virtual void Start()
        {
            gameManager = FindObjectOfType<GameManager>();
            mouseCellClicker = FindObjectOfType<MouseCellClicker>();
            panel = gameObject;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OpenPanel()
        {
            panel.SetActive(true);
        }

        public void ClosePanel()
        {
            panel.SetActive(false);
            mouseCellClicker.ClosingGUIPanel = true;
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