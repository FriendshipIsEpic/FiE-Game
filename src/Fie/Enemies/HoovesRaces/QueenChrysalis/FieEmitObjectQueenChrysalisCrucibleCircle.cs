using Fie.Manager;
using Fie.Object;
using Fie.Utility;
using ParticlePlayground;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Enemies.HoovesRaces.QueenChrysalis
{
	[FiePrefabInfo("Prefabs/Enemies/ChangelingForces/QueenChrysalis/Power/QueenChrysalisCrucibleCircle")]
	public class FieEmitObjectQueenChrysalisCrucibleCircle : FieEmittableObjectBase
	{
		public delegate void hitDelegate(Vector3 point);

		[SerializeField]
		private float CIRCLE_SCALING_DURATION = 1.5f;

		[SerializeField]
		private float CIRCLE_DESTROY_DURATION = 1f;

		[SerializeField]
		private float START_SCALE = 0.2f;

		[SerializeField]
		private float END_SCALE = 3f;

		[SerializeField]
		private List<Transform> _scaledTransform = new List<Transform>();

		[SerializeField]
		private PlaygroundParticlesC _flameParticle;

		private float _lifeTimeCount;

		private bool _isEndTrail;

		private bool _isEndUpdate;

		private Tweener<TweenTypesInOutSine> _scaleTweener = new Tweener<TweenTypesInOutSine>();

		public event hitDelegate hitEvent;

		public override void awakeEmitObject()
		{
			_scaleTweener.InitTweener(CIRCLE_SCALING_DURATION, START_SCALE, END_SCALE);
			if (_flameParticle != null)
			{
				_flameParticle.emit = true;
			}
			ApplyScaleToScaledTransforms(START_SCALE);
		}

		public void Update()
		{
			if (!_isEndUpdate)
			{
				_lifeTimeCount += Time.deltaTime;
				float scaleXZ = _scaleTweener.UpdateParameterFloat(Time.deltaTime);
				ApplyScaleToScaledTransforms(scaleXZ);
				if (_lifeTimeCount >= CIRCLE_SCALING_DURATION)
				{
					_isEndUpdate = true;
					destoryEmitObject(CIRCLE_DESTROY_DURATION);
					if (_flameParticle != null)
					{
						_flameParticle.emit = false;
					}
				}
			}
		}

		public void ApplyScaleToScaledTransforms(float scaleXZ)
		{
			if (_scaledTransform.Count > 0)
			{
				foreach (Transform item in _scaledTransform)
				{
					item.localScale = new Vector3(scaleXZ, scaleXZ, 1f);
				}
			}
		}

		private void OnTriggerEnter(Collider collider)
		{
			if (!_isEndUpdate && !(_lifeTimeCount > CIRCLE_SCALING_DURATION) && collider.gameObject.tag == getHostileTagString())
			{
				FieGameCharacter fieGameCharacter = addDamageToCollisionCharacter(collider, getDefaultDamageObject());
				if (fieGameCharacter != null)
				{
					Vector3 vector = collider.ClosestPointOnBounds(base.transform.position);
					FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectQueenChrysalisHitEffectSmall>(base.transform, Vector3.zero).transform.position = vector;
					FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectQueenChrysalisHitEffectBurned>(fieGameCharacter.centerTransform, Vector3.zero);
					if (this.hitEvent != null)
					{
						this.hitEvent(vector);
					}
				}
			}
		}
	}
}
