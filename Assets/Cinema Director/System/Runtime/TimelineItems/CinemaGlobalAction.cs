using System;
using UnityEngine;

namespace CinemaDirector
{
    [ExecuteInEditMode]
    public abstract class CinemaGlobalAction : TimelineAction, IComparable
    {
        /// <summary>
        /// Called when the running time of the cutscene hits the firetime of the action
        /// </summary>
        public abstract void Trigger();

        /// <summary>
        /// Called at each update when the action is to be played.
        /// </summary>
        public virtual void UpdateTime(float time, float deltaTime) { }

        /// <summary>
        /// Called when the running time of the cutscene exceeds the duration of the action
        /// </summary>
        public abstract void End();

        /// <summary>
        /// Called when the cutscene time is set/skipped manually.
        /// </summary>
        public virtual void SetTime(float time, float deltaTime) { }

        /// <summary>
        /// Pause any action as necessary
        /// </summary>
        public virtual void Pause() { }

        /// <summary>
        /// Resume from paused.
        /// </summary>
        public virtual void Resume() { }

        /// <summary>
        /// Reverse trigger. Called when scrubbing backwards.
        /// </summary>
        public virtual void ReverseTrigger() { }

        /// <summary>
        /// Reverse End. Called when scrubbing backwards.
        /// </summary>
        public virtual void ReverseEnd() { }

        public int CompareTo(object other)
        {
            CinemaGlobalAction otherAction = (CinemaGlobalAction)other;
            return (int)(otherAction.Firetime - this.Firetime);
        }
    }
}