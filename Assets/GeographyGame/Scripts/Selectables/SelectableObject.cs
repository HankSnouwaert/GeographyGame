using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WPM
{
    abstract public class SelectableObject : MonoBehaviour, ISelectableObject
    {
        //Public Variables
        public string ObjectName { get; set; }
        public bool Selected { get; set; }
        //Interal Interface References
        protected IGameManager gameManager;
        //Flag to determine if the object can be selected
        protected bool selectionEnabled = false;
        //Error Checking
        protected InterfaceFactory interfaceFactory;
        protected IErrorHandler errorHandler;

        protected virtual void Awake()
        {
            interfaceFactory = FindObjectOfType<InterfaceFactory>();
            if (interfaceFactory == null)
                gameObject.SetActive(false);
        }

        protected virtual void Start()
        {
            errorHandler = interfaceFactory.ErrorHandler;
            gameManager = interfaceFactory.GameManager;
            if (gameManager == null || errorHandler == null)
            {
                gameObject.SetActive(false);
            }
            else
                selectionEnabled = true;
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

        abstract public void OnSelectableEnter(ISelectableObject selectableObject);

        abstract public void OtherObjectSelected(ISelectableObject selectedObject);

        abstract public void OnCellEnter(int index);

        abstract public void OnCellClick(int index);
    }
}
