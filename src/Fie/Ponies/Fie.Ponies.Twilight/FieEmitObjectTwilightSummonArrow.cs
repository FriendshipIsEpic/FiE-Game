using Fie.Manager;
using Fie.Object;
using GameDataEditor;
using ParticlePlayground;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Ponies.Twilight
{
	[FiePrefabInfo("Prefabs/Twilight/Power/TwilightSummonArrow")]
	public class FieEmitObjectTwilightSummonArrow : FieEmittableObjectBase
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
			if (base.ownerCharacter != null)
			{
				GDESkillTreeData skill = base.ownerCharacter.GetSkill(FieConstValues.FieSkill.MAGIC_SUMMON_ARROW_LV1_2);
				if (skill != null)
				{
					SummonArrowEmitNum += (int)skill.Value1;
				}
				else
				{
					skill = base.ownerCharacter.GetSkill(FieConstValues.FieSkill.MAGIC_SUMMON_ARROW_LV3_2);
					if (skill != null)
					{
						SummonArrowEmitNum += (int)skill.Value1;
					}
				}
			}
			SummonArrowEmitInterval = SummonArrowDuration / (float)SummonArrowEmitNum;
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
					FieManagerBehaviour<FieActivityManager>.I.RequestLobbyOnlyActivity(base.ownerCharacter, FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_ACTIVITY_TITLE_ELE_MAGIC_ABILITY_2), FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_ACTIVITY_NOTE_ELE_MAGIC_ABILITY_2));
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
			if (base.ownerCharacter != null && base.ownerCharacter.GetSkill(FieConstValues.FieSkill.MAGIC_SUMMON_ARROW_LV4_STUNNING_ARROW) != null)
			{
				FieEmitObjectTwilightStunningSummonArrowChild fieEmitObjectTwilightStunningSummonArrowChild = FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectTwilightStunningSummonArrowChild>(base.transform, directionalVec, transform, base.ownerCharacter);
				if (fieEmitObjectTwilightStunningSummonArrowChild != null)
				{
					fieEmitObjectTwilightStunningSummonArrowChild.childId = _emitCount;
				}
			}
			else
			{
				FieEmitObjectTwilightSummonArrowChild fieEmitObjectTwilightSummonArrowChild = FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectTwilightSummonArrowChild>(base.transform, directionalVec, transform, base.ownerCharacter);
				if (fieEmitObjectTwilightSummonArrowChild != null)
				{
					fieEmitObjectTwilightSummonArrowChild.childId = _emitCount;
				}
			}
			_emitCount++;
		}
	}
}
