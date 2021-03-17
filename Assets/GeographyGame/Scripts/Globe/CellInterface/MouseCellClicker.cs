using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPM
{
    public class MouseCellClicker : MonoBehaviour, ICellClicker
    {
        private GameManager gameManager;
        public ICellEnterer cellEnterer;
        public bool ClosingGUIPanel { get; set; } = false;
        public bool NewObjectSelected { get; set; } = false;

        // Start is called before the first frame update
        void Awake()
        {
            // Setup grid events
            gameManager = FindObjectOfType<GameManager>();
            cellEnterer = gameManager.cellManager.GetComponent<ICellEnterer>();
            gameManager.worldGlobeMap.OnCellClick += HandleOnCellClick;
        }

        /// <summary>
        /// Called whenever a hex cell is clicked
        /// Inputs:
        ///     cellIndex: index of cell clicked
        /// </summary>
        public void HandleOnCellClick(int cellIndex)
        {
            //Check if a GUI panel is being closed
            if (ClosingGUIPanel)
            {
                gameManager.CursorOverUI = false;
                ClosingGUIPanel = false;
                cellEnterer.HandleOnCellEnter(cellIndex);
            }
            else
            {
                //Check that a hex is being clicked while an object is selected
                if (!gameManager.CursorOverUI && !gameManager.GamePaused && gameManager.SelectedObject != null)
                {
                    //Make sure your not clicking on a new object
                    if (NewObjectSelected)
                        NewObjectSelected = false;
                    else
                        try
                        {
                            gameManager.SelectedObject.OnCellClick(cellIndex);
                        }
                        catch (System.Exception ex)
                        {
                            gameManager.errorHandler.catchException(ex);
                        }
                }
            }

        }    
    }
}
