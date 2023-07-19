using Fie.Manager;
using Fie.Object;
using Fie.Utility;
using GameDataEditor;
using System;
using UnityEngine;

namespace Fie.Ponies.Twilight
{
	[FieAbilityID(FieConstValues.FieAbility.MAGIC_BUBBLE)]
	public class FieStateMachineTwilightForceField : FieStateMachineAbilityBase
	{
		private const string FORCE_FIELD_SIGNATURE = "magic_bubble";

		private const float FORCE_FIELD_DELAY = 0.3f;

		private const float FORCE_FIELD_DEFAULT_COOLDOWN = 60f;

		private bool _isEnd;

		public override void updateState<T>(ref T gameCharacter)
		{
			if (!_isEnd && gameCharacter is FieTwilight)
			{
				FieTwilight fieTwilight = gameCharacter as FieTwilight;
				if (!FieTwilight.ignoreAttackState.Contains(fieTwilight.getStateMachine().nowStateType()))
				{
					Vector3 vector = (fieTwilight.flipState != 0) ? Vector3.right : Vector3.left;
					if (fieTwilight.getStateMachine().nowStateType() == typeof(FieStateMachineCommonIdle) || fieTwilight.getStateMachine().nowStateType() == typeof(FieStateMachinePoniesIdle) || fieTwilight.getStateMachine().nowStateType() == typeof(FieStateMachineTwilightFireSmall))
					{
						if (fieTwilight.groundState == FieObjectGroundState.Grounding)
						{
							fieTwilight.getStateMachine().setState(typeof(FieStateMachineTwilightFireSmall), isForceSet: true, isDupulicate: true);
						}
						fieTwilight.physicalForce.SetPhysicalForce(vector * -1f + Vector3.up * FieRandom.Range(-0.2f, 0.2f), 5000f, 0.1f);
					}
					FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectTwilightForceField>(fieTwilight.hornTransform, vector, null, fieTwilight);
					FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectTwilightSpellEffect>(fieTwilight.hornTransform, Vector3.zero, null);
					fieTwilight.SetDialog(100, FieMasterData<GDEWordScriptsListData>.I.GetMasterData(GDEItemKeys.WordScriptsList_P_MAGIC_ABILITY_MAGIC_BUBBLE_1), FieMasterData<GDEWordScriptsListData>.I.GetMasterData(GDEItemKeys.WordScriptsList_P_MAGIC_ABILITY_MAGIC_BUBBLE_2));
					FieManagerBehaviour<FieActivityManager>.I.RequestLobbyOnlyActivity(gameCharacter, FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_ACTIVITY_TITLE_ELE_MAGIC_ABILITY_3), FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_ACTIVITY_NOTE_ELE_MAGIC_ABILITY_3));
					float cooldown = 60f;
					fieTwilight.abilitiesContainer.SetCooldown<FieStateMachineTwilightForceField>(cooldown);
					_isEnd = true;
				}
			}
		}

		public override float getDelay()
		{
			return 0.3f;
		}

		public override string getSignature()
		{
			return "magic_bubble";
		}

		public override void initialize(FieGameCharacter gameCharacter)
		{
			defaultCoolDown = 60f;
		}

		public override bool isEnd()
		{
			return _isEnd;
		}

		public override Type getNextState()
		{
			return typeof(FieStateMachinePoniesAttackIdle);
		}

		public override FieAbilityActivationType getActivationType()
		{
			return FieAbilityActivationType.COOLDOWN;
		}
	}
}
