using Fie.Manager;
using Fie.Object;
using GameDataEditor;
using ParticlePlayground;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Ponies.RisingSun
{
	[FiePrefabInfo("Prefabs/RisingSun/Power/RisingSunSummonArrow")]
	public class FieEmitObjectRisingSunSummonArrow : FieEmittableObjectBase
	{
		[SerializeField]
		private float SummonArrowDuration = 1.5f;

		[SerializeField]
		private float SummonArrowEmitDuration = 1f;

		[SerializeField]
		private int SummonArrowEmitNum = 10;

		[SerializeField]
		private float SummonArrowEmitInterval = 0.1f;

		[SerializeField]
		private FieDetector _emitPointDetector;

		[SerializeField]
		private List<PlaygroundParticlesC> _childParticles = new List<PlaygroundParticlesC>();

		private float _lifeTimeCount;

		private float _emitTimeCount;

		private float _emitInterval;

		private int _emitCount;

		private bool _isEndUpdate;

		private bool _isEndEmit;

		public override void awakeEmitObject()
		{
			_childParticles.ForEach(delegate(PlaygroundParticlesC particle)
			{
				particle.Emit(setEmission: true);
			});
			_emitPointDetector.Initialize();
			_emitTimeCount = SummonArrowEmitInterval;
		}

		public void Update()
		{
			if (!_isEndUpdate)
			{
				_lifeTimeCount += Time.deltaTime;
				_emitTimeCount += Time.deltaTime;
				if (_emitTimeCount >= SummonArrowEmitInterval && _emitCount < SummonArrowEmitNum)
				{
					FieManagerBehaviour<FieActivityManager>.I.RequestLobbyOnlyActivity(base.ownerCharacter, FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_ACTIVITY_TITLE_RISING_SUN_ABILITY_2), FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_ACTIVITY_NOTE_RISING_SUN_ABILITY_2));
					emitChild();
					_emitTimeCount = 0f;
				}
				if (_lifeTimeCount > SummonArrowEmitDuration && !_isEndEmit)
				{
					_childParticles.ForEach(delegate(PlaygroundParticlesC particle)
					{
						particle.Emit(setEmission: false);
					});
					_isEndEmit = true;
				}
				if (_lifeTimeCount >= SummonArrowDuration)
				{
					destoryEmitObject();
				}
			}
		}

		private void emitChild()
		{
			Transform transform = null;
			Vector3 directionalVec = base.directionalVec;
			if (_emitPointDetector != null)
			{
				transform = _emitPointDetector.getRandomEnemyTransform(isCenter: true);
				if (transform != null)
				{
					directionalVec = Quaternion.LookRotation(transform.position - base.transform.position) * Vector3.forward;
				}
			}
			FieEmitObjectRisingSunSummonArrowChild fieEmitObjectRisingSunSummonArrowChild = FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectRisingSunSummonArrowChild>(base.transform, directionalVec, transform, base.ownerCharacter);
			if (fieEmitObjectRisingSunSummonArrowChild != null)
			{
				fieEmitObjectRisingSunSummonArrowChild.childId = _emitCount;
			}
			_emitCount++;
		}
	}
}
