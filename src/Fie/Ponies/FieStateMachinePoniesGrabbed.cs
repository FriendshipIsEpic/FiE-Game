using Fie.Object;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Ponies
{
	public class FieStateMachinePoniesGrabbed : FieStateMachineGameCharacterBase
	{
		private enum GrabbedState
		{
			STATE_PREPARE,
			STATE_GRABBED,
			STATE_GRABBED_END
		}

		private const float MAXIMUM_GRABBED_TIME = 6f;

		private GrabbedState _staggerState;

		private float _totalTime;

		private bool _isEnd;

		private Type _nextState = typeof(FieStateMachineCommonIdle);

		private bool _isReleased;

		private Transform _anchorTransform;

		private Transform _anchorCharacterCenterTransform;

		private Quaternion _initialRotation = Quaternion.identity;

		private FieGameCharacter _grabbingCharacter;

		public void SetReleaseState(bool isReleased)
		{
			_isReleased = isReleased;
		}

		public void SetAnchorTransform(Transform anchorTransform, Transform anchorCharacterCenterTransform)
		{
			_anchorTransform = anchorTransform;
			_anchorCharacterCenterTransform = anchorCharacterCenterTransform;
		}

		public override void updateState<T>(ref T gameCharacter)
		{
			if (gameCharacter is FiePonies)
			{
				FiePonies fiePonies = gameCharacter as FiePonies;
				_totalTime += Time.deltaTime;
				switch (_staggerState)
				{
				case GrabbedState.STATE_PREPARE:
				{
					TrackEntry trackEntry = fiePonies.animationManager.SetAnimation(14, isLoop: true, isForceSet: true);
					_staggerState = GrabbedState.STATE_GRABBED;
					break;
				}
				case GrabbedState.STATE_GRABBED:
					if (_anchorTransform != null && _anchorCharacterCenterTransform != null)
					{
						fiePonies.position = _anchorTransform.position;
						fiePonies.transform.rotation = _anchorTransform.rotation;
						fiePonies.setFlipByVector((_anchorCharacterCenterTransform.position - fiePonies.transform.position).normalized);
					}
					if (_isReleased || _totalTime > 6f)
					{
						_nextState = typeof(FieStateMachinePoniesStagger);
						_isEnd = true;
					}
					break;
				}
			}
		}

		public override void initialize(FieGameCharacter gameCharacter)
		{
			FiePonies fiePonies = gameCharacter as FiePonies;
			if (!(fiePonies == null))
			{
				fiePonies.emotionController.StopAutoAnimation();
				fiePonies.isEnableAutoFlip = false;
				fiePonies.isEnableCollider = false;
				fiePonies.isEnableGravity = false;
				fiePonies.isEnableHeadTracking = false;
				fiePonies.isSpeakable = false;
				fiePonies.resetMoveForce();
				_initialRotation = fiePonies.transform.rotation;
			}
		}

		public override void terminate(FieGameCharacter gameCharacter)
		{
			FiePonies fiePonies = gameCharacter as FiePonies;
			if (!(fiePonies == null))
			{
				fiePonies.emotionController.RestartAutoAnimation();
				fiePonies.setGravityRate(1f);
				fiePonies.isEnableAutoFlip = true;
				fiePonies.isEnableGravity = true;
				fiePonies.isEnableCollider = true;
				fiePonies.isEnableHeadTracking = true;
				fiePonies.isSpeakable = true;
				fiePonies.transform.rotation = _initialRotation;
			}
		}

		public override bool isEnd()
		{
			return _isEnd;
		}

		public override Type getNextState()
		{
			return _nextState;
		}

		public override List<Type> getAllowedStateList()
		{
			List<Type> list = new List<Type>();
			list.Add(typeof(FieStateMachinePoniesStagger));
			return list;
		}
	}
}
