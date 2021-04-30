using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WPM
{
    public class InventoryPopUpUI : UIElement, IInventoryPopUpUI
    {
        private Text[] textComponents;
        private Text displayText;
        //public bool PersistantPopUp { get; set; }
        private bool persistantPopUp;
        public bool TempPopUp { get; set; }
        private string persistantPopUpMessage;
        protected override void Awake()
        {
            base.Awake();
            textComponents = UIObject.GetComponentsInChildren<Text>();
            displayText = textComponents[0];
        }

        protected override void Start()
        {
            base.Start();
            CloseUI();
        }

        public void DisplayPopUp(string displayString, bool persistant)
        {
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
