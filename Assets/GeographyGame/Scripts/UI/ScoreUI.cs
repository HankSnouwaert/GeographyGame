using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WPM
{

    public class ScoreUI : UIElement, IScoreUI, ITurnsUI
    {
        //Private Variables
        private Text[] textComponents;
        private Text displayText;
        private int currentScore;
        private int currentTurnsRamaining;
        //Error Checking
        private bool componentMissing = false;

        protected override void Awake()
        {
            base.Awake();
            if (gameObject.activeSelf)
            {
                textComponents = UIObject.GetComponentsInChildren<Text>();
                if (textComponents == null || textComponents.Length > 1)
                {
                    componentMissing = true;
                }
                else
                {
                    displayText = textComponents[0];
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
            displayText.text = "Score: " + currentScore + System.Environment.NewLine + "Turns Left: " + currentTurnsRamaining;
        }
    }
}
