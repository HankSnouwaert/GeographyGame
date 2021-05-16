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
        private bool componentMissing = false;

        protected override void Awake()
        {
            base.Awake();
            textComponents = UIObject.GetComponentsInChildren<Text>();
            if(textComponents == null || textComponents.Length > 1)
            {
                componentMissing = true;
            }
            else
            {
                displayText = textComponents[0];
                UIOpen = true;
            } 
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
