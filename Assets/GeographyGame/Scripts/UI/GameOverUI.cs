using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WPM
{
    public class GameOverUI : UIElement, IGameOverUI
    {
        private Text[] textComponents;
        private Text displayText;
        public override void Awake()
        {
            base.Awake();
            textComponents = uiObject.GetComponentsInChildren<Text>();
            displayText = textComponents[0];
            CloseUI();
        }

        public override void OpenUI()
        {
            base.OpenUI();
            SetGameOverMessage();
        }

        private void SetGameOverMessage()
        {
            displayText.text = "Time's Up!" + System.Environment.NewLine + "Your Score Was: " + gameManager.score;
        }

        public void ResetGameSelected()
        {
            gameManager.GameReset();
        }

        public void ExitGameSelected()
        {
            gameManager.ExitGame();
        }
    }
}