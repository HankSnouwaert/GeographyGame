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
        private IScoreManager scoreManager;
        protected override void Awake()
        {
            base.Awake();
            textComponents = UIObject.GetComponentsInChildren<Text>();
            displayText = textComponents[0];
        }

        protected override void Start()
        {
            base.Start();
            scoreManager = gameManager.ScoreManager;
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