using System.Collections.Generic;
// Cinema Suite
using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// An implementation of an event that can be performed on an arbitrary actor.
    /// </summary>
    [ExecuteInEditMode]
    public abstract class CinemaActorEvent : TimelineItem
    {
        /// <summary>
        /// Trigger this event using the given actor.
        /// </summary>
        /// <param name="Actor">The actor to perform the event on.</param>
        public abstract void Trigger(GameObject Actor);

        /// <summary>
        /// Reverse the trigger.
        /// </summary>
        /// <param name="Actor">The actor to perform the event on.</param>
        public virtual void Reverse(GameObject Actor) { }

        public virtual void SetTimeTo(float deltaTime) { }

        public virtual void Pause() { }

        public virtual void Resume() { }

        public virtual void Initialize(GameObject Actor) { }

        public virtual void Stop(GameObject Actor) { }

        /// <summary>
        /// Get the actors associated with this Actor Event. Can return null.
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
        /// The Actor Track Group associated with this event.
        /// </summary>
        public ActorTrackGroup ActorTrackGroup
        {
            get
            {
                return this.TimelineTrack.TrackGroup as ActorTrackGroup;
            }
        }

        
    }
}