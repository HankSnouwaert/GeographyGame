using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WPM
{
    public class GameMenuUI : UIElement, IGameMenuUI
    {
        protected override void Start()
        {
            base.Start();
            CloseUI();
        }

        public void ReturnToGameSelected()
        {
            CloseUI();
            gameManager.ResumeGame();
        }

        public void RestartGameSelected()
        {
            gameManager.GameReset();
        }

        public void ExitGameSelected()
        {
            gameManager.ExitGame();
        }
    }
}
