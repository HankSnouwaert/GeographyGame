using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPM
{
    public class ScoreManager : MonoBehaviour, IScoreManager
    {
        //Public Variables
        public int Score { get; protected set; } = 0;
        //Local Interface References
        private IUIManager uiManager;
        private IScoreUI scoreUI;
        //Error Checking
        private InterfaceFactory interfaceFactory;
        private IErrorHandler errorHandler;

        void Awake()
        {
            interfaceFactory = FindObjectOfType<InterfaceFactory>();
            if (interfaceFactory == null)
                gameObject.SetActive(false);
        }

        void Start()
        {
            uiManager = interfaceFactory.UIManager;
            errorHandler = interfaceFactory.ErrorHandler;
            if (errorHandler == null || uiManager == null)
            {
                gameObject.SetActive(false);
            }
            else
            {
                scoreUI = uiManager.ScoreUI;
                if (scoreUI == null)
                    errorHandler.ReportError("Score UI Missing", ErrorState.close_window);
                else
                    scoreUI.UpdateDisplayedScore(Score);
            }
        }

        /// <summary>
        ///  Update the games current score
        /// </summary>
        /// <param name="scoreModification"></param> The amount the score should be changed by>
        /// <returns></returns> 
        public void UpdateScore(int scoreModification)
        {
            Score = Score + scoreModification;
            if(scoreUI != null)
                scoreUI.UpdateDisplayedScore(Score);
        }
    }
}
