using Fie.Object;
using UnityEngine;

namespace Fie.Enemies.HoovesRaces.QueenChrysalis
{
	[FiePrefabInfo("Prefabs/Enemies/ChangelingForces/QueenChrysalis/Power/QueenChrysalisIgniteCollision")]
	public class FieEmitObjectQueenChrysalisIgniteCollision : FieEmittableObjectBase
	{
		public delegate void GrabbedDelegate(FieGameCharacter grabbedCharacter);

		[SerializeField]
		private float ChargeDuration = 0.5f;

		[SerializeField]
		private float ChargeDamageDuration = 0.5f;

		private float _lifeTimeCount;

		private bool _isEndUpdate;

		public event GrabbedDelegate grabbedEvent;

		public void Update()
		{
			if (!_isEndUpdate)
			{
				_lifeTimeCount += Time.deltaTime;
				if (_lifeTimeCount >= ChargeDuration)
				{
					_isEndUpdate = true;
					destoryEmitObject();
				}
				if (initTransform != null)
				{
					base.transform.position = initTransform.position;
				}
			}
		}

		private void OnTriggerEnter(Collider collider)
		{
			if (!_isEndUpdate && !(_lifeTimeCount > ChargeDamageDuration) && collider.gameObject.tag == getHostileTagString())
			{
				FieGameCharacter fieGameCharacter = addDamageToCollisionCharacter(collider, getDefaultDamageObject());
				if (fieGameCharacter != null)
				{
					if (this.grabbedEvent != null)
					{
						this.grabbedEvent(fieGameCharacter);
					}
					destoryEmitObject(ChargeDuration - _lifeTimeCount);
				}
			}
		}
	}
}
