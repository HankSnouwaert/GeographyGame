namespace WPM
{
    /// <summary> 
    /// Used to instantiate and reference the player character object
    /// </summary>
    public interface IPlayerManager
    {
        /// <summary> 
        /// The player character object
        /// </summary>
        IPlayerCharacter PlayerCharacter { get; }

        /// <summary> 
        /// Intantiate the player character in a given cell
        /// </summary>
        /// <param 
        /// name="startCell"> The cell the player will be generated in 
        /// </param>
        /// <returns> 
        /// A flag indicating if the player was successfully instantiated
        /// </returns>
        bool IntantiatePlayer(Cell startCell);
    }
}