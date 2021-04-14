using System.Collections.Generic;

namespace WPM
{
    public interface ITurnsManager
    {
        List<ITurnBasedObject> TurnBasedObjects { get; set; }
        int TurnsRemaining { get; }

        void NextTurn(int turns);
    }
}