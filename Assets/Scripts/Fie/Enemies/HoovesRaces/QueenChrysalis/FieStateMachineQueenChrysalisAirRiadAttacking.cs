using Fie.Manager;
using Fie.Object;
using Fie.Utility;
using GameDataEditor;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;
using Event = Spine.Event;

namespace Fie.Enemies.HoovesRaces.QueenChrysalis
{
	public class FieStateMachineQueenChrysalisAirRiadAttacking : FieStateMachineGameCharacterBase
	{
		private enum AttackingState
		{
			AIR_RAID_ATTACKING_START,
			AIR_RAID_ATTACKING,
			AIR_RAID_ATTACKING_FINISHED
		}

		private const float MAXIMUM_ATTACK_DISTANCE = 20f;

		private const float ATTAKING_DRATION = 0.3f;

		private Type _nextState = typeof(FieStateMachineCommonIdle);

		private AttackingState _fireState;

		private bool _isEnd;

		private bool _isFinished;

		private Tweener<TweenTypesInSine> _attackingTweener = new Tweener<TweenTypesInSine>();

		public override void updateState<T>(ref T gameCharacter)
		{
			FieQueenChrysalis chrysalis = gameCharacter as FieQueenChrysalis;
			if (!(chrysalis == null))
			{
				switch (_fireState)
				{
				case AttackingState.AIR_RAID_ATTACKING_START:
				{
					TrackEntry trackEntry = chrysalis.animationManager.SetAnimation(9, isLoop: false, isForceSet: true);
					chrysalis.SetDialog(100, FieMasterData<GDEWordScriptsListData>.I.GetMasterData(GDEItemKeys.WordScriptsList_E_THE_INSECT_QUEEN_USING_ABILITY_1), FieMasterData<GDEWordScriptsListData>.I.GetMasterData(GDEItemKeys.WordScriptsList_E_THE_INSECT_QUEEN_USING_ABILITY_2));
					if (trackEntry != null)
					{
						trackEntry.Event += delegate(TrackEntry state, Event trackIndex)
						{
							if (trackIndex.Data.Name == "move")
							{
								InitializeAttackingTweener(chrysalis);
								_fireState = AttackingState.AIR_RAID_ATTACKING_FINISHED;
							}
						};
					}
					else
					{
						_isEnd = true;
					}
					_fireState = AttackingState.AIR_RAID_ATTACKING;
					break;
				}
				case AttackingState.AIR_RAID_ATTACKING_FINISHED:
					if (!_attackingTweener.IsEnd())
					{
						chrysalis.position = _attackingTweener.UpdateParameterVec3(Time.deltaTime);
					}
					else
					{
						FieManagerBehaviour<FieGameCameraManager>.I.gameCamera.setWiggler(0.4f, 10, new Vector3(0.05f, 0.3f));
						_nextState = typeof(FieStateMachineQueenChrysalisAirRaidFinished);
						_isEnd = true;
					}
					break;
				}
			}
		}

		private void InitializeAttackingTweener(FieQueenChrysalis chrysalis)
		{
			if (chrysalis == null)
			{
				_isEnd = true;
			}
			else
			{
				Vector3 vector = chrysalis.position;
				Vector3 vector2 = Vector3.down;
				FieGameCharacter lockonTargetObject = chrysalis.detector.lockonTargetObject;
				if (lockonTargetObject != null)
				{
					vector2 = (lockonTargetObject.centerTransform.position - chrysalis.centerTransform.position).normalized;
					vector2.y = -0.25f;
					vector2 = vector2.normalized;
				}
				int layerMask = 1049088;
				if (Physics.Raycast(chrysalis.centerTransform.position, vector2, out RaycastHit hitInfo, 20f, layerMask))
				{
					vector = hitInfo.point;
				}
				layerMask = 512;
				if (Physics.Raycast(vector, Vector3.down, out hitInfo, 20f, layerMask) && hitInfo.collider.tag == "Floor")
				{
					vector = hitInfo.point;
				}
				if (vector2 != Vector3.down)
				{
					chrysalis.setFlipByVector(new Vector3(vector2.x, 0f, 0f).normalized);
				}
				_attackingTweener.InitTweener(0.3f, chrysalis.position, vector);
			}
		}

		public override void initialize(FieGameCharacter gameCharacter)
		{
			if (!(gameCharacter == null))
			{
				gameCharacter.emotionController.StopAutoAnimation();
				gameCharacter.isEnableCollider = false;
				gameCharacter.isEnableAutoFlip = false;
				gameCharacter.isEnableGravity = false;
				gameCharacter.resetMoveForce();
				autoFlipToEnemy(gameCharacter);
			}
		}

		public override void terminate(FieGameCharacter gameCharacter)
		{
			if (!(gameCharacter == null))
			{
				gameCharacter.isEnableGravity = true;
				gameCharacter.isEnableAutoFlip = true;
				gameCharacter.isEnableCollider = true;
				gameCharacter.rootBone.transform.localRotation = Quaternion.identity;
				gameCharacter.resetMoveForce();
			}
		}

		public override List<Type> getAllowedStateList()
		{
			return new List<Type>();
		}

		public override bool isEnd()
		{
			return _isEnd;
		}

		public override Type getNextState()
		{
			return _nextState;
		}

		public override bool isFinished()
		{
			return _isFinished;
		}
	}
}
