using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WPM
{

    public class ScoreUI : UIElement, IScoreUI, ITurnsUI
    {
        private Text[] textComponents;
        private Text displayText;
        private int currentScore;
        private int currentTurnsRamaining;
        protected override void Awake()
        {
            base.Awake();
            textComponents = uiObject.GetComponentsInChildren<Text>();
            displayText = textComponents[0];
            UIOpen = true;
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
