using Fie.Object;
using UnityEngine;

namespace Fie.Enemies.HoovesRaces.ChangelingAlpha
{
	[FiePrefabInfo("Prefabs/Enemies/ChangelingForces/ChangelingAlpha/Power/ChangelingAlphaChargeEffect")]
	public class FieEmitObjectChangelingAlphaChargeEffect : FieEmittableObjectBase
	{
		private const float DURATION = 1f;

		public override void awakeEmitObject()
		{
			if (initTransform != null)
			{
				base.transform.position = initTransform.position;
			}
			base.transform.rotation = Quaternion.LookRotation(directionalVec);
			destoryEmitObject(1f);
		}
	}
}
