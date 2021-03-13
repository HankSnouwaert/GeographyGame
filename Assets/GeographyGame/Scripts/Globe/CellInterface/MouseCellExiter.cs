using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    namespace WPM
{
    public class MouseCellExiter : MonoBehaviour, ICellExiter
    {
        private GameManager gameManager;

        void Awake()
        { 
            gameManager = FindObjectOfType<GameManager>();
            gameManager.worldGlobeMap.OnCellExit += HandleOnCellExit;
        }

        /// <summary>
        /// Called whenever the curser moves out of a hex
        /// Inputs:
        ///     cellIndex: index of cell clicked (Not currently used)
        /// </summary>
        public void HandleOnCellExit(int cellIndex)
        {
            if (!gameManager.CursorOverUI && !gameManager.GamePaused)
            {
                Province province = gameManager.worldGlobeMap.provinceHighlighted;
                if (province == null || gameManager.CursorOverUI)
                    gameManager.hexInfoPanel.SetActive(false);
            }

        }
    }
}

