using Fie.Manager;
using Fie.Object;
using GameDataEditor;
using System;
using UnityEngine;

namespace Fie.Ponies.RainbowDash
{
	public class FieStateMachineRainbowDashEvasion : FieStateMachineGameCharacterBase
	{
		private const float EVASION_SHIELD_REGEN_DELAY = 1.5f;

		private const float EVASION_DEFAULT_SHILED_COST = 0.03f;

		private Type _nextState;

		private bool _isEnd;

		public override void updateState<T>(ref T gameCharacter)
		{
			if (!_isEnd && gameCharacter is FieRainbowDash)
			{
				FieRainbowDash fieRainbowDash = gameCharacter as FieRainbowDash;
				float num = Vector3.Dot(fieRainbowDash.externalInputVector.normalized, Vector3.up);
				float num2 = Vector3.Dot(fieRainbowDash.externalInputVector.normalized, fieRainbowDash.flipDirectionVector);
				if (fieRainbowDash.externalInputForce > 0.25f)
				{
					if (num > 0.7f)
					{
						_nextState = typeof(FieStateMachineRainbowDashEvasionUp);
					}
					else if (num <= -0.7f && fieRainbowDash.groundState == FieObjectGroundState.Flying)
					{
						_nextState = typeof(FieStateMachineRainbowDashEvasionDown);
					}
					else if (num2 > 0.7f)
					{
						_nextState = typeof(FieStateMachineRainbowDashEvasionFront);
					}
				}
				if (_nextState == null)
				{
					_nextState = typeof(FieStateMachineRainbowDashEvasionBack);
				}
				fieRainbowDash.damageSystem.calcShieldDirect((0f - fieRainbowDash.healthStats.maxShield) * 0.03f);
				fieRainbowDash.damageSystem.setRegenerateDelay(Mathf.Max(1.5f, fieRainbowDash.healthStats.regenerateDelay));
				fieRainbowDash.SetDialog(FieMasterData<GDEWordScriptTriggerTypeData>.I.GetMasterData(GDEItemKeys.WordScriptTriggerType_WS_TRIGGER_TYPE_EVADED), 25);
				FieManagerBehaviour<FieActivityManager>.I.RequestLobbyOnlyActivity(gameCharacter, FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_ACTIVITY_TITLE_ELE_LOYALTY_EVADE), FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_ACTIVITY_NOTE_ELE_LOYALTY_EVADE));
				_isEnd = true;
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

		public override bool isNotNetworkSync()
		{
			return true;
		}
	}
}
