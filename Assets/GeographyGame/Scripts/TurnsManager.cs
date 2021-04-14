using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPM
{
    public class TurnsManager : MonoBehaviour, ITurnsManager
    {
        public int TurnsRemaining { get; protected set; } = 250;
        public List<ITurnBasedObject> TurnBasedObjects { get; set; } = new List<ITurnBasedObject>();
        private IGameManager gameManager;
        private IUIManager uiManager;
        private int globalTurnCounter = 0;
        
        void Start()
        {
            InterfaceFactory interfaceFactory = FindObjectOfType<InterfaceFactory>();
            gameManager = interfaceFactory.GameManager;
            uiManager = interfaceFactory.UIManager;
        }

        /// <summary>
        /// Called whenever a new turn happens in game. Multiple turns can pass at once.
        /// Inputs:
        ///     turns: How many turns are passing
        /// </summary>
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
            uiManager.TurnsUI.UpdateDisplayedRemainingTurns(TurnsRemaining);
        }
    }

}
