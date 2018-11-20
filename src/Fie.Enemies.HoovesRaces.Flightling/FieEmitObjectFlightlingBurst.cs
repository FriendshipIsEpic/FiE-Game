using Fie.Object;
using UnityEngine;

namespace Fie.Enemies.HoovesRaces.Flightling
{
	[FiePrefabInfo("Prefabs/Enemies/ChangelingForces/Flightling/Power/FlightlingBurst")]
	public class FieEmitObjectFlightlingBurst : FieEmittableObjectBase
	{
		[SerializeField]
		private float FlightlingBurstDuration = 0.7f;

		[SerializeField]
		private float FlightlingBurstDestroyDuration = 0.5f;

		private bool _isEndUpdate;

		private float _lifeCount;

		private void Update()
		{
			if (!_isEndUpdate)
			{
				_lifeCount += Time.deltaTime;
				if (_lifeCount > FlightlingBurstDuration)
				{
					destoryEmitObject(FlightlingBurstDestroyDuration);
					_isEndUpdate = true;
				}
			}
		}

		private void OnTriggerEnter(Collider collider)
		{
			if (!_isEndUpdate && collider.gameObject.tag == getHostileTagString())
			{
				addDamageToCollisionCharacter(collider, getDefaultDamageObject());
			}
		}
	}
}
