using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WPM
{
    public class GameMenuUI : UIElement, IGameMenuUI
    {
        private Text[] textComponents;
        private Text displayText;

        public override void Start()
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

        // Update is called once per frame
        public void ExitGameSelected()
        {
            gameManager.ExitGame();
        }
    }
}
