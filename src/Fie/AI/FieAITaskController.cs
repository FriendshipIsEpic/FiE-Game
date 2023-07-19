using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.AI
{
	public class FieAITaskController : FieAIControllerBase
	{
		public struct FieAIFrontAndBackPoint
		{
			public float frontDistance;

			public float backDistance;

			public Vector3 frontPoint;

			public Vector3 backPoint;
		}

		public Dictionary<Type, float> taskExecuteTimeList = new Dictionary<Type, float>();

		private Dictionary<Type, FieAITaskBase> taskCache = new Dictionary<Type, FieAITaskBase>();

		private Queue<FieAITaskBase> preQueueTaskList = new Queue<FieAITaskBase>();

		private FieAITaskBase currentTask;

		public void Start()
		{
			currentTask = getStartAI();
		}

		public void Update()
		{
			if (!(base.ownerCharacter == null) && PhotonNetwork.isMasterClient && currentTask.TaskExec(this))
			{
				currentTask.Terminate(this);
				taskExecuteTimeList[currentTask.GetType()] = Time.time;
				if (preQueueTaskList.Count > 0)
				{
					currentTask = preQueueTaskList.Dequeue();
				}
				else
				{
					currentTask = getNextTask(currentTask.GetWeight());
				}
				if (currentTask != null)
				{
					currentTask.nextStateWeightList.Clear();
					currentTask.Initialize(this);
				}
			}
		}

		public void resetTask()
		{
			currentTask = getStartAI();
		}

		private FieAITaskBase getTaskInstanceFromCache(Type type)
		{
			if (!taskCache.ContainsKey(type))
			{
				FieAITaskBase fieAITaskBase = Activator.CreateInstance(type) as FieAITaskBase;
				if (fieAITaskBase == null || !fieAITaskBase.GetType().IsSubclassOf(typeof(FieAITaskBase)))
				{
					return null;
				}
				taskCache[type] = fieAITaskBase;
			}
			return taskCache[type];
		}

		private FieAITaskBase getNextTask(Dictionary<Type, int> weightList)
		{
			if (weightList.Count == 1)
			{
				using (Dictionary<Type, int>.Enumerator enumerator = weightList.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						KeyValuePair<Type, int> current = enumerator.Current;
						if (current.Key.IsSubclassOf(typeof(FieAITaskBase)))
						{
							return getTaskInstanceFromCache(current.Key);
						}
					}
				}
			}
			else if (weightList.Count > 1)
			{
				int num = 0;
				foreach (KeyValuePair<Type, int> weight in weightList)
				{
					num += weight.Value;
				}
				if (num > 0)
				{
					int num2 = UnityEngine.Random.Range(0, num);
					int num3 = 1;
					foreach (KeyValuePair<Type, int> weight2 in weightList)
					{
						num3 += weight2.Value;
						if (num2 <= num3 - 1)
						{
							if (!weight2.Key.IsSubclassOf(typeof(FieAITaskBase)))
							{
								break;
							}
							return getTaskInstanceFromCache(weight2.Key);
						}
					}
				}
			}
			return getStartAI();
		}

		private FieAITaskBase getStartAI()
		{
			return getTaskInstanceFromCache(base.ownerCharacter.getDefaultAITask());
		}

		public float getExecutedTaskInterval(Type taskType)
		{
			if (!taskExecuteTimeList.ContainsKey(taskType))
			{
				return 3.40282347E+38f;
			}
			return Time.time - taskExecuteTimeList[taskType];
		}

		public void ResetQueueTask()
		{
			preQueueTaskList.Clear();
		}

		public void AddQueueTask<T>() where T : FieAITaskBase
		{
			preQueueTaskList.Enqueue(getTaskInstanceFromCache(typeof(T)));
		}

		public FieAIFrontAndBackPoint GetFrontAndBackPoint(float rayLength = 2f)
		{
			FieAIFrontAndBackPoint result = default(FieAIFrontAndBackPoint);
			if (base.ownerCharacter == null)
			{
				return result;
			}
			int layerMask = 1049088;
			if (Physics.Raycast(base.ownerCharacter.centerTransform.position, base.ownerCharacter.flipDirectionVector, out RaycastHit hitInfo, rayLength, layerMask))
			{
				result.frontPoint = hitInfo.point;
				result.frontDistance = Vector3.Distance(hitInfo.point, base.ownerCharacter.centerTransform.position);
			}
			if (Physics.Raycast(base.ownerCharacter.centerTransform.position, -base.ownerCharacter.flipDirectionVector, out hitInfo, rayLength, layerMask))
			{
				result.backPoint = hitInfo.point;
				result.backDistance = Vector3.Distance(hitInfo.point, base.ownerCharacter.centerTransform.position);
			}
			return result;
		}
	}
}
