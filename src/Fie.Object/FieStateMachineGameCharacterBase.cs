using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Object
{
	public abstract class FieStateMachineGameCharacterBase : FieStateMachineInterface
	{
		protected List<FieGameCharacter> targetGameCharacterList = new List<FieGameCharacter>();

		event StateChangeDelegate FieStateMachineInterface.stateChangeEvent
		{
			add
			{
				stateChangeEvent += value;
			}
			remove
			{
				stateChangeEvent -= value;
			}
		}

		private event StateChangeDelegate stateChangeEvent;

		public void terminateAndCallStateChangeEvent(FieGameCharacter gameCharacter, Type fromType, Type toType)
		{
			if (this.stateChangeEvent != null)
			{
				this.stateChangeEvent(fromType, toType);
			}
			terminate(gameCharacter);
		}

		public abstract void updateState<T>(ref T gameCharacter) where T : FieGameCharacter;

		public abstract bool isEnd();

		public abstract Type getNextState();

		public virtual void initialize(FieGameCharacter gameCharacter)
		{
		}

		public virtual void terminate(FieGameCharacter gameCharacter)
		{
		}

		protected void autoFlipToEnemy(FieGameCharacter gameCharacter)
		{
			if (!(gameCharacter == null))
			{
				FieGameCharacter lockonEnemyGameCharacter = gameCharacter.detector.getLockonEnemyGameCharacter();
				if (lockonEnemyGameCharacter != null)
				{
					Vector3 vector = lockonEnemyGameCharacter.transform.position - gameCharacter.transform.position;
					if (vector.x > 0f)
					{
						gameCharacter.setFlip(FieObjectFlipState.Right);
					}
					else
					{
						gameCharacter.setFlip(FieObjectFlipState.Left);
					}
				}
			}
		}

		public void addTargetGameCharacter(FieGameCharacter targetGameCharacter)
		{
			targetGameCharacterList.Add(targetGameCharacter);
		}

		public virtual List<Type> getAllowedStateList()
		{
			return new List<Type>();
		}

		public virtual float getDelay()
		{
			return 0f;
		}

		public virtual bool isFinished()
		{
			return isEnd();
		}

		public virtual bool isNotNetworkSync()
		{
			return false;
		}
	}
}
