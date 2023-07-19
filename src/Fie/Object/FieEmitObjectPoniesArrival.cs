using ParticlePlayground;
using UnityEngine;

namespace Fie.Object
{
	[FiePrefabInfo("Prefabs/PlayerCommon/FiePlayerArrival")]
	public class FieEmitObjectPoniesArrival : FieEmittableObjectBase
	{
		[SerializeField]
		private float ARRIVAL_EFFECT_DURATION = 3.5f;

		[SerializeField]
		private PlaygroundParticlesC _myPlayGround;

		public override void awakeEmitObject()
		{
			destoryEmitObject(ARRIVAL_EFFECT_DURATION);
		}

		public void SetSubMeshObject(GameObject submeshObject)
		{
			if (!(_myPlayGround == null) && _myPlayGround.manipulators.Count > 0)
			{
				foreach (ManipulatorObjectC manipulator in _myPlayGround.manipulators)
				{
					if (manipulator.type == MANIPULATORTYPEC.Property && manipulator.property.type == MANIPULATORPROPERTYTYPEC.MeshTarget)
					{
						manipulator.property.meshTarget.gameObject = submeshObject;
					}
				}
			}
		}
	}
}
