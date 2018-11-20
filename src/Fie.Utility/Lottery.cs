using System.Collections.Generic;
using UnityEngine;

namespace Fie.Utility
{
	public class Lottery<T>
	{
		private class LotteryItem
		{
			public T obj;

			public int weight;

			public LotteryItem(T obj, int weight)
			{
				this.obj = obj;
				this.weight = weight;
			}
		}

		private List<LotteryItem> _lotList = new List<LotteryItem>();

		private int maximumWeight;

		public void AddItem(T item, int weight = 1)
		{
			if (weight <= 0)
			{
				weight = 1;
				UnityEngine.Debug.Log("Lottery : A weight must be bigger than 0. It's rounded to 1.");
			}
			_lotList.Add(new LotteryItem(item, weight));
			maximumWeight += weight;
		}

		public void InitializeFromListData(List<T> lotteryList)
		{
			_lotList = new List<LotteryItem>();
			foreach (T lottery in lotteryList)
			{
				AddItem(lottery);
			}
		}

		public bool IsExecutable()
		{
			return _lotList.Count > 0;
		}

		public T Lot()
		{
			int num = Random.Range(1, maximumWeight + 1);
			int num2 = 0;
			for (int i = 0; i < _lotList.Count; i++)
			{
				num2 += _lotList[i].weight;
				if (num2 >= num)
				{
					return _lotList[i].obj;
				}
			}
			UnityEngine.Debug.Log("Lottery : Faild to lot. Dump : " + _lotList.ToString());
			return _lotList[0].obj;
		}
	}
}
