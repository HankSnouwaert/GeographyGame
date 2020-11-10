using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WPM
{
    public class SelectableObject : MonoBehaviour
    {
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

        public virtual void Selected()
        {
            if (gameManager.selectedObject != null)
                if (gameManager.selectedObject != this)
                    gameManager.selectedObject.Deselected();

            gameManager.selectedObject = this;
            selected = true;
        }

        public virtual void Deselected()
        {
            if (gameManager.selectedObject == this)
                gameManager.selectedObject = null;
            selected = false;
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
