using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace WPM
{
    public class StartMenu : MonoBehaviour
    {
        [SerializeField]
        private GameObject gameSettingsObject;

        private IGameSettings gameSettings;

        //Error Checking
        private IErrorHandler errorHandler;
        private InterfaceFactoryStartScene interfaceFactory;
        private bool componentMissing = false;

        private void Awake()
        {
            interfaceFactory = FindObjectOfType<InterfaceFactoryStartScene>();
            if (interfaceFactory == null)
                gameObject.SetActive(false);
            try
            {
                gameSettings = gameSettingsObject.GetComponent(typeof(IGameSettings)) as IGameSettings;
                if (gameSettings == null)
                    componentMissing = true;
            }
            catch
            {
                componentMissing = true;
            }
        }
        
        public void PlayGameSelected()
        {
            SceneManager.LoadScene("StudentGame", LoadSceneMode.Single);
        }

        public void TutorialSelected()
        {
            gameSettings.TutorialActive = true;
            SceneManager.LoadScene("StudentGame", LoadSceneMode.Single);
        }
    }
}


