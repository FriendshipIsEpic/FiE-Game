using Fie.Object;

namespace Fie.Ponies.Applejack
{
	[FiePrefabInfo("Prefabs/Applejack/Power/ApplejackKickEffect2")]
	public class FieEmitObjectApplejackKickEffect2 : FieEmittableObjectBase
	{
		private const float DURATION = 1f;

		public override void awakeEmitObject()
		{
			destoryEmitObject(1f);
		}

		private void Update()
		{
			if (initTransform != null)
			{
				base.transform.position = initTransform.position;
			}
		}
	}
}
