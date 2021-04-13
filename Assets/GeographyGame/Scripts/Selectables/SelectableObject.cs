using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WPM
{
    public class SelectableObject : MonoBehaviour, ISelectableObject
    {
        public string ObjectName { get; set; }
        public bool Selected { get; set; }
        protected WorldMapGlobe map;
        protected GameManager gameManager;
        protected IErrorHandler errorHandler;
        protected IUIManager uiManager;
        protected PlayerCharacter player;

        // Start is called before the first frame update

        public virtual void Awake()
        {
            gameManager = FindObjectOfType<GameManager>();
            player = FindObjectOfType<PlayerCharacter>();
        }

        public virtual void Start()
        {
            uiManager = FindObjectOfType<InterfaceFactory>().UIManager;
            errorHandler = FindObjectOfType<InterfaceFactory>().ErrorHandler; 
        }

        public virtual void OnMouseDown()
        {
            if (!uiManager.CursorOverUI)
            {
                if (gameManager.SelectedObject == null)
                    Select();
                else
                {
                    if (gameManager.SelectedObject == (ISelectableObject)this)
                        Deselect();
                    else
                        gameManager.SelectedObject.ObjectSelected(this);
                }
            }
        }

        public virtual void OnMouseEnter()
        {
            if (!uiManager.CursorOverUI)
            {
                gameManager.HighlightedObject = this;
                //uiManager.MouseOverInfoUI.UpdateHexInfoPanel();
            }
        }

        public virtual void OnMouseExit()
        {
            if (!uiManager.CursorOverUI)
            {
                gameManager.HighlightedObject = null;
                //gameManager.UpdateHexInfoPanel();
            }
        }

        public virtual void Select()
        {
            if (gameManager.SelectedObject != null)
                if (gameManager.SelectedObject != (ISelectableObject)this)
                    gameManager.SelectedObject.Deselect();

            gameManager.SelectedObject = this;
            Selected = true;
        }

        public virtual void Deselect()
        {
            if (gameManager.SelectedObject == (ISelectableObject)this)
                gameManager.SelectedObject = null;
            Selected = false;
        }

        public virtual void OnSelectableEnter(ISelectableObject selectedObject)
        {

        }

        public virtual void ObjectSelected(ISelectableObject selectedObject)
        {
            
        }

        public virtual void MouseEnter()
        {

        }

        public virtual void OnCellEnter(int index)
        {

        }

        public virtual void OnCellClick(int index)
        {

        }
    }
}
