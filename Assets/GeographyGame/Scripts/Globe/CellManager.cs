using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPM
{
    public class CellManager : MonoBehaviour
    {
        public GameManager gameManager;
        public WorldMapGlobe worldGlobeMap;
        public bool ClosingGUIPanel { get; set; } = false;
        public bool CursorOverUI { get; set; } = false;
        public bool NewObjectSelected { get; set; } = false;

        // Start is called before the first frame update
        void Start()
        {
            // Setup grid events
            worldGlobeMap.OnCellEnter += HandleOnCellEnter;
            worldGlobeMap.OnCellExit += HandleOnCellExit;
            worldGlobeMap.OnCellClick += HandleOnCellClick;
        }

        // Update is called once per frame
        void Update()
        {
           
        }

        /// <summary>
        /// Called whenever a hex cell is clicked
        /// Inputs:
        ///     cellIndex: index of cell clicked
        /// </summary>
        void HandleOnCellClick(int cellIndex)
        {
            //Check if a GUI panel is beling closed
            if (ClosingGUIPanel)
            {
                gameManager.CursorOverUI = false;
                ClosingGUIPanel = false;
                HandleOnCellEnter(cellIndex);
            }
            else
            {
                //Check that a hex is being clicked while an object is selected
                if (!CursorOverUI && !gameManager.GamePaused && gameManager.SelectedObject != null)
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
                            //Get Stack Trace
                            string combinedStackTrace = ex.StackTrace;
                            var inner = ex.InnerException;
                            while (inner != null)
                            {
                                combinedStackTrace = combinedStackTrace + inner.StackTrace;
                                inner = inner.InnerException;
                            }

                            if (gameManager.errorState < ErrorState.close_window)
                                gameManager.errorState = ErrorState.close_window;

                            gameManager.DisplayError(ex.Message, combinedStackTrace);
                        }
                }
            }

        }

        /// <summary>
        /// Called whenever a hex cell is moused over
        /// Inputs:
        ///     cellIndex: index of cell entered
        /// </summary>
        void HandleOnCellEnter(int cellIndex)
        {
            //Run any on cell enter method for the selected object
            if (!CursorOverUI && !gameManager.GamePaused && gameManager.SelectedObject != null)
            {
                if (gameManager.HighlightedObject != null)
                    gameManager.SelectedObject.OnSelectableEnter(gameManager.HighlightedObject);
               
                gameManager.SelectedObject.OnCellEnter(cellIndex);
            }
        }

        /// <summary>
        /// Called whenever the curser moves out of a hex
        /// Inputs:
        ///     cellIndex: index of cell clicked (Not currently used)
        /// </summary>
        void HandleOnCellExit(int cellIndex)
        {
            if (!CursorOverUI && !gameManager.GamePaused)
            {
                Province province = worldGlobeMap.provinceHighlighted;
                if (province == null || CursorOverUI)
                    gameManager.hexInfoPanel.SetActive(false);
            }
        }


    }
}
