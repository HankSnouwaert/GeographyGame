using System.Collections.Generic;

namespace WPM
{
    /// <summary> 
    /// Tracks the game's turns and all turn based objects
    /// </summary>
    public interface ITurnsManager
    {
        /// <summary> 
        /// All the objects in the game that are dependent on the game's turns
        /// </summary>
        List<ITurnBasedObject> TurnBasedObjects { get; set; }

        /// <summary> 
        /// The turns remaining in game
        /// </summary>
        int TurnsRemaining { get; }

        /// <summary>
        /// Called whenever a new turn happens in game. Multiple turns can pass at once.
        /// </summary>
        /// <param name="turns"> How many turns are passing </param>
        void NextTurn(int turns);
    }
}