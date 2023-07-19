using Fie.Object;
using UnityEngine;

namespace Fie.Ponies.Twilight
{
	[FiePrefabInfo("Prefabs/Twilight/Power/TwilightForceFieldEmitEffects")]
	public class FieEmitObjectTwilightForceFieldEmitEffect : FieEmittableObjectBase
	{
		[SerializeField]
		private float duration = 3f;

		public override void awakeEmitObject()
		{
			destoryEmitObject(duration);
		}

		private void Update()
		{
		}
	}
}
