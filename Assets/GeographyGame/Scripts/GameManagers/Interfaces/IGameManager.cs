﻿namespace WPM
{
    /// <summary> 
    /// Interface used to hold general game state information and access to manager and player interfaces
    /// </summary>
    public interface IGameManager
    {
        /// <summary> 
        /// Interface used to intantiate and access the player object
        /// </summary>
        IPlayerManager PlayerManager { get; }
        /// <summary> 
        /// Interface used to access in-game camera functionality
        /// </summary>
        ICameraManager CameraManager { get; }
        /// <summary> 
        /// Interface used to access tourist functionality
        /// </summary>
        ITouristManager TouristManager { get; }
        /// <summary> 
        /// Interface used to access game turns
        /// </summary>
        ITurnsManager TurnsManager { get; }
        /// <summary> 
        /// Interface used to access game score
        /// </summary>
        IScoreManager ScoreManager { get; }
        /// <summary> 
        /// Object the cursor is currently over
        /// </summary>
        ISelectableObject HighlightedObject { get; set; }
        /// <summary> 
        /// Object currently selected by player
        /// </summary>
        ISelectableObject SelectedObject { get; set; }
        /// <summary> 
        /// Flag that indicates if the game is paused
        /// </summary>
        bool GamePaused { get; set; }
        /// <summary> 
        /// Exit the application
        /// </summary>
        void ExitGame();
        /// <summary> 
        /// End the game
        /// </summary>
        void GameOver();
        /// <summary> 
        /// Restart the game
        /// </summary>
        void GameReset();
        /// <summary> 
        /// Resume the game if it is paused
        /// </summary>
        void ResumeGame();
    }
}