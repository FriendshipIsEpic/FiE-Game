// Cinema Suite
using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// A basic global event that can be Triggered when the Firetime is reached.
    /// </summary>
    public abstract class CinemaGlobalEvent : TimelineItem
    {
        /// <summary>
        /// The event is triggered when the Cutscene's runningtime passes the fire time.
        /// </summary>
        public abstract void Trigger();

        /// <summary>
        /// The event can be triggered when the Cutscene is played in reverse passing the fire time.
        /// </summary>
        public virtual void Reverse() { }
    }
}