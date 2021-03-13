using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPM
{
    public class MouseCellEnterer : MonoBehaviour, ICellEnterer
    {
        private GameManager gameManager;

        // Start is called before the first frame update
        void Awake()
        {
            gameManager = FindObjectOfType<GameManager>();
            gameManager.worldGlobeMap.OnCellEnter += HandleOnCellEnter;
        }

        /// <summary>
        /// Called whenever a hex cell is moused over
        /// Inputs:
        ///     cellIndex: index of cell entered
        /// </summary>
        public void HandleOnCellEnter(int cellIndex)
        {
            //Run any on cell enter method for the selected object
            if (!gameManager.CursorOverUI && !gameManager.GamePaused && gameManager.SelectedObject != null)
            {
                if (gameManager.HighlightedObject != null)
                    gameManager.SelectedObject.OnSelectableEnter(gameManager.HighlightedObject);

                gameManager.SelectedObject.OnCellEnter(cellIndex);
            }
        }
    }
}
