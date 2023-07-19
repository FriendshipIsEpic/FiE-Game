using Fie.Enemies.HoovesRaces.QueenChrysalis;
using Fie.Object;
using UnityEngine;

namespace Fie.AI
{
	public class FieAITaskQueenChrysalisIdle : FieAITaskBase
	{
		public override bool Task(FieAITaskController manager)
		{
			FieQueenChrysalis fieQueenChrysalis = manager.ownerCharacter as FieQueenChrysalis;
			if (fieQueenChrysalis == null)
			{
				return true;
			}
			if (fieQueenChrysalis.canAbleToCallMinion)
			{
				nextStateWeightList[typeof(FieAITaskQueenChrysallisCallingMinion)] = 100;
				return true;
			}
			if (manager.ownerCharacter.groundState == FieObjectGroundState.Flying)
			{
				nextStateWeightList[typeof(FieAITaskQueenChrysalisAirRaid)] = 100;
				if (fieQueenChrysalis.detector.lockonTargetObject != null)
				{
					float num = Vector3.Distance(fieQueenChrysalis.detector.lockonTargetObject.centerTransform.position, fieQueenChrysalis.centerTransform.position);
					if (num < 1.25f)
					{
						nextStateWeightList[typeof(FieAITaskQueenChrysalisDoubleSlash)] = 500;
					}
					else
					{
						nextStateWeightList[typeof(FieAITaskQueenChrysalisDoubleSlash)] = 100;
					}
				}
			}
			else
			{
				FieAITaskController.FieAIFrontAndBackPoint frontAndBackPoint = manager.GetFrontAndBackPoint();
				if (frontAndBackPoint.backDistance > 0f)
				{
					nextStateWeightList[typeof(FieAITaskQueenChrysalisTeleportation)] = 100;
					return true;
				}
				if (fieQueenChrysalis.detector.lockonTargetObject != null)
				{
					float num2 = Vector3.Distance(fieQueenChrysalis.detector.lockonTargetObject.centerTransform.position, fieQueenChrysalis.centerTransform.position);
					if (num2 > 5f)
					{
						nextStateWeightList[typeof(FieAITaskQueenChrysalisTeleportation)] = 100;
						return true;
					}
					if (num2 > 3f)
					{
						nextStateWeightList[typeof(FieAITaskQueenChrysalisTeleportation)] = 200;
						nextStateWeightList[typeof(FieAITaskQueenChrysalisShot)] = 200;
						nextStateWeightList[typeof(FieAITaskQueenChrysalisDoubleSlash)] = 100;
						nextStateWeightList[typeof(FieAITaskQueenChrysalisCrucible)] = 250;
						nextStateWeightList[typeof(FieAITaskQueenChrysalisIgnite)] = 200;
					}
					else
					{
						nextStateWeightList[typeof(FieAITaskQueenChrysalisTeleportation)] = 50;
						nextStateWeightList[typeof(FieAITaskQueenChrysalisShot)] = 50;
						nextStateWeightList[typeof(FieAITaskQueenChrysalisDoubleSlash)] = 400;
						nextStateWeightList[typeof(FieAITaskQueenChrysalisCrucible)] = 200;
						nextStateWeightList[typeof(FieAITaskQueenChrysalisIgnite)] = 150;
					}
				}
			}
			if (fieQueenChrysalis.healthStats.shield <= 0f)
			{
				nextStateWeightList[typeof(FieAITaskQueenChrysalisMeteoShower)] = 50 + (int)(100f * (fieQueenChrysalis.healthStats.hitPoint / fieQueenChrysalis.healthStats.maxHitPoint));
			}
			return true;
		}
	}
}
