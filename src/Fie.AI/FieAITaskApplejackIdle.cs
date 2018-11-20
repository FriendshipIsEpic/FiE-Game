using Fie.Manager;
using Fie.Object;
using Fie.Ponies;
using Fie.Ponies.Applejack;
using UnityEngine;

namespace Fie.AI
{
	public class FieAITaskApplejackIdle : FieAITaskBase
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
			if (manager.ownerCharacter.groundState != 0)
			{
				return false;
			}
			if (manager.ownerCharacter.damageSystem.isDead)
			{
				if (manager.ownerCharacter.friendshipStats.getCurrentFriendshipPoint() >= 3)
				{
					(manager.ownerCharacter as FiePonies).TryToRevive();
				}
				return true;
			}
			if (injuryCharacter != null && manager.ownerCharacter.friendshipStats.getCurrentFriendshipPoint() >= 2)
			{
				nextStateWeightList[typeof(FieAITaskPoniesRescue)] = 100;
				return true;
			}
			float num = Vector3.Distance(manager.ownerCharacter.transform.position, FieManagerBehaviour<FieUserManager>.I.gameOwnerCharacter.transform.position);
			if (manager.ownerCharacter.detector.lockonTargetObject == null)
			{
				if (num > 2.5f)
				{
					nextStateWeightList[typeof(FieAITaskPoniesOwnerTracking)] = 100;
					return true;
				}
				return false;
			}
			float num2 = Vector3.Distance(manager.ownerCharacter.transform.position, manager.ownerCharacter.detector.lockonTargetObject.position);
			if (num2 > 4.5f)
			{
				nextStateWeightList[typeof(FieAITaskPoniesOwnerTracking)] = 100;
				return true;
			}
			if (num2 > 2.5f)
			{
				nextStateWeightList[typeof(FieAITaskApplejackEnemyTracking)] = 500;
				if (manager.ownerCharacter.abilitiesContainer.GetCooltime<FieStateMachineApplejackRope>() <= 0f)
				{
					nextStateWeightList[typeof(FieAITaskApplejackRope)] = 500;
				}
				return true;
			}
			if (manager.ownerCharacter.abilitiesContainer.GetCooltime<FieStateMachineApplejackStomp>() <= 0f && manager.ownerCharacter.abilitiesContainer.GetCooltime<FieStateMachineApplejackYeehaw>() <= 0f)
			{
				nextStateWeightList[typeof(FieAITaskApplejackStomp)] = (int)Mathf.Max(1000f * manager.ownerCharacter.healthStats.nowHelthAndShieldRatePerMax, 100f);
				nextStateWeightList[typeof(FieAITaskApplejackYeehaw)] = (int)Mathf.Max(2000f * (1f - manager.ownerCharacter.healthStats.nowHelthAndShieldRatePerMax), 500f);
				return true;
			}
			nextStateWeightList[typeof(FieAITaskApplejackBackstep)] = 200;
			if (manager.ownerCharacter.groundState == FieObjectGroundState.Grounding)
			{
				nextStateWeightList[typeof(FieAITaskApplejackJump)] = 200;
			}
			nextStateWeightList[typeof(FieAITaskApplejackMelee)] = 200;
			return true;
		}
	}
}
