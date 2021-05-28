using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WPM
{

    public class ScoreUI : UIElement, IScoreUI, ITurnsUI
    {
        //Displayed Text
        [SerializeField]
        private Text scoreText;
        [SerializeField]
        private Text turnsText;
        //Private Variables
        private int currentScore;
        private int currentTurnsRamaining;
        //Error Checking
        private bool componentMissing = false;

        protected override void Awake()
        {
            base.Awake();
            if (gameObject.activeSelf)
            {
                if (scoreText == null || turnsText == null)
                {
                    componentMissing = true;
                }
                else
                {
                    UIOpen = true;
                }
            }
        }

        protected override void Start()
        {
            base.Start();
            if (componentMissing)
                errorHandler.ReportError("Error retrieving Score UI components", ErrorState.restart_scene);
        }

        public void UpdateDisplayedScore(int newScore)
        {
            currentScore = newScore;
            UpdateDisplayedText();
        }

        public void UpdateDisplayedRemainingTurns(int newTurnsRemaining)
        {
            currentTurnsRamaining = newTurnsRemaining;
            UpdateDisplayedText();
        }

        private void UpdateDisplayedText()
        {
            scoreText.text = currentScore.ToString(); 
            turnsText.text = currentTurnsRamaining.ToString();
        }
    }
}
