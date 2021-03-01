using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WPM
{
    public class SelectableObject : MonoBehaviour
    {
        public string objectName;
        public bool selected;
        protected WorldMapGlobe map;
        protected GameManager gameManager;
        protected PlayerCharacter player;

        // Start is called before the first frame update

        public virtual void Start()
        {
            gameManager = FindObjectOfType<GameManager>();
            player = FindObjectOfType<PlayerCharacter>();
        }

        public virtual void OnMouseDown()
        {
            if (gameManager.selectedObject == null)
                Selected();
            else
            {
                if (gameManager.selectedObject == this)
                    Deselected();
                else
                    gameManager.selectedObject.ObjectSelected(this);
            }   
        }

        public virtual void OnMouseEnter()
        {
            gameManager.SetHighlightedObject(this);
            gameManager.UpdateHexInfoPanel();
        }

        public virtual void OnMouseExit()
        {
            gameManager.SetHighlightedObject(null);
            gameManager.UpdateHexInfoPanel();
        }

        public virtual void Selected()
        {
            if (gameManager.selectedObject != null)
                if (gameManager.selectedObject != this)
                    gameManager.selectedObject.Deselected();

            gameManager.selectedObject = this;
            gameManager.newObjectSelected = true;
            selected = true;
        }

        public virtual void Deselected()
        {
            if (gameManager.selectedObject == this)
                gameManager.selectedObject = null;
            selected = false;
        }

        public virtual void OnSelectableEnter(SelectableObject selectedObject)
        {

        }

        public virtual void ObjectSelected(SelectableObject selectedObject)
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

        public virtual void EndOfTurn(int turns)
        {

        }

    }
}
