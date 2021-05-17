using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WPM
{
    public class InventoryPopUpUI : UIElement, IInventoryPopUpUI
    {
        //Public Variables
        public bool TempPopUp { get; set; }

        //Private Variables
        private bool started;
        private Text[] textComponents;
        private Text displayText;
        private bool persistantPopUp;
        private string persistantPopUpMessage;
        //Error Handling
        bool componentMissing = false;

        protected override void Awake()
        {
            base.Awake();
            textComponents = UIObject.GetComponentsInChildren<Text>();
            if (textComponents == null || textComponents.Length > 1)
            {
                componentMissing = true;
            }
            else
            {
                displayText = textComponents[0];
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
        }
    }
}
