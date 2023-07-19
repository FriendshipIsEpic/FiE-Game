using ParticlePlayground;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Utility
{
	public class FieParticlePlaygroundChain : MonoBehaviour
	{
		[SerializeField]
		private PlaygroundParticlesC source;

		[SerializeField]
		private List<PlaygroundParticlesC> emitters;

		[SerializeField]
		private float delay;

		private PlaygroundEventC birthEvent;

		private bool isStartCoroutine;

		private IEnumerator ChainTask(float delay = 0f)
		{
			yield return (object)new WaitForSeconds(delay);
			/*Error: Unable to find new state assignment for yield return*/;
		}

		private void Awake()
		{
			if (source != null)
			{
				birthEvent = PlaygroundC.CreateEvent(source);
				birthEvent.broadcastType = EVENTBROADCASTC.EventListeners;
				birthEvent.eventType = EVENTTYPEC.Birth;
				birthEvent.particleEvent += StartChain;
			}
		}

		private void Start()
		{
			if (emitters != null && emitters.Count > 0)
			{
				foreach (PlaygroundParticlesC emitter in emitters)
				{
					emitter.emit = false;
				}
			}
		}

		private void Update()
		{
			if (isStartCoroutine)
			{
				StartCoroutine(ChainTask(delay));
				isStartCoroutine = false;
			}
		}

		private void StartChain(PlaygroundEventParticle particle)
		{
			isStartCoroutine = true;
		}
	}
}
