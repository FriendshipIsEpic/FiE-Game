using Fie.Manager;
using Fie.Object;
using Fie.Ponies.Twilight;
using UnityEngine;

namespace Fie.AI
{
	public class FieAITaskTwilightIdle : FieAITaskTwilightBase
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
			FieTwilight fieTwilight = manager.ownerCharacter as FieTwilight;
			if (fieTwilight.damageSystem.isDead)
			{
				if (fieTwilight.friendshipStats.getCurrentFriendshipPoint() >= 3)
				{
					fieTwilight.TryToRevive();
				}
				return true;
			}
			if (injuryCharacter != null && fieTwilight.friendshipStats.getCurrentFriendshipPoint() >= 2)
			{
				nextStateWeightList[typeof(FieAITaskPoniesRescue)] = 100;
				return true;
			}
			float num = Vector3.Distance(fieTwilight.transform.position, FieManagerBehaviour<FieUserManager>.I.gameOwnerCharacter.transform.position);
			if (fieTwilight.detector.lockonTargetObject == null)
			{
				if (num > 2.5f)
				{
					nextStateWeightList[typeof(FieAITaskPoniesOwnerTracking)] = 100;
					return true;
				}
				return false;
			}
			float num2 = Vector3.Distance(fieTwilight.transform.position, fieTwilight.detector.lockonTargetObject.position);
			if (num2 > 4.5f)
			{
				nextStateWeightList[typeof(FieAITaskPoniesOwnerTracking)] = 100;
				return true;
			}
			if (num2 > 2.5f)
			{
				if (manager.ownerCharacter.groundState != FieObjectGroundState.Flying)
				{
					nextStateWeightList[typeof(FieAITaskTwilightJump)] = 500;
				}
				if (fieTwilight.abilitiesContainer.GetCooltime<FieStateMachineTwilightSparklyCannon>() <= 0f)
				{
					nextStateWeightList[typeof(FieStateMachineTwilightSparklyCannon)] = 100;
				}
				if (fieTwilight.abilitiesContainer.GetCooltime<FieStateMachineTwilightSummonArrow>() <= 0f)
				{
					nextStateWeightList[typeof(FieAITaskTwilightSummonArrow)] = 100;
				}
				if (fieTwilight.abilitiesContainer.GetCooltime<FieStateMachineTwilightForceField>() <= 0f)
				{
					nextStateWeightList[typeof(FieAITaskTwilightForceField)] = 100;
				}
				if (fieTwilight.healthStats.shield > fieTwilight.healthStats.maxShield * 0.3f)
				{
					nextStateWeightList[typeof(FieAITaskTwilightBaseShot)] = 100;
				}
				else
				{
					nextStateWeightList[typeof(FieAITaskTwilightEnemyEvade)] = 100;
				}
				return true;
			}
			nextStateWeightList.Clear();
			nextStateWeightList[typeof(FieAITaskTwilightEnemyEvade)] = 100;
			return true;
		}
	}
}
