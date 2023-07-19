using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.AI
{
	public abstract class FieAITaskBase : FieAITaskInterface
	{
		protected delegate bool TaskDelegate(FieAITaskController manager);

		protected float currentTime;

		public Dictionary<Type, int> nextStateWeightList = new Dictionary<Type, int>();

		protected event TaskDelegate taskEvent;

		public FieAITaskBase()
		{
			taskEvent += Task;
		}

		public virtual bool TaskExec(FieAITaskController manager)
		{
			if (this.taskEvent == null)
			{
				return true;
			}
			currentTime += Time.deltaTime;
			return this.taskEvent(manager);
		}

		public virtual void Initialize(FieAITaskController manager)
		{
		}

		public virtual void Terminate(FieAITaskController manager)
		{
		}

		public abstract bool Task(FieAITaskController manager);

		public Dictionary<Type, int> GetWeight()
		{
			return nextStateWeightList;
		}
	}
}
