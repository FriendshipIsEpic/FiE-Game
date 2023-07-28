using System;
using System.Collections.Generic;
// Cinema Suite
using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// The base class for all cinema events
    /// </summary>
    [ExecuteInEditMode]
    public abstract class CinemaActorAction : TimelineAction
    {
        /// <summary>
        /// Called when the running time of the cutscene hits the firetime of the action
        /// </summary>
        public abstract void Trigger(GameObject Actor);

        /// <summary>
        /// Called at each update when the action is to be played.
        /// </summary>
        public virtual void UpdateTime(GameObject Actor, float time, float deltaTime) { }

        /// <summary>
        /// Called when the running time of the cutscene exceeds the duration of the action
        /// </summary>
        public abstract void End(GameObject Actor);

        /// <summary>
        /// Called when the cutscene exists preview/play mode. Return properties to pre-cached state if necessary.
        /// </summary>
        public virtual void Stop(GameObject Actor) { }

        /// <summary>
        /// Called when the cutscene time is set/skipped manually.
        /// </summary>
        /// <param name="Actor">The actor to target for this event.</param>
        /// <param name="time">The new running time.</param>
        /// <param name="deltaTime">The deltaTime since the last update call.</param>
        public virtual void SetTime(GameObject Actor, float time, float deltaTime) { }

        /// <summary>
        /// Reverse trigger. Called when scrubbing backwards.
        /// </summary>
        public virtual void ReverseTrigger(GameObject Actor) { }

        /// <summary>
        /// Reverse End. Called when scrubbing backwards.
        /// </summary>
        public virtual void ReverseEnd(GameObject Actor) { }

        /// <summary>
        /// Pause any action as necessary
        /// </summary>
        public virtual void Pause(GameObject Actor) { }

        /// <summary>
        /// Resume from paused.
        /// </summary>
        public virtual void Resume(GameObject Actor) { }

        public int CompareTo(object other)
        {
            CinemaGlobalAction otherAction = (CinemaGlobalAction)other;
            return (int)(otherAction.Firetime - this.Firetime);
        }

        /// <summary>
        /// Get the actors associated with this Actor Action. Can return null.
        /// </summary>
        /// <returns>A set of actors related to this actor event.</returns>
        public virtual List<Transform> GetActors()
        {
            IMultiActorTrack track = (TimelineTrack as IMultiActorTrack);
            if (track != null)
            {
                return track.Actors;
            }
            return null;
        }

        /// <summary>
        /// Called when the cutscene time is set/skipped manually.
        /// </summary>
        [Obsolete("Use SetTime with Actor")]
        public virtual void SetTime(float time, float deltaTime) { }

        /// <summary>
        /// Reverse trigger. Called when scrubbing backwards.
        /// </summary>
        [Obsolete("Use ReverseTrigger with Actor")]
        public virtual void ReverseTrigger() { }

        /// <summary>
        /// Reverse End. Called when scrubbing backwards.
        /// </summary>
        [Obsolete("Use ReverseEnd with Actor")]
        public virtual void ReverseEnd() { }
    }
}