// Cinema Suite
using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// Generic transition From any colour To any colour.
    /// </summary>
    [CutsceneItem("Transitions", "Color Transition", CutsceneItemGenre.GlobalItem)]
    public class ColorTransition : CinemaGlobalAction
    {
        // The starting colour
        public Color From = Color.black;

        // The final colour
        public Color To = Color.clear;

        /// <summary>
        /// Setup the effect when the script is loaded.
        /// </summary>
        void Awake()
        {
            if (GetComponent<GUITexture>() == null)
            {
                gameObject.transform.position = Vector3.zero;
                gameObject.transform.localScale = new Vector3(100, 100, 100);
                gameObject.AddComponent<GUITexture>();
                GetComponent<GUITexture>().enabled = false;
                GetComponent<GUITexture>().texture = new Texture2D(1, 1);
                GetComponent<GUITexture>().pixelInset = new Rect(0f, 0f, Screen.width, Screen.height);
                GetComponent<GUITexture>().color = Color.clear;
            }
        }

        /// <summary>
        /// Enable the overlay texture and set to From colour
        /// </summary>
        public override void Trigger()
        {
            GetComponent<GUITexture>().enabled = true;
            GetComponent<GUITexture>().pixelInset = new Rect(0f, 0f, Screen.width, Screen.height);
            GetComponent<GUITexture>().color = From;
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
                GetComponent<GUITexture>().enabled = true;
                UpdateTime(time, deltaTime);
            }
            else if (GetComponent<GUITexture>().enabled)
            {
                GetComponent<GUITexture>().enabled = false;
            }
        }

        /// <summary>
        /// End the effect by disabling the overlay texture.
        /// </summary>
        public override void End()
        {
            GetComponent<GUITexture>().enabled = false;
        }

        /// <summary>
        /// The end of the action has been triggered while playing the Cutscene in reverse.
        /// </summary>
        public override void ReverseEnd()
        {
            GetComponent<GUITexture>().enabled = true;
            GetComponent<GUITexture>().pixelInset = new Rect(0f, 0f, Screen.width, Screen.height);
            GetComponent<GUITexture>().color = To;
        }

        /// <summary>
        /// Disable the overlay texture
        /// </summary>
        public override void Stop()
        {
            if (GetComponent<GUITexture>() != null)
            {
                GetComponent<GUITexture>().enabled = false;
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
            GetComponent<GUITexture>().color = Color.Lerp(from, to, transition);
        }

    }
}
