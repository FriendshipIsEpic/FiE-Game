// Cinema Suite 2014

using UnityEngine;
namespace CinemaDirector
{
    [TimelineTrackAttribute("Curve Track", TimelineTrackGenre.MultiActorTrack, CutsceneItemGenre.MultiActorCurveClipItem)]
    public class MultiCurveTrack : TimelineTrack, IActorTrack
    {

        public override void Initialize()
        {
            foreach (CinemaMultiActorCurveClip clipCurve in this.TimelineItems)
            {
                clipCurve.Initialize();
            }
        }

        public override void UpdateTrack(float time, float deltaTime)
        {
            base.elapsedTime = time;
            foreach (CinemaMultiActorCurveClip clipCurve in this.TimelineItems)
            {
                clipCurve.SampleTime(time);
            }
        }

        public override void Stop()
        {
            foreach (CinemaMultiActorCurveClip clipCurve in this.TimelineItems)
            {
                clipCurve.Revert();
            }
        }

        public override TimelineItem[] TimelineItems
        {
            get
            {
                return GetComponentsInChildren<CinemaMultiActorCurveClip>();
            }
        }

        public Transform Actor
        {
            get
            {
                ActorTrackGroup component = base.transform.parent.GetComponent<ActorTrackGroup>();
                if (component == null)
                {
                    return null;
                }
                return component.Actor;
            }
        }
    }
}