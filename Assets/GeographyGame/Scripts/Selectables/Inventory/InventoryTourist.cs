using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace WPM
{
    public class InventoryTourist : InventoryItem, IInventoryTourist
    {
        //Local Reference Interfaces
        private IDropOffUI dropOffUI;
        private ITouristManager touristManager;
        private IScoreManager scoreManager;
        private IInventoryPopUpUI inventoryPopUpUI;
        //Component Objects
        private IDestinationSetter destinationSetter;
        private ITouristDropper touristDropper;
        //Public Variables
        public string DestinationName { get; set; }
        public Province ProvinceDestination { get; set; }
        public Landmark LandmarkDestination { get; set; }
        public Country CountryDestination { get; set; }
        public DestinationType DestinationType { get; set; }
        //Local Variables
        private Text dialog;
        private int destinationIndex;
        private bool componentMissing = false;
        private bool destinationSet = false;
        //Local Constants
        private const int TOURIST_DROP_OFF_SCORE = 100;

        protected override void Awake()
        {
            base.Awake();
            try
            {
                destinationSetter = GetComponent(typeof(IDestinationSetter)) as IDestinationSetter;
                touristDropper = GetComponent(typeof(ITouristDropper)) as ITouristDropper;
            }
            catch
            {
                componentMissing = true;
            }
        }

        protected override void Start()
        {
            base.Start();
            if (gameObject.activeSelf)
            {
                if (componentMissing == true)
                    errorHandler.ReportError("Tourist component missing", ErrorState.close_window);

                dropOffUI = uiManager.DropOffUI;
                if (dropOffUI == null)
                    errorHandler.ReportError("Drop Off UI missing", ErrorState.restart_scene);

                touristManager = gameManager.TouristManager;
                if (touristManager == null)
                    errorHandler.ReportError("Tourist Manager missing", ErrorState.restart_scene);

                scoreManager = gameManager.ScoreManager;
                if (scoreManager == null)
                    errorHandler.ReportError("Score Manager missing", ErrorState.restart_scene);

                inventoryPopUpUI = uiManager.InventoryPopUpUI;
                if (inventoryPopUpUI == null)
                    errorHandler.ReportError("Inventory Pop Up UI missing", ErrorState.restart_scene);

                //Set Tourist Destination
                destinationSet = destinationSetter.SetDestination(this, touristManager.CurrentRegion);
                if (destinationSet)
                {
                    inventoryPopUpUI.DisplayPopUp("Hey there!  I want to see " + DestinationName + "!", false);
                    dropOffUI.ToggleOptionForDropOff(false);
                }
                else
                    errorHandler.ReportError("Tourist destination not set", ErrorState.close_window);      
            }
        }

        public override void Select()
        {
            base.Select();
            dropOffUI.ToggleOptionForDropOff(true);
            dropOffUI.SetDropOffDelegate(AttemptDropOff);
            SetPopUpRequest(true);
        }

        public override void Deselect()
        {
            base.Deselect();
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            dropOffUI.ToggleOptionForDropOff(false);
            dropOffUI.ClearDropOffDelegate();
            inventoryPopUpUI.ClearPopUp(true);
        }

        public override void MouseEnter()
        {
            inventoryPopUpUI.DisplayPopUp("I want to see " + DestinationName + "!", false);
        }
        
        public override void MouseExit()
        {
            inventoryPopUpUI.ClearPopUp(false);
        }
        
        public void SetPopUpRequest(bool persistant)
        {
            inventoryPopUpUI.DisplayPopUp("I want to see " + DestinationName + "!", persistant);
        }

        public override void OnCellClick(int index)
        {
            if(index == playerCharacter.CellLocation.index)
            {
                playerCharacter.Select();
            }
        }

        public override void MouseDown()
        {
            Select();
        }

        public override void OnCellEnter(int index)
        {
            //Nothing Happens
        }

        public override void OnSelectableEnter(ISelectableObject selectableObject)
        {
            //Nothing Happens
        }

        public override void OtherObjectSelected(ISelectableObject selectedObject)
        {
            //There will need to be check later to account for multiple object selection
        }

        public void AttemptDropOff()
        {
            //Clear the Event System so that it gets updated with the tourist if the drop off fails
            EventSystem.current.SetSelectedGameObject(null);

            bool dropOffSuccess = touristDropper.AttemptDropOff(playerCharacter.CellLocation, DestinationType, ProvinceDestination, LandmarkDestination, CountryDestination);

            if (dropOffSuccess)
                DropOffSuccess();
            else
                DropOffFailure();     
        }

        /// <summary> 
        /// Drops off the tourists and indicates success to the player
        /// </summary>
        private void DropOffSuccess()
        {
            Deselect();
            //Remove Tourist from Inventory
            playerCharacter.RemoveItem(InventoryLocation);
            scoreManager.UpdateScore(TOURIST_DROP_OFF_SCORE);
            uiManager.CursorOverUI = false;
            inventoryPopUpUI.DisplayPopUp("Exactly where I wanted to go!", false);

            /*  This will be for drop off sound effects
            if (success)
                dropOffSuccess.Play();
            else
                dropOffFailure.Play();
           */
        }

        /// <summary> 
        /// Indicates to the player that the tourist drop off failed
        /// </summary>
        private void DropOffFailure()
        {
            inventoryPopUpUI.DisplayPopUp("Well this doesn't look right. . . .", false);
            return;
        }
        
    }
}
