using Fie.Object;
using Fie.Utility;
using Spine;
using System;
using UnityEngine;

namespace Fie.Ponies.RisingSun
{
	public class FieStateMachineRisingSunFireBig : FieStateMachineRisingSunInterface
	{
		private enum FireState
		{
			FIRE_START,
			FIRING
		}

		private const float GROUND_FORCE_TIME = 1.5f;

		private const float GROUND_FORCE = 15f;

		private Type _nextState = typeof(FieStateMachineCommonIdle);

		private FireState _fireState;

		private bool _isEnd;

		private float _endTime = 3.40282347E+38f;

		private float _timeCount;

		private bool _isSetEndAnim;

		private Tweener<TweenTypesOutSine> _groundForceTweener = new Tweener<TweenTypesOutSine>();

		public override void updateState<T>(ref T gameCharacter)
		{
			if (gameCharacter is FieRisingSun)
			{
				_timeCount += Time.deltaTime;
				FieRisingSun fieRisingSun = gameCharacter as FieRisingSun;
				switch (_fireState)
				{
				case FireState.FIRE_START:
				{
					if (fieRisingSun.groundState == FieObjectGroundState.Grounding)
					{
						fieRisingSun.animationManager.SetAnimationChain(25, 26, isLoop: true);
					}
					else
					{
						_nextState = typeof(FieStateMachineRisingSunFlying);
					}
					TrackEntry trackEntry = fieRisingSun.animationManager.SetAnimationChain(28, 16, isLoop: true);
					if (trackEntry != null)
					{
						_endTime = trackEntry.endTime;
					}
					_fireState = FireState.FIRING;
					fieRisingSun.isEnableGravity = false;
					break;
				}
				}
				if (_timeCount > _endTime)
				{
					if (fieRisingSun.groundState == FieObjectGroundState.Grounding && !_isSetEndAnim)
					{
						TrackEntry trackEntry2 = fieRisingSun.animationManager.SetAnimation(27);
						if (trackEntry2 != null)
						{
							_endTime += trackEntry2.endTime;
						}
						_isSetEndAnim = true;
					}
					else
					{
						_isEnd = true;
					}
				}
			}
		}

		public override void terminate(FieGameCharacter gameCharacter)
		{
			gameCharacter.isEnableGravity = true;
		}

		public override bool isEnd()
		{
			return _isEnd;
		}

		public override Type getNextState()
		{
			return _nextState;
		}
	}
}
