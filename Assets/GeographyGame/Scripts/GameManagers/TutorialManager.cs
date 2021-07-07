using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPM
{
    public class TutorialManager : MonoBehaviour, ITutorialManager
    {
        //Internal Interface References
        private IGameManager gameManager;
        private IUIManager uiManager;
        //Error Checking
        private InterfaceFactory interfaceFactory;
        private IErrorHandler errorHandler;
        private bool componentMissing;

        private void Awake()
        {
            interfaceFactory = FindObjectOfType<InterfaceFactory>();
            if (interfaceFactory == null)
                gameObject.SetActive(false);
        }

        void Start()
        {
            gameManager = interfaceFactory.GameManager;
            uiManager = interfaceFactory.UIManager;
            gameManager = interfaceFactory.GameManager;
            if (gameManager == null || uiManager == null || errorHandler == null)
                gameObject.SetActive(false);
        }

        public void BeginTutorial()
        {

        }

        public void EndTutorial()
        {

        }
    }
}


