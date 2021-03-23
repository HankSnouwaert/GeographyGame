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

        public virtual void Awake()
        {
            gameManager = FindObjectOfType<GameManager>();
            player = FindObjectOfType<PlayerCharacter>();
        }

        public virtual void OnMouseDown()
        {
            if (gameManager.SelectedObject == null)
                Selected();
            else
            {
                if (gameManager.SelectedObject == this)
                    Deselected();
                else
                    gameManager.SelectedObject.ObjectSelected(this);
            }   
        }

        public virtual void OnMouseEnter()
        {
            gameManager.HighlightedObject = this;
            gameManager.UpdateHexInfoPanel();
        }

        public virtual void OnMouseExit()
        {
            gameManager.HighlightedObject = null;
            gameManager.UpdateHexInfoPanel(); 
        }

        public virtual void Selected()
        {
            if (gameManager.SelectedObject != null)
                if (gameManager.SelectedObject != this)
                    gameManager.SelectedObject.Deselected();

            gameManager.SelectedObject = this;
            selected = true;
        }

        public virtual void Deselected()
        {
            if (gameManager.SelectedObject == this)
                gameManager.SelectedObject = null;
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
