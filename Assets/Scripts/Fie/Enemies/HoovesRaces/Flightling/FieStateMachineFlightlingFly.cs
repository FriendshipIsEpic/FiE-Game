using Fie.Object;
using Fie.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Enemies.HoovesRaces.Flightling
{
	public class FieStateMachineFlightlingFly : FieStateMachineGameCharacterBase
	{
		private enum FlightState
		{
			PREPARE,
			CHECK_VELOCITY,
			DO,
			END
		}

		private const float MAX_ARTTITUDE = 2.5f;

		private const float MIN_ARTTITUDE = 0.5f;

		private FlightState state;

		private bool _isEnd;

		private Type _nextState;

		private float _physicalSinWave;

		private Tweener<TweenTypesInOutSine> _moveTweener = new Tweener<TweenTypesInOutSine>();

		public override void updateState<T>(ref T gameCharacter)
		{
			FieFlightling fieFlightling = gameCharacter as FieFlightling;
			if (!(fieFlightling == null))
			{
				switch (state)
				{
				default:
					return;
				case FlightState.PREPARE:
					fieFlightling.animationManager.SetAnimation(8, isLoop: true);
					state = FlightState.DO;
					break;
				case FlightState.DO:
					break;
				}
				autoFlipToEnemy(fieFlightling);
				if (!_moveTweener.IsEnd())
				{
					Vector3 position = _moveTweener.UpdateParameterVec3(Time.deltaTime);
					fieFlightling.GetComponent<Rigidbody>().MovePosition(position);
				}
			}
		}

		public Vector3 calcArtitude(FieGameCharacter gameCharacter, Vector3 normal)
		{
			int layerMask = 512;
			if (Physics.Raycast(gameCharacter.position + Vector3.up * 0.5f, normal, out RaycastHit hitInfo, 1024f, layerMask))
			{
				Vector3 point = hitInfo.point;
				float x = point.x;
				Vector3 point2 = hitInfo.point;
				float y = point2.y + FieRandom.Range(0.5f, 2.5f);
				Vector3 point3 = hitInfo.point;
				return new Vector3(x, y, point3.z);
			}
			return Vector3.zero;
		}

		public void setNextPosition(Vector3 nowPosition, Vector3 nextPosition, float time)
		{
			_moveTweener.InitTweener(time, nowPosition, nextPosition);
		}

		public bool isEndMoving()
		{
			return _moveTweener.IsEnd();
		}

		public override void initialize(FieGameCharacter gameCharacter)
		{
			if (!(gameCharacter == null))
			{
				gameCharacter.isEnableGravity = false;
				gameCharacter.isEnableAutoFlip = true;
			}
		}

		public override void terminate(FieGameCharacter gameCharacter)
		{
			if (!(gameCharacter == null))
			{
				gameCharacter.isEnableGravity = true;
				gameCharacter.isEnableAutoFlip = false;
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
			list.Add(typeof(FieStateMachineAnyConsider));
			return list;
		}
	}
}
