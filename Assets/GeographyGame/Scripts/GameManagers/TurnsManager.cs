using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPM
{
    public class TurnsManager : MonoBehaviour, ITurnsManager
    {
        //Public Variables
        public int TurnsRemaining { get; protected set; } = 250;
        public List<ITurnBasedObject> TurnBasedObjects { get; set; } = new List<ITurnBasedObject>();
        //Internal Interface References
        private IGameManager gameManager;
        private IUIManager uiManager;
        private ITurnsUI turnsUI;
        //Private Variables
        private int globalTurnCounter = 0;
        //Error Checking
        private InterfaceFactory interfaceFactory;
        private IErrorHandler errorHandler;

        private void Awake()
        {
            interfaceFactory = FindObjectOfType<InterfaceFactory>();
            if(interfaceFactory == null)
                gameObject.SetActive(false);
        }

        private void Start()
        {
            gameManager = interfaceFactory.GameManager;
            uiManager = interfaceFactory.UIManager;
            errorHandler = interfaceFactory.ErrorHandler;
            if (gameManager == null || uiManager == null || errorHandler == null)
                gameObject.SetActive(false);
            else
            {
                turnsUI = uiManager.TurnsUI;
                if (turnsUI == null)
                    errorHandler.ReportError("Turns UI Missing", ErrorState.restart_scene);
                else
                    turnsUI.UpdateDisplayedRemainingTurns(TurnsRemaining);
            }
        }

        /// <summary>
        /// Called whenever a new turn happens in game. Multiple turns can pass at once.
        /// </summary>
        /// <param name="turns"> How many turns are passing </param>
        public void NextTurn(int turns)
        {
            globalTurnCounter = globalTurnCounter + turns;
            UpdateRemainingTurns(turns * -1);
            //Run any end of turn scripts for the rest of the game's objects
            foreach (ITurnBasedObject turnBasedObject in TurnBasedObjects)
            {
                turnBasedObject.EndOfTurn(turns);
            }
        }

        /// <summary> 
        /// Update the turns remaining until the game ends and check if game has ended
        /// </summary>
        /// <param name="turnModification"></param> The number of turns the reminaing turns
        /// are updated by
        private void UpdateRemainingTurns(int turnModification)
        {
            TurnsRemaining = TurnsRemaining + turnModification;
            if (TurnsRemaining <= 0)
            {
                TurnsRemaining = 0;
                gameManager.GameOver();
            }
            turnsUI.UpdateDisplayedRemainingTurns(TurnsRemaining);
        }
    }

}
