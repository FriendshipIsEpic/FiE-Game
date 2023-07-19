using Fie.Manager;
using Fie.Object;
using Fie.Utility;
using UnityEngine;

namespace Fie.Ponies.RainbowDash
{
	[FiePrefabInfo("Prefabs/RainbowDash/Power/RainbowDashDoublePaybackCollision")]
	public class FieEmitObjectRainbowDashDoublePaybackCollision : FieEmittableObjectBase
	{
		[SerializeField]
		private float RAINBOW_DASH_DOUBLE_PAYBACK_DURATION = 0.6f;

		[SerializeField]
		private float RAINBOW_DASH_DOUBLE_PAYBACK_DAMAGE_DURATION = 0.5f;

		[SerializeField]
		private float RAINBOW_DASH_DOUBLE_PAYBACK_PUSH_FORCE = 7f;

		private float _lifeTimeCount;

		private bool _isEndTrail;

		private bool _isEndUpdate;

		public void Update()
		{
			if (initTransform != null)
			{
				base.transform.position = initTransform.position;
			}
			base.transform.localRotation = Quaternion.identity;
			if (!_isEndUpdate)
			{
				_lifeTimeCount += Time.deltaTime;
				if (_lifeTimeCount >= RAINBOW_DASH_DOUBLE_PAYBACK_DURATION)
				{
					_isEndUpdate = true;
					destoryEmitObject();
				}
			}
		}

		private void OnTriggerEnter(Collider collider)
		{
			if (!_isEndUpdate && !(_lifeTimeCount > RAINBOW_DASH_DOUBLE_PAYBACK_DAMAGE_DURATION) && collider.gameObject.tag == getHostileTagString())
			{
				FieGameCharacter fieGameCharacter = addDamageToCollisionCharacter(collider, getDefaultDamageObject());
				if (fieGameCharacter != null)
				{
					FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectRainbowDashDoublePaybackHitEffect>(base.transform, directionalVec);
					FieManagerBehaviour<FieGameCameraManager>.I.gameCamera.setWiggler(Wiggler.WiggleTemplate.WIGGLE_TYPE_MIDDLE);
					fieGameCharacter.resetMoveForce();
					fieGameCharacter.setMoveForce(directionalVec * RAINBOW_DASH_DOUBLE_PAYBACK_PUSH_FORCE, 0f, useRound: false);
				}
			}
		}
	}
}
