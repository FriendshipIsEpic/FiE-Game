using UnityEngine;

namespace CinemaDirector
{
    [CutsceneItemAttribute("Transform", "Set Parent", CutsceneItemGenre.ActorItem)]
    public class SetParent : CinemaActorEvent
    {
        public override void Trigger(GameObject actor)
        {
        }

        public override void Reverse(GameObject actor)
        {
        }
    }
}