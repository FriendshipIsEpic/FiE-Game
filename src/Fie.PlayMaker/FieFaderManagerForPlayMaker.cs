using Fie.Manager;
using HutongGames.PlayMaker;
using UnityEngine;

namespace Fie.PlayMaker
{
	[ActionCategory("Friendship is Epic")]
	public class FieFaderManagerForPlayMaker : FsmStateAction
	{
		public FieFaderManager.FadeType fadeType;

		public float fadeTime = 1f;

		public float delay;

		private float delayCount;

		private bool isLaunched;

		public override void Reset()
		{
			delayCount = 0f;
			isLaunched = false;
		}

		public override void OnUpdate()
		{
			if (!isLaunched)
			{
				if (delayCount <= delay)
				{
					delayCount += Time.deltaTime;
				}
				else
				{
					if (delayCount >= delay && fadeType != 0)
					{
						FieManagerBehaviour<FieFaderManager>.I.LaunchFader(fadeType, fadeTime);
					}
					isLaunched = true;
				}
			}
		}
	}
}
