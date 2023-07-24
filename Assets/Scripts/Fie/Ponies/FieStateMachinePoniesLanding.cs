using Fie.Object;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;
using Event = Spine.Event;

namespace Fie.Ponies
{
	public class FieStateMachinePoniesLanding : FieStateMachineGameCharacterBase
	{
		public enum LandingState
		{
			LANDING_START,
			LANDING
		}

		private const float LANDING_DELAY = 0.5f;

		private bool _isEnd;

		private bool _isFinished;

		private LandingState _landingState;

		private float _landingCount = 0.05f;

		public override void updateState<T>(ref T gameCharacter)
		{
			if (gameCharacter is FiePonies)
			{
				FiePonies fiePonies = gameCharacter as FiePonies;
				Vector3 a = fiePonies.externalInputVector;
				a.y = (a.z = 0f);
				switch (_landingState)
				{
				case LandingState.LANDING_START:
					if (fiePonies.groundState == FieObjectGroundState.Grounding)
					{
						Vector3 externalInputVector = fiePonies.externalInputVector;
						fiePonies.addMoveForce(new Vector3(externalInputVector.x * (500f * fiePonies.externalInputForce), 0f, 0f), 0.5f);
						_landingCount = 0.5f;
						TrackEntry trackEntry = fiePonies.animationManager.SetAnimation(4, isLoop: false, isForceSet: true);
						if (trackEntry != null)
						{
							trackEntry.Event += delegate(TrackEntry state, Event trackIndex)
							{
								if (trackIndex.Data.Name == "finished")
								{
									_isFinished = true;
								}
							};
							trackEntry.Complete += delegate
							{
								_isEnd = true;
							};
						}
						_landingState = LandingState.LANDING;
					}
					break;
				case LandingState.LANDING:
					_landingCount -= Time.deltaTime;
					if (_landingCount <= 0f)
					{
						_isEnd = true;
					}
					a *= 0.5f;
					fiePonies.physicalForce.SetPhysicalForce(Vector3.zero, 0f);
					break;
				}
			}
		}

		public override void initialize(FieGameCharacter gameCharacter)
		{
			if (!(gameCharacter == null))
			{
				gameCharacter.isEnableAutoFlip = true;
			}
		}

		public override void terminate(FieGameCharacter gameCharacter)
		{
			if (!(gameCharacter == null))
			{
				gameCharacter.setGravityRate(1f);
				gameCharacter.isEnableGravity = true;
				gameCharacter.isEnableAutoFlip = true;
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
			if (!_isFinished)
			{
				return new List<Type>();
			}
			List<Type> list = new List<Type>();
			list.Add(typeof(FieStateMachineAnyConsider));
			return list;
		}
	}
}
