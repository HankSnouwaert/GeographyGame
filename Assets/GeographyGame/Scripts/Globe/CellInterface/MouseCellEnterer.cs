using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPM
{
    /// <summary>
    ///  Used to handle instances of the mouse moving over a cell on the world globe map
    /// </summary>
    public class MouseCellEnterer : MonoBehaviour, ICellEnterer
    {
        private GameManager gameManager;
        private IUIManager uiManager;
        private ICellManager cellManager;
        void Awake()
        {
            gameManager = FindObjectOfType<GameManager>();
            gameManager.worldGlobeMap.OnCellEnter += HandleOnCellEnter;
            uiManager = gameManager.uiManagerObject.GetComponent(typeof(IUIManager)) as IUIManager;
            cellManager = GetComponent(typeof(ICellManager)) as ICellManager;
        }

        /// <summary>
        ///  Called when the mouse cursor moves over a cell on the world globe map
        /// </summary>
        /// <param name="cellIndex"></param> The cell the mouse cursor moves over>
        /// <returns></returns> 
        public void HandleOnCellEnter(int cellIndex)
        {
            //Run any on cell enter method for the selected object
            if (!uiManager.CursorOverUI && !gameManager.GamePaused && gameManager.SelectedObject != null)
            {
                if (gameManager.HighlightedObject != null)
                    gameManager.SelectedObject.OnSelectableEnter(gameManager.HighlightedObject);

                gameManager.SelectedObject.OnCellEnter(cellIndex);
                cellManager.highlightedCellIndex = cellIndex;
                cellManager.highlightedCell = gameManager.worldGlobeMap.cells[cellIndex];
            }
        }
    }
}
