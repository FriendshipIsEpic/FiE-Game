// Cinema Suite
using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// Transition from Clear to Black over time by overlaying a guiTexture.
    /// </summary>
    [CutsceneItem("Transitions", "Fade to Black", CutsceneItemGenre.GlobalItem)]
    public class FadeToBlack : CinemaGlobalAction
    {
        private Color From = Color.clear;
        private Color To = Color.black;

        /// <summary>
        /// Setup the effect when the script is loaded.
        /// </summary>
        void Awake()
        {
            if (guiTexture == null)
            {
                gameObject.transform.position = Vector3.zero;
                gameObject.transform.localScale = new Vector3(100, 100, 100);
                gameObject.AddComponent<GUITexture>();
                guiTexture.enabled = false;
                guiTexture.texture = new Texture2D(1, 1);
                guiTexture.pixelInset = new Rect(0f, 0f, Screen.width, Screen.height);
                guiTexture.color = Color.clear;
            }
        }

        /// <summary>
        /// Enable the overlay texture and set the Color to Clear.
        /// </summary>
        public override void Trigger()
        {
            guiTexture.enabled = true;
            guiTexture.pixelInset = new Rect(0f, 0f, Screen.width, Screen.height);
            guiTexture.color = From;
        }

        /// <summary>
        /// Firetime is reached when playing in reverse, disable the effect.
        /// </summary>
        public override void ReverseTrigger()
        {
            End();
        }

        /// <summary>
        /// Update the effect over time, progressing the transition
        /// </summary>
        /// <param name="time">The time this action has been active</param>
        /// <param name="deltaTime">The time since the last update</param>
        public override void UpdateTime(float time, float deltaTime)
        {
            float transition = time / Duration;
            FadeToColor(From, To, transition);
        }

        /// <summary>
        /// Set the transition to an arbitrary time.
        /// </summary>
        /// <param name="time">The time of this action</param>
        /// <param name="deltaTime">the deltaTime since the last update call.</param>
        public override void SetTime(float time, float deltaTime)
        {
            if (time >= 0 && time <= Duration)
            {
                guiTexture.enabled = true;
                UpdateTime(time, deltaTime);
            }
            else if (guiTexture.enabled)
            {
                guiTexture.enabled = false;
            }
        }

        /// <summary>
        /// End the effect by disabling the overlay texture.
        /// </summary>
        public override void End()
        {
            guiTexture.enabled = false;
        }

        /// <summary>
        /// The end of the action has been triggered while playing the Cutscene in reverse.
        /// </summary>
        public override void ReverseEnd()
        {
            guiTexture.enabled = true;
            guiTexture.pixelInset = new Rect(0f, 0f, Screen.width, Screen.height);
            guiTexture.color = To;
        }

        /// <summary>
        /// Disable the overlay texture
        /// </summary>
        public override void Stop()
        {
            if (guiTexture != null)
            {
                guiTexture.enabled = false;
            }
        }

        /// <summary>
        /// Fade from one colour to another over a transition period.
        /// </summary>
        /// <param name="from">The starting colour</param>
        /// <param name="to">The final colour</param>
        /// <param name="transition">the Lerp transition value</param>
        private void FadeToColor(Color from, Color to, float transition)
        {
            guiTexture.color = Color.Lerp(from, to, transition);
        }

    }
}
