using System;
// Cinema Suite
using System.Collections.Generic;
using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// A track which maintains all timeline items marked for actor tracks and multi actor tracks.
    /// </summary>
    [TimelineTrackAttribute("Actor Track", new TimelineTrackGenre[] { TimelineTrackGenre.ActorTrack, TimelineTrackGenre.MultiActorTrack }, CutsceneItemGenre.ActorItem)]
    public class ActorItemTrack : TimelineTrack, IActorTrack, IMultiActorTrack
    {
        /// <summary>
        /// Initialize this Track and all the timeline items contained within.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            foreach (CinemaActorEvent cinemaEvent in this.ActorEvents)
            {
                foreach (Transform actor in Actors)
                {
                    if (actor != null)
                    {
                        cinemaEvent.Initialize(actor.gameObject);
                    }
                }
            }
        }

        /// <summary>
        /// The cutscene has been set to an arbitrary time by the user.
        /// Processing must take place to catch up to the new time.
        /// </summary>
        /// <param name="time">The new cutscene running time</param>
        public override void SetTime(float time)
        {
            float previousTime = elapsedTime;
            base.SetTime(time);

            foreach (TimelineItem item in GetTimelineItems())
            {
                // Check if it is an actor event.
                CinemaActorEvent cinemaEvent = item as CinemaActorEvent;
                if (cinemaEvent != null)
                {
                    if ((previousTime < cinemaEvent.Firetime) && (((elapsedTime >= cinemaEvent.Firetime))))
                    {
                        foreach (Transform actor in Actors)
                        {
                            if (actor != null)
                                cinemaEvent.Trigger(actor.gameObject);
                        }
                    }
                    else if (((previousTime >= cinemaEvent.Firetime) && (elapsedTime < cinemaEvent.Firetime)))
                    {
                        foreach (Transform actor in Actors)
                        {
                            if (actor != null)
                                cinemaEvent.Reverse(actor.gameObject);
                        }
                    }
                }

                // Check if it is an actor action.
                CinemaActorAction action = item as CinemaActorAction;
                if (action != null)
                {
                    foreach (Transform actor in Actors)
                    {
                        if (actor != null)
                            action.SetTime(actor.gameObject, (time - action.Firetime), time - previousTime);
                    }
                }
            }
        }

        /// <summary>
        /// Update this track since the last frame.
        /// </summary>
        /// <param name="time">The new running time.</param>
        /// <param name="deltaTime">The deltaTime since last update.</param>
        public override void UpdateTrack(float time, float deltaTime)
        {
            float previousTime = base.elapsedTime;
            base.UpdateTrack(time, deltaTime);

            foreach (TimelineItem item in GetTimelineItems())
            {
                // Check if it is an actor event.
                CinemaActorEvent cinemaEvent = item as CinemaActorEvent;
                if (cinemaEvent != null)
                {
                    if ((previousTime < cinemaEvent.Firetime) && (((base.elapsedTime >= cinemaEvent.Firetime))))
                    {
                        foreach (Transform actor in Actors)
                        {
                            if (actor != null)
                                cinemaEvent.Trigger(actor.gameObject);
                        }
                    }
                    if (((previousTime >= cinemaEvent.Firetime) && (base.elapsedTime < cinemaEvent.Firetime)))
                    {
                        foreach (Transform actor in Actors)
                        {
                            if (actor != null)
                                cinemaEvent.Reverse(actor.gameObject);
                        }
                    }
                }

                CinemaActorAction action = item as CinemaActorAction;
                if (action != null)
                {
                    if ((previousTime < action.Firetime && base.elapsedTime >= action.Firetime) && base.elapsedTime < action.EndTime)
                    {
                        foreach (Transform actor in Actors)
                        {
                            if (actor != null)
                            {
                                action.Trigger(actor.gameObject);
                            }
                        }
                    }
                    else if (previousTime <= action.EndTime && base.elapsedTime > action.EndTime)
                    {
                        foreach (Transform actor in Actors)
                        {
                            if (actor != null)
                            {
                                action.End(actor.gameObject);
                            }
                        }
                    }
                    else if (previousTime >= action.Firetime && previousTime < action.EndTime && base.elapsedTime < action.Firetime)
                    {
                        foreach (Transform actor in Actors)
                        {
                            if (actor != null)
                            {
                                action.ReverseTrigger(actor.gameObject);
                            }
                        }
                    }
                    else if ((previousTime > action.EndTime && (base.elapsedTime > action.Firetime) && (base.elapsedTime <= action.EndTime)))
                    {
                        foreach (Transform actor in Actors)
                        {
                            if (actor != null)
                            {
                                action.ReverseEnd(actor.gameObject);
                            }
                        }
                    }
                    else if ((base.elapsedTime > action.Firetime) && (base.elapsedTime <= action.EndTime))
                    {
                        foreach (Transform actor in Actors)
                        {
                            if (actor != null)
                            {
                                float runningTime = time - action.Firetime;
                                action.UpdateTime(actor.gameObject, runningTime, deltaTime);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Resume playback after being paused.
        /// </summary>
        public override void Resume()
        {
            base.Resume();
            foreach (TimelineItem item in GetTimelineItems())
            {
                CinemaActorAction action = item as CinemaActorAction;
                if (action != null)
                {
                    if (((elapsedTime > action.Firetime)) && (elapsedTime < (action.Firetime + action.Duration)))
                    {
                        foreach (Transform actor in Actors)
                        {
                            if (actor != null)
                            {
                                action.Resume(actor.gameObject);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Stop the playback of this track.
        /// </summary>
        public override void Stop()
        {
            base.Stop();
            base.elapsedTime = 0f;
            foreach (TimelineItem item in GetTimelineItems())
            {
                CinemaActorEvent cinemaEvent = item as CinemaActorEvent;
                if (cinemaEvent != null)
                {
                    foreach (Transform actor in Actors)
                    {
                        if (actor != null)
                            cinemaEvent.Stop(actor.gameObject);
                    }
                }
            
                CinemaActorAction action = item as CinemaActorAction;
                if (action != null)
                {
                    foreach (Transform actor in Actors)
                    {
                        if (actor != null)
                            action.Stop(actor.gameObject);
                    }
                }
            }
        }

        /// <summary>
        /// Get the Actor associated with this track. Can return null.
        /// </summary>
        public Transform Actor
        {
            get
            {
                ActorTrackGroup atg = this.TrackGroup as ActorTrackGroup;
                if (atg == null)
                {
                    Debug.LogError("No ActorTrackGroup found on parent.", this);
                    return null;
                }
                return atg.Actor;
            }
        }

        /// <summary>
        /// Get the Actors associated with this track. Can return null.
        /// In the case of MultiActors it will return the full list.
        /// </summary>
        public List<Transform> Actors
        {
            get
            {
                ActorTrackGroup trackGroup = TrackGroup as ActorTrackGroup;
                if (trackGroup != null)
                {
                    List<Transform> actors = new List<Transform>() { };
                    actors.Add(trackGroup.Actor);
                    return actors;
                }

                MultiActorTrackGroup multiActorTrackGroup = TrackGroup as MultiActorTrackGroup;
                if (multiActorTrackGroup != null)
                {
                    return multiActorTrackGroup.Actors;
                }
                return null;
            }
        }

        public CinemaActorEvent[] ActorEvents
        {
            get
            {
                return base.GetComponentsInChildren<CinemaActorEvent>();
            }
        }

        public CinemaActorAction[] ActorActions
        {
            get
            {
                return base.GetComponentsInChildren<CinemaActorAction>();
            }
        }
    }
}