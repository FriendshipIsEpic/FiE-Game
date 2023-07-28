// Cinema Suite
using CinemaDirector.Helpers;
using System.Collections.Generic;
using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// Utility action for disabling a lot of GameObjects at once and then re-enabling them at the end of the action.
    /// </summary>
    [CutsceneItem("Utility", "Mass Disabler", CutsceneItemGenre.GlobalItem)]
    public class MassDisabler : CinemaGlobalAction, IRevertable
    {
        // Game Objects to be disabled temporarily.
        public List<GameObject> GameObjects = new List<GameObject>();

        // Game Object Tags to be disabled temporarily.
        public List<string> Tags = new List<string>();

        // Cache the game objects.
        private List<GameObject> tagsCache = new List<GameObject>();

        // Options for reverting in editor.
        [SerializeField]
        private RevertMode editorRevertMode = RevertMode.Revert;

        // Options for reverting during runtime.
        [SerializeField]
        private RevertMode runtimeRevertMode = RevertMode.Revert;

        /// <summary>
        /// Cache the initial state of the target GameObject's active state.
        /// </summary>
        /// <returns>The Info necessary to revert this event.</returns>
        public RevertInfo[] CacheState()
        {
            List<GameObject> gameObjects = new List<GameObject>();
            foreach (string tag in Tags)
            {
                GameObject[] tagged = GameObject.FindGameObjectsWithTag(tag);
                foreach (GameObject gameObject in tagged)
                {
                    gameObjects.Add(gameObject);
                }
            }

            gameObjects.AddRange(GameObjects);

            List<RevertInfo> reverts = new List<RevertInfo>();
            foreach (GameObject go in gameObjects)
            {
                if (go != null)
                {
                    reverts.Add(new RevertInfo(this, go, "SetActive", go.activeInHierarchy));
                }
            }

            return reverts.ToArray();
        }

        /// <summary>
        /// Trigger this Action and disable all Game Objects
        /// </summary>
        public override void Trigger()
        {
            tagsCache.Clear();
            foreach (string tag in Tags)
            {
                GameObject[] gameObjects = GameObject.FindGameObjectsWithTag(tag);
                foreach (GameObject gameObject in gameObjects)
                {
                    tagsCache.Add(gameObject);
                }
            }

            setActive(false);
        }

        /// <summary>
        /// End the action and re-enable all game objects.
        /// </summary>
        public override void End()
        {
            setActive(true);
        }

        /// <summary>
        /// Trigger the beginning of the action while playing in reverse.
        /// </summary>
        public override void ReverseTrigger()
        {
            End();
        }

        /// <summary>
        /// Trigger the end of the action while playing in reverse.
        /// </summary>
        public override void ReverseEnd()
        {
            Trigger();
        }

        /// <summary>
        /// Enable/Disable all the game objects.
        /// </summary>
        /// <param name="enabled">Enable or Disable</param>
        private void setActive(bool enabled)
        {
            // Enable gameobjects
            foreach (GameObject gameObject in GameObjects)
            {
                gameObject.SetActive(enabled);
            }

            // Enable tags
            foreach (GameObject gameObject in tagsCache)
            {
                gameObject.SetActive(enabled);
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