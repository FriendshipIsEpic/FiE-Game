using Fie.Enemies;
using Fie.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Object
{
	public class FieStateMachineStatusEffectPull : FieStateMachineGameCharacterBase
	{
		private enum PullState
		{
			PULL_START,
			PULLING,
			PULL_END
		}

		private Tweener<TweenTypesInOutSine> _pullTweener = new Tweener<TweenTypesInOutSine>();

		private Vector3 _pullPoint = Vector3.zero;

		private PullState _pullState;

		private float _duration;

		public override void initialize(FieGameCharacter gameCharacter)
		{
			gameCharacter.isEnableAutoFlip = false;
		}

		public override void terminate(FieGameCharacter gameCharacter)
		{
			gameCharacter.isEnableAutoFlip = true;
		}

		public void setDuration(float duration)
		{
			_duration = duration;
			_pullTweener.InitTweener(_duration, _pullTweener.getStartParamVec3(), _pullPoint);
		}

		public void setPullPoint(Vector3 point)
		{
			_pullPoint = point;
			_pullTweener.InitTweener(_duration, _pullTweener.getStartParamVec3(), _pullPoint);
		}

		public override void updateState<T>(ref T gameCharacter)
		{
			switch (_pullState)
			{
			case PullState.PULL_START:
				if (gameCharacter is FieObjectEnemies)
				{
					gameCharacter.animationManager.SetAnimation(3, isLoop: false, isForceSet: true);
				}
				_pullTweener.InitTweener(_duration, gameCharacter.position, _pullPoint);
				_pullState = PullState.PULLING;
				break;
			case PullState.PULLING:
				if (_pullPoint != Vector3.zero)
				{
					Vector3 vector = _pullTweener.UpdateParameterVec3(Time.deltaTime);
					gameCharacter.currentMovingVec += (vector - gameCharacter.position) * (1f / Time.deltaTime);
					gameCharacter.GetComponent<Rigidbody>().MovePosition(vector);
				}
				break;
			}
			_duration -= Time.deltaTime;
		}

		public override bool isEnd()
		{
			return _duration <= 0f;
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
