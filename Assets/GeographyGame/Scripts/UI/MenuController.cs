using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;

namespace SpeedTutorMainMenuSystem
{
    public class MenuController : MonoBehaviour
    {
        #region Map Properties
        [Header("Map Properties")]
        [SerializeField] private Text saveFile;
        [SerializeField] private Toggle climate;
        [SerializeField] private Toggle terrain;
        [SerializeField] private Toggle physicalLandmarks;
        [SerializeField] private Toggle culturalLandmarks;
        [SerializeField] private Toggle provinces;
        #endregion

        #region Default Values
        [Header("Default Menu Values")]
        [SerializeField] private float defaultBrightness;
        [SerializeField] private float defaultVolume;
        [SerializeField] private int defaultSen;
        [SerializeField] private bool defaultInvertY;

        [Header("Levels To Load")]
        public string _newGameButtonLevel;
        private string levelToLoad;

        const int MAIN_MENU = 1;
        const int OPTIONS = 2;
        const int GRAPHICS = 3;
        const int SOUND = 4;
        const int GAMEPLAY = 5;
        const int CONTROLS = 6;
        const int STARTGAME = 7;
        const int LOADMAP = 8;
        const int NEWMAP = 9;
        const int CONNECT = 10;
        const int STARTLAN = 11;

        private int menuNumber;

        #endregion

        #region Menu Dialogs
        [Header("Main Menu Components")]
        [SerializeField] private GameObject menuDefaultCanvas;
        [SerializeField] private GameObject GeneralSettingsCanvas;
        [SerializeField] private GameObject graphicsMenu;
        [SerializeField] private GameObject soundMenu;
        [SerializeField] private GameObject gameplayMenu;
        [SerializeField] private GameObject controlsMenu;
        [SerializeField] private GameObject confirmationMenu;
        [Space(10)]
        [Header("Menu Popout Dialogs")]
        [SerializeField] private GameObject noSaveDialog;
        [SerializeField] private GameObject newGameDialog;
        [SerializeField] private GameObject loadGameDialog;
        [SerializeField] private GameObject lanDialog;
        [SerializeField] private GameObject networkManager;
        #endregion

        #region Slider Linking
        [Header("Menu Sliders")]
        [SerializeField] private Text controllerSenText;
        [SerializeField] private Slider controllerSenSlider;
        public float controlSenFloat = 2f;
        [Space(10)]
        [SerializeField] private Brightness brightnessEffect;
        [SerializeField] private Slider brightnessSlider;
        [SerializeField] private Text brightnessText;
        [Space(10)]
        [SerializeField] private Text volumeText;
        [SerializeField] private Slider volumeSlider;
        [Space(10)]
        [SerializeField] private Toggle invertYToggle;
        #endregion

        #region Initialisation - Button Selection & Menu Order
        private void Start()
        {
            menuNumber = 1;
        }
        #endregion

        //MAIN SECTION
        public IEnumerator ConfirmationBox()
        {
            confirmationMenu.SetActive(true);
            yield return new WaitForSeconds(2);
            confirmationMenu.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (menuNumber == OPTIONS || menuNumber == STARTGAME || menuNumber == LOADMAP)
                {
                    GoBackToMainMenu();
                    ClickSound();
                }

                else if (menuNumber == GRAPHICS || menuNumber == SOUND || menuNumber == GAMEPLAY)
                {
                    GoBackToOptionsMenu();
                    ClickSound();
                }

                else if (menuNumber == CONTROLS) //CONTROLS MENU
                {
                    GoBackToGameplayMenu();
                    ClickSound();
                }
            }
        }

        private void ClickSound()
        {
            GetComponent<AudioSource>().Play();
        }

        #region Menu Mouse Clicks
        public void MouseClick(string buttonType)
        {
            if (buttonType == "Controls")
            {
                gameplayMenu.SetActive(false);
                controlsMenu.SetActive(true);
                menuNumber = CONTROLS;
            }

            if (buttonType == "Graphics")
            {
                GeneralSettingsCanvas.SetActive(false);
                graphicsMenu.SetActive(true);
                menuNumber = GRAPHICS;
            }

            if (buttonType == "Sound")
            {
                GeneralSettingsCanvas.SetActive(false);
                soundMenu.SetActive(true);
                menuNumber = SOUND;
            }

            if (buttonType == "Gameplay")
            {
                GeneralSettingsCanvas.SetActive(false);
                gameplayMenu.SetActive(true);
                menuNumber = GAMEPLAY;
            }

            if (buttonType == "Exit")
            {
                Debug.Log("YES QUIT!");
                Application.Quit();
            }

            if (buttonType == "Options")
            {
                menuDefaultCanvas.SetActive(false);
                GeneralSettingsCanvas.SetActive(true);
                menuNumber = OPTIONS;
            }

            if (buttonType == "LoadMap")
            {
                menuDefaultCanvas.SetActive(false);
                loadGameDialog.SetActive(true);
                menuNumber = LOADMAP;
            }

            if (buttonType == "StartGame")
            {
                menuDefaultCanvas.SetActive(false);
                newGameDialog.SetActive(true);
                menuNumber = STARTGAME;
            }

            if (buttonType == "NewMap")
            {
                menuDefaultCanvas.SetActive(false);
                newGameDialog.SetActive(true);
                menuNumber = NEWMAP;
            }

            if (buttonType == "StartLAN")
            {
                menuDefaultCanvas.SetActive(false);
                lanDialog.SetActive(true);
                networkManager.SetActive(true);
                menuNumber = STARTLAN;
            }

            if (buttonType == "Connect")
            {
                menuDefaultCanvas.SetActive(false);
                lanDialog.SetActive(true);
                networkManager.SetActive(true);
                menuNumber = CONNECT;
            }
        }
        #endregion

        public void VolumeSlider(float volume)
        {
            AudioListener.volume = volume;
            volumeText.text = volume.ToString("0.0");
        }

        public void VolumeApply()
        {
            PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);
            Debug.Log(PlayerPrefs.GetFloat("masterVolume"));
            StartCoroutine(ConfirmationBox());
        }

        public void BrightnessSlider(float brightness)
        {
            brightnessEffect.brightness = brightness;
            brightnessText.text = brightness.ToString("0.0");
        }

        public void BrightnessApply()
        {
            PlayerPrefs.SetFloat("masterBrightness", brightnessEffect.brightness);
            Debug.Log(PlayerPrefs.GetFloat("masterBrightness"));
            StartCoroutine(ConfirmationBox());
        }

        public void ControllerSen()
        {
            controllerSenText.text = controllerSenSlider.value.ToString("0");
            controlSenFloat = controllerSenSlider.value;
        }

        public void GameplayApply()
        {
            if (invertYToggle.isOn) //Invert Y ON
            {
                PlayerPrefs.SetInt("masterInvertY", 1);
                Debug.Log("Invert" + " " + PlayerPrefs.GetInt("masterInvertY"));
            }

            else if (!invertYToggle.isOn) //Invert Y OFF
            {
                PlayerPrefs.SetInt("masterInvertY", 0);
                Debug.Log(PlayerPrefs.GetInt("masterInvertY"));
            }

            PlayerPrefs.SetFloat("masterSen", controlSenFloat);
            Debug.Log("Sensitivity" + " " + PlayerPrefs.GetFloat("masterSen"));

            StartCoroutine(ConfirmationBox());
        }

      

        #region ResetButton
        public void ResetButton(string GraphicsMenu)
        {
            if (GraphicsMenu == "Brightness")
            {
                brightnessEffect.brightness = defaultBrightness;
                brightnessSlider.value = defaultBrightness;
                brightnessText.text = defaultBrightness.ToString("0.0");
                BrightnessApply();
            }

            if (GraphicsMenu == "Audio")
            {
                AudioListener.volume = defaultVolume;
                volumeSlider.value = defaultVolume;
                volumeText.text = defaultVolume.ToString("0.0");
                VolumeApply();
            }

            if (GraphicsMenu == "Graphics")
            {
                controllerSenText.text = defaultSen.ToString("0");
                controllerSenSlider.value = defaultSen;
                controlSenFloat = defaultSen;

                invertYToggle.isOn = false;

                GameplayApply();
            }
        }
        #endregion

        #region Dialog Options - This is where we load what has been saved in player prefs!
        public void ClickNewGameDialog(string ButtonType)
        {
            if (ButtonType == "Save")
            {
                SaveObject saveObject = new SaveObject
                {
                    climate = climate.isOn,
                    terrain = terrain.isOn,
                    culturalLandmarks = culturalLandmarks.isOn,
                    physicalLandmarks = physicalLandmarks.isOn,
                    provinces = provinces.isOn
                };
                string json = JsonUtility.ToJson(saveObject);
                File.WriteAllText(Application.dataPath + "/" + saveFile.text + ".txt", json);
            }

            if (ButtonType == "Back")
            {
                GoBackToMainMenu();
            }
        }

        public void ClickSendSettingsDialog(string ButtonType)
        {
            if (ButtonType == "Send")
            {
               
            }

            if (ButtonType == "Back")
            {
                GoBackToMainMenu();
            }
        }

        public void ClickLoadGameDialog(string ButtonType)
        {
            if (ButtonType == "Yes")
            {
                if (PlayerPrefs.HasKey("SavedLevel"))
                {
                    Debug.Log("I WANT TO LOAD THE SAVED GAME");
                    //LOAD LAST SAVED SCENE
                    levelToLoad = PlayerPrefs.GetString("SavedLevel");
                    SceneManager.LoadScene(levelToLoad);
                }

                else
                {
                    Debug.Log("Load Game Dialog");
                    menuDefaultCanvas.SetActive(false);
                    loadGameDialog.SetActive(false);
                    noSaveDialog.SetActive(true);
                }
            }

            if (ButtonType == "No")
            {
                GoBackToMainMenu();
            }
        }
        #endregion

        #region Back to Menus
        public void GoBackToOptionsMenu()
        {
            GeneralSettingsCanvas.SetActive(true);
            graphicsMenu.SetActive(false);
            soundMenu.SetActive(false);
            gameplayMenu.SetActive(false);

            GameplayApply();
            BrightnessApply();
            VolumeApply();

            menuNumber = OPTIONS;
        }

        public void GoBackToMainMenu()
        {
            menuDefaultCanvas.SetActive(true);
            newGameDialog.SetActive(false);
            loadGameDialog.SetActive(false);
            noSaveDialog.SetActive(false);
            lanDialog.SetActive(false);
            networkManager.SetActive(false);
            GeneralSettingsCanvas.SetActive(false);
            graphicsMenu.SetActive(false);
            soundMenu.SetActive(false);
            gameplayMenu.SetActive(false);
            menuNumber = MAIN_MENU;
        }

        public void GoBackToGameplayMenu()
        {
            controlsMenu.SetActive(false);
            gameplayMenu.SetActive(true);
            menuNumber = GAMEPLAY;
        }

        public void ClickQuitOptions()
        {
            GoBackToMainMenu();
        }

        public void ClickNoSaveDialog()
        {
            GoBackToMainMenu();
        }
        #endregion
    }
    
    public class SaveObject
    {
        public bool climate;
        public bool terrain;
        public bool physicalLandmarks;
        public bool culturalLandmarks;
        public bool provinces;
    }
}
