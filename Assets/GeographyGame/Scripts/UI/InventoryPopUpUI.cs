using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WPM
{
    public class InventoryPopUpUI : UIElement, IInventoryPopUpUI, IDropOffUI
    {
        //Public Variables
        public bool TempPopUp { get; set; }
        //Private Variables
        private bool dropOffEnabled = false;
        private bool started;
        private bool persistantPopUp;
        private string persistantPopUpMessage;
        //UI Components
        private Button dropOffButton;
        private GameObject dropOffButtonObject;
        private Button[] buttonComponents;
        private Text[] textComponents;
        private Text displayText;
        //Error Handling
        bool componentMissing = false;

        protected override void Awake()
        {
            base.Awake();
            textComponents = UIObject.GetComponentsInChildren<Text>();
            if (textComponents == null)
                componentMissing = true;
            else
                displayText = textComponents[0];

            buttonComponents = UIObject.GetComponentsInChildren<Button>();
            if (buttonComponents == null || buttonComponents.Length > 1)
                componentMissing = true;
            else
            {
                dropOffButton = buttonComponents[0];
                dropOffButtonObject = dropOffButton.gameObject;
            }
        }

        protected override void Start()
        {
            base.Start();
            if (gameObject.activeSelf)
            {
                if (componentMissing)
                    errorHandler.ReportError("Error getting Inventory Pop Up UI Elements", ErrorState.close_window);
            }
            started = true;
            CloseUI();
        }

        public void DisplayPopUp(string displayString, bool persistant)
        {
            if (!started)
                Start();

            OpenUI();
            displayText.text = displayString;
            if (persistant)
            {
                persistantPopUpMessage = displayString;
                persistantPopUp = true;
            }
            else
            {
                TempPopUp = true;
                ToggleOptionForDropOff(false);
            }
        }

        public void ClearPopUp(bool persistant)
        {
            TempPopUp = false;
            if (persistant)
            {
                persistantPopUpMessage = "";
                persistantPopUp = false;
                CloseUI();
            }
            else
            {
                if (persistantPopUp)
                    ResetPersistantMessage();
                else
                    CloseUI();
            }
        }

        public void ResetPersistantMessage()
        {
            DisplayPopUp(persistantPopUpMessage, true);
            ToggleOptionForDropOff(true);
        }

        public void SetDropOffDelegate(DropOffDelegate dropOffDelegate)
        {
            if(dropOffDelegate == null)
            {
                errorHandler.ReportError("Drop Off Delegate Null", ErrorState.close_window);
                return;
            }
            dropOffButton.onClick.AddListener(delegate { dropOffDelegate(); });
        }

        public void ClearDropOffDelegate()
        {
            dropOffButton.onClick.RemoveAllListeners();
        }

        public void ToggleOptionForDropOff(bool active)
        {
            dropOffButtonObject.SetActive(active);
        }

    }
}
