using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WPM
{
    public class GameOverUI : UIElement, IGameOverUI
    {
        //Private Variables
        private Text[] textComponents;
        private Text displayText;
        private IScoreManager scoreManager;
        //Error Checking
        private bool componentMissing = false;

        protected override void Awake()
        {
            base.Awake();
            textComponents = UIObject.GetComponentsInChildren<Text>();
            if (textComponents == null)
            {
                componentMissing = true;
            }
            else
            {
                displayText = textComponents[0];
            }
        }

        protected override void Start()
        {
            base.Start();
            if (gameObject.activeSelf)
            {
                if (componentMissing)
                    errorHandler.ReportError("Error getting Inventory Game Over UI Elements", ErrorState.close_window);

                scoreManager = gameManager.ScoreManager;
                if (scoreManager == null)
                    errorHandler.ReportError("Score Manager missing", ErrorState.restart_scene);
            }
            CloseUI();
        }

        public override void OpenUI()
        {
            base.OpenUI();
            SetGameOverMessage();
        }

        private void SetGameOverMessage()
        {
            displayText.text = "Time's Up!" + System.Environment.NewLine + "Your Score Was: " + scoreManager.Score;
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