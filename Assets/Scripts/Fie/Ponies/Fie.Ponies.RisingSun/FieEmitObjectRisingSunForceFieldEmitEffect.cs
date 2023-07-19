using Fie.Object;
using UnityEngine;

namespace Fie.Ponies.RisingSun
{
	[FiePrefabInfo("Prefabs/RisingSun/Power/RisingSunForceFieldEmitEffects")]
	public class FieEmitObjectRisingSunForceFieldEmitEffect : FieEmittableObjectBase
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
