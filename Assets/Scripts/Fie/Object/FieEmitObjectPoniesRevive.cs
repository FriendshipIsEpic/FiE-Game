using UnityEngine;

namespace Fie.Object
{
	[FiePrefabInfo("Prefabs/PlayerCommon/FiePoniesRevive")]
	public class FieEmitObjectPoniesRevive : FieEmittableObjectBase
	{
		[SerializeField]
		private float REVIVE_EFFECT_DURATION = 2.5f;

		public override void awakeEmitObject()
		{
			destoryEmitObject(REVIVE_EFFECT_DURATION);
		}
	}
}
