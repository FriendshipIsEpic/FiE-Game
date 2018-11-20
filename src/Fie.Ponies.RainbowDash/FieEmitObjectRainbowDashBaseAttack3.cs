using Fie.Manager;
using Fie.Object;
using Fie.Utility;
using PigeonCoopToolkit.Effects.Trails;
using UnityEngine;

namespace Fie.Ponies.RainbowDash
{
	[FiePrefabInfo("Prefabs/RainbowDash/Power/RainbowDashBaseAttack3")]
	public class FieEmitObjectRainbowDashBaseAttack3 : FieEmittableObjectBase
	{
		[SerializeField]
		private float DURATION = 0.3f;

		[SerializeField]
		private float TRAIL_DURATION = 0.4f;

		[SerializeField]
		private float DAMAGE_DURATION = 0.2f;

		[Range(0f, 4f)]
		[SerializeField]
		private int GAIND_AWESOME_COUNT = 1;

		[SerializeField]
		private float RAINBOW_DASH_BASE_ATTACK_PUSH_FORCE = 4f;

		[SerializeField]
		private SmoothTrail _airKickTrail;

		private float _lifeTimeCount;

		private bool _isEndUpdate;

		public override void awakeEmitObject()
		{
			_airKickTrail.Emit = true;
		}

		public void Update()
		{
			if (!_isEndUpdate)
			{
				_lifeTimeCount += Time.deltaTime;
				if (_lifeTimeCount >= DURATION)
				{
					_isEndUpdate = true;
					destoryEmitObject();
				}
				if (_lifeTimeCount >= TRAIL_DURATION)
				{
					_airKickTrail.Emit = false;
				}
				if (initTransform != null)
				{
					base.transform.position = initTransform.position;
				}
			}
		}

		private void OnTriggerEnter(Collider collider)
		{
			if (!_isEndUpdate && !(_lifeTimeCount > DAMAGE_DURATION) && collider.gameObject.tag == getHostileTagString())
			{
				FieGameCharacter fieGameCharacter = addDamageToCollisionCharacter(collider, getDefaultDamageObject());
				if (fieGameCharacter != null)
				{
					FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectRainbowDashHitEffectMiddle>(base.transform, collider.ClosestPointOnBounds(base.transform.position));
					FieManagerBehaviour<FieGameCameraManager>.I.gameCamera.setWiggler(Wiggler.WiggleTemplate.WIGGLE_TYPE_MIDDLE);
					if (base.ownerCharacter != null)
					{
						FieRainbowDash fieRainbowDash = base.ownerCharacter as FieRainbowDash;
						if (fieRainbowDash != null)
						{
							fieRainbowDash.requestSetAwesomeEffect(GAIND_AWESOME_COUNT);
						}
					}
					fieGameCharacter.setMoveForce(directionalVec * RAINBOW_DASH_BASE_ATTACK_PUSH_FORCE, 0f, useRound: false);
					destoryEmitObject(DURATION - _lifeTimeCount);
					_isEndUpdate = true;
				}
			}
		}
	}
}
