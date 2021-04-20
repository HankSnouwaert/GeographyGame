using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPM
{
    public class ScoreManager : MonoBehaviour, IScoreManager
    {
        public int Score { get; protected set; } = 0;
        private IUIManager uiManager;
        void Start()
        {
            uiManager = FindObjectOfType<InterfaceFactory>().UIManager;
        }

        /// <summary>
        ///  Update the games current score
        /// </summary>
        /// <param name="scoreModification"></param> The amount the score should be changed by>
        /// <returns></returns> 
        public void UpdateScore(int scoreModification)
        {
            Score = Score + scoreModification;
            uiManager.ScoreUI.UpdateDisplayedScore(Score);
        }
    }
}
