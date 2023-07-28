using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// An event for calling the game object send message method.
    /// Cannot be reversed.
    /// </summary>
    [CutsceneItemAttribute("Game Object", "Send Message", CutsceneItemGenre.ActorItem)]
    public class SendMessageGameObject : CinemaActorEvent
    {
        public string MethodName = string.Empty;
        public object Parameter = null;
        public SendMessageOptions SendMessageOptions = SendMessageOptions.DontRequireReceiver;

        /// <summary>
        /// Trigger this event and send the message.
        /// </summary>
        /// <param name="actor">the actor to send the message to.</param>
        public override void Trigger(GameObject actor)
        {
            if (actor != null)
            {
                actor.SendMessage(MethodName, Parameter, SendMessageOptions);
            }
        }

    }
}