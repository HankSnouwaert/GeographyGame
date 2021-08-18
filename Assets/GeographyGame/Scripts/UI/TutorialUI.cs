using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WPM
{
    public class TutorialUI : UIElement, ITutorialUI
    {
        [SerializeField]
        private Button button1;
        [SerializeField]
        private Text button1Text;
        [SerializeField]
        private Button button2;
        [SerializeField]
        private Text button2Text;
        [SerializeField]
        private Text mainText;
        //Components
        RectTransform rectTransform;
        //Error Checking
        private bool componentsMissing = false;
        private GameSettings gameSettings;

        protected override void Awake()
        {
            base.Awake();
            rectTransform = gameObject.GetComponent(typeof(RectTransform)) as RectTransform;
            if (button1 == null || button2 == null || button1Text == null || button2Text == null || mainText == null || rectTransform == null)
                componentsMissing = true;
        }

        protected override void Start()
        {
            base.Start();
            gameSettings = FindObjectOfType<GameSettings>();
            if (gameSettings == null)
            {
                errorHandler.ReportError("Game Settings missing", ErrorState.restart_scene);
                return;
            }
            if (componentsMissing)
                errorHandler.ReportError("Inventory UI components missing", ErrorState.restart_scene);
            if (!gameSettings.TutorialActive)
                gameObject.SetActive(false);
        }

        public void EnableButton1(bool enabled)
        {
            button1.gameObject.SetActive(enabled);
        }

        public void EnableButton2(bool enabled)
        {
            button2.gameObject.SetActive(enabled);
        }

        public void SetMainText(string textString)
        {
            mainText.text = textString;
        }

        public void SetButton1Text(string textString)
        {
            button1Text.text = textString;
        }

        public void SetButton2Text(string textString)
        {
            button2Text.text = textString;
        }

        public void SetButton1Delegate(DropOffDelegate buttonDelegate)
        {
            if (buttonDelegate == null)
            {
                errorHandler.ReportError("Drop Off Delegate Null", ErrorState.close_window);
                return;
            }
            button1.onClick.AddListener(delegate { buttonDelegate(); });
        }

        public void SetButton2Delegate(DropOffDelegate buttonDelegate)
        {
            if (buttonDelegate == null)
            {
                errorHandler.ReportError("Drop Off Delegate Null", ErrorState.close_window);
                return;
            }
            button2.onClick.AddListener(delegate { buttonDelegate(); });
        }

        public void SetUIPosition()
        {
            uiManager.ApplyAnchorPreset(rectTransform, TextAnchor.UpperRight, false, true);
            //rectTransform.pivot = new Vector2(uiManager.MainCanvas.GetComponent<RectTransform>().rect.xMax, uiManager.MainCanvas.GetComponent<RectTransform>().rect.yMax);
            //rectTransform.anchoredPosition = new Vector2(uiManager.MainCanvas.GetComponent<RectTransform>().rect.xMax, uiManager.MainCanvas.GetComponent<RectTransform>().rect.yMax);
        }
    }
}

