using Fie.Manager;
using Fie.Object;
using Fie.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Enemies.HoovesRaces.Changeling
{
	public class FieStateMachineChangelingArrival : FieStateMachineGameCharacterBase
	{
		private enum ArrivalState
		{
			ARRIVAL_START,
			ARRIVING,
			ARRIVAL_END
		}

		private const float POSITION_INITIALIZE_TIME = 0.75f;

		private Tweener<TweenTypesInSine> positionTweener = new Tweener<TweenTypesInSine>();

		private ArrivalState _arrivalState;

		private bool _isEnd;

		private FieEmitObjectChangelingForcesArrivalParticleEffect _arriveEffect;

		public override void initialize(FieGameCharacter gameCharacter)
		{
			if (!(gameCharacter == null))
			{
				gameCharacter.isEnableCollider = false;
				gameCharacter.isEnableGravity = true;
				gameCharacter.isEnableAutoFlip = false;
				gameCharacter.skeletonUtility.skeletonRenderer.GetComponent<MeshRenderer>().enabled = false;
			}
		}

		public override void terminate(FieGameCharacter gameCharacter)
		{
			if (!(gameCharacter == null))
			{
				gameCharacter.isEnableCollider = true;
				gameCharacter.isEnableGravity = true;
				gameCharacter.isEnableAutoFlip = true;
				gameCharacter.skeletonUtility.skeletonRenderer.GetComponent<MeshRenderer>().enabled = true;
				if (_arriveEffect != null)
				{
					_arriveEffect.StopEffect();
				}
			}
		}

		public override void updateState<T>(ref T gameCharacter)
		{
			if (!_isEnd && gameCharacter is FieChangeling)
			{
				FieChangeling fieChangeling = gameCharacter as FieChangeling;
				if (!(fieChangeling == null))
				{
					switch (_arrivalState)
					{
					case ArrivalState.ARRIVAL_START:
						_arrivalState = ArrivalState.ARRIVING;
						fieChangeling.position += Vector3.up * 3f;
						fieChangeling.groundState = FieObjectGroundState.Flying;
						_arriveEffect = FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectChangelingForcesArrivalParticleEffect>(fieChangeling.centerTransform, Vector3.zero, null);
						break;
					case ArrivalState.ARRIVING:
						if (fieChangeling.groundState == FieObjectGroundState.Grounding)
						{
							FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectChangelingForcesArrivalFireEffect>(fieChangeling.transform, Vector3.zero, null);
							_isEnd = true;
						}
						break;
					}
				}
			}
		}

		public override bool isEnd()
		{
			return _isEnd;
		}

		public override Type getNextState()
		{
			return typeof(FieStateMachineCommonIdle);
		}

		public override List<Type> getAllowedStateList()
		{
			return new List<Type>();
		}
	}
}
