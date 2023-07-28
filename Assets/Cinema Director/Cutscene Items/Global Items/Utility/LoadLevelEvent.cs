// Cinema Suite
using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// Event for loading a new level
    /// </summary>
    [CutsceneItem("Utility", "Load Level", CutsceneItemGenre.GlobalItem)]
    public class LoadLevelEvent : CinemaGlobalEvent
    {
        // The index of the level to be loaded.
        public int Level = 0;

        /// <summary>
        /// Trigger the level load. Only in Runtime.
        /// </summary>
        public override void Trigger()
        {
            if (Application.isPlaying)
            {
                Application.LoadLevel(Level);
            }
        }
    }
}