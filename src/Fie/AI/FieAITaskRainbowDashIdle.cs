using Fie.Manager;
using Fie.Object;
using Fie.Ponies.RainbowDash;
using UnityEngine;

namespace Fie.AI
{
	public class FieAITaskRainbowDashIdle : FieAITaskBase
	{
		private FieGameCharacter injuryCharacter;

		public override void Initialize(FieAITaskController manager)
		{
			if (Random.Range(0f, 100f) > 50f)
			{
				FieGameCharacter randomEnemyGameCharacter = manager.ownerCharacter.detector.getRandomEnemyGameCharacter();
				if (randomEnemyGameCharacter != null)
				{
					manager.ownerCharacter.detector.ChangeLockonTargetByInstanceID(randomEnemyGameCharacter.GetInstanceID());
				}
			}
			injuryCharacter = FieManagerBehaviour<FieInGameCharacterStatusManager>.I.GetNearbyInjuryAllyCharacter(manager.ownerCharacter);
		}

		public override bool Task(FieAITaskController manager)
		{
			FieRainbowDash fieRainbowDash = manager.ownerCharacter as FieRainbowDash;
			if (fieRainbowDash.damageSystem.isDead)
			{
				if (fieRainbowDash.friendshipStats.getCurrentFriendshipPoint() >= 3)
				{
					fieRainbowDash.TryToRevive();
				}
				return true;
			}
			if (injuryCharacter != null && fieRainbowDash.friendshipStats.getCurrentFriendshipPoint() >= 2)
			{
				nextStateWeightList[typeof(FieAITaskPoniesRescue)] = 100;
				return true;
			}
			float num = Vector3.Distance(fieRainbowDash.transform.position, FieManagerBehaviour<FieUserManager>.I.gameOwnerCharacter.transform.position);
			if (fieRainbowDash.detector.lockonTargetObject == null)
			{
				if (num > 2.5f)
				{
					nextStateWeightList[typeof(FieAITaskPoniesOwnerTracking)] = 100;
					return true;
				}
				return false;
			}
			float num2 = Vector3.Distance(fieRainbowDash.transform.position, fieRainbowDash.detector.lockonTargetObject.position);
			if (num2 > 4.5f)
			{
				nextStateWeightList[typeof(FieAITaskPoniesOwnerTracking)] = 100;
				return true;
			}
			if (fieRainbowDash.abilitiesContainer.GetCooltime<FieStateMachineRainbowDashDoublePayback>() <= 0f)
			{
				nextStateWeightList[typeof(FieAITaskRainbowDashDoublePayback)] = 500;
			}
			if (fieRainbowDash.abilitiesContainer.GetCooltime<FieStateMachineRainbowDashRainblow>() <= 0f)
			{
				nextStateWeightList[typeof(FieAITaskRainbowDashRainblow)] = 500;
			}
			if (num2 > 2.5f)
			{
				if (fieRainbowDash.healthStats.shield > 0f)
				{
					nextStateWeightList[typeof(FieAITaskRainbowDashEnemyTracking)] = 500;
				}
				else
				{
					nextStateWeightList[typeof(FieAITaskRainbowDashEnemyEvade)] = 500;
				}
				if (fieRainbowDash.awesomeCount > 0)
				{
					nextStateWeightList[typeof(FieAITaskRainbowDashOmniSmash)] = 200 * fieRainbowDash.awesomeCount;
				}
				return true;
			}
			if (fieRainbowDash.healthStats.shield <= 0f)
			{
				nextStateWeightList[typeof(FieAITaskRainbowDashEnemyEvade)] = 500;
				return true;
			}
			nextStateWeightList[typeof(FieAITaskRainbowDashEvasion)] = 200;
			if (fieRainbowDash.groundState == FieObjectGroundState.Grounding)
			{
				nextStateWeightList[typeof(FieAITaskRainbowDashJump)] = 200;
			}
			nextStateWeightList[typeof(FieAITaskRainbowDashMelee)] = 200;
			return true;
		}
	}
}
