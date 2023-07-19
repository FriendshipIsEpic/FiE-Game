using UnityEngine;

namespace Fie.Object
{
	[FiePrefabInfo("Prefabs/PlayerCommon/FiePoniesGainedFriendship")]
	public class FieEmitObjectPoniesGainedFriendship : FieEmittableObjectBase
	{
		[SerializeField]
		private float GAINED_EFFECT_DURATION = 2.5f;

		public override void awakeEmitObject()
		{
			destoryEmitObject(GAINED_EFFECT_DURATION);
		}
	}
}
