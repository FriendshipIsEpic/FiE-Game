using Fie.Object;
using UnityEngine;

namespace Fie.Ponies.Applejack
{
	[FiePrefabInfo("Prefabs/Applejack/Power/ApplejackKickEffect")]
	public class FieEmitObjectApplejackKickEffect : FieEmittableObjectBase
	{
		private const float DURATION = 0.6f;

		public override void awakeEmitObject()
		{
			if (initTransform != null)
			{
				base.transform.position = initTransform.position;
			}
			base.transform.rotation = Quaternion.LookRotation(directionalVec);
			destoryEmitObject(0.6f);
		}
	}
}
