// Cinema Suite
using CinemaDirector.Helpers;
using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// The character track group is a type of actor group, specialized for humanoid characters.
    /// </summary>
    [TrackGroupAttribute("Character Track Group", TimelineTrackGenre.CharacterTrack)]
    public class CharacterTrackGroup : ActorTrackGroup, IRevertable, IBakeable
    {
        // Options for reverting in editor.
        [SerializeField]
        private RevertMode editorRevertMode = RevertMode.Revert;

        // Options for reverting during runtime.
        [SerializeField]
        private RevertMode runtimeRevertMode = RevertMode.Revert;

        // Has a bake been called on this track group?
        private bool hasBeenBaked = false;

        /// <summary>
        /// Bake the Mecanim preview data.
        /// </summary>
        public void Bake()
        {
            if (Actor == null || Application.isPlaying) return;
            Animator animator = Actor.GetComponent<Animator>();
            if (animator == null)
            { return; }
            
            float frameRate = 30;
            int frameCount = (int)((Cutscene.Duration * frameRate) + 2);
            animator.StopPlayback();
            animator.recorderStartTime = 0;
            animator.StartRecording(frameCount);
            for (int i = 0; i < frameCount; i++)
            {
                base.UpdateTrackGroup(i * (1.0f / frameRate), (1.0f / frameRate));
                animator.Update(1.0f / frameRate);
            }
            animator.recorderStopTime = frameCount * (1.0f / frameRate);
            animator.StopRecording();
            animator.StartPlayback();

            hasBeenBaked = true;
        }

        /// <summary>
        /// Cache the Actor Transform.
        /// </summary>
        /// <returns>The revert info for the Actor's transform.</returns>
        public RevertInfo[] CacheState()
        {
            RevertInfo[] reverts = new RevertInfo[3];
            if (Actor == null) return new RevertInfo[0];
            reverts[0] = new RevertInfo(this, Actor.transform, "localPosition", Actor.transform.localPosition);
            reverts[1] = new RevertInfo(this, Actor.transform, "localRotation", Actor.transform.localRotation);
            reverts[2] = new RevertInfo(this, Actor.transform, "localScale", Actor.transform.localScale);
            return reverts;
        }

        /// <summary>
        /// Initialize the Track Group as normal and initialize the Animator if in Editor Mode.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            if (!Application.isPlaying)
            {
                if (Actor == null) return;
                Animator animator = Actor.GetComponent<Animator>();
                if (animator == null)
                {
                    return;
                }
                animator.StartPlayback();
            }
        }

        /// <summary>
        /// Update the Track Group over time. If in editor mode, play the baked animator data.
        /// </summary>
        /// <param name="time">The new running time.</param>
        /// <param name="deltaTime">the deltaTime since last update.</param>
        public override void UpdateTrackGroup(float time, float deltaTime)
        {
            if (Application.isPlaying)
            {
                base.UpdateTrackGroup(time, deltaTime);
            }
            else
            {
                foreach (TimelineTrack track in GetTracks())
                {
                    if (!(track is MecanimTrack))
                    {
                        track.UpdateTrack(time, deltaTime);
                    } 
                }

                if (Actor == null) return;
                Animator animator = Actor.GetComponent<Animator>();
                if (animator == null)
                {
                    return;
                }

                animator.playbackTime = time;
                animator.Update(0);
            }
        }

        /// <summary>
        /// Stop this track group and stop playback on animator.
        /// </summary>
        public override void Stop()
        {
            base.Stop();

            if (!Application.isPlaying)
            {
                if (hasBeenBaked)
                {
                    Animator animator = Actor.GetComponent<Animator>();
                    if (animator == null)
                    {
                        return;
                    }

                    if (animator.recorderStopTime > 0)
                    {
                        animator.StartPlayback();
                        animator.playbackTime = 0;
                        animator.Update(0);
                        animator.StopPlayback();

                        animator.Rebind();
                    }
                }
            }
        }

        /// <summary>
        /// Option for choosing when this Event will Revert to initial state in Editor.
        /// </summary>
        public RevertMode EditorRevertMode
        {
            get { return editorRevertMode; }
            set { editorRevertMode = value; }
        }

        /// <summary>
        /// Option for choosing when this Event will Revert to initial state in Runtime.
        /// </summary>
        public RevertMode RuntimeRevertMode
        {
            get { return runtimeRevertMode; }
            set { runtimeRevertMode = value; }
        }
    }
}