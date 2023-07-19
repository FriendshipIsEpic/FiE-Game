using Fie.Object;
using UnityEngine;

namespace Fie.Enemies.HoovesRaces.QueenChrysalis
{
	[FiePrefabInfo("Prefabs/Enemies/ChangelingForces/QueenChrysalis/Power/QueenChrysalisHitEffectBurned")]
	public class FieEmitObjectQueenChrysalisHitEffectBurned : FieEmittableObjectBase
	{
		[SerializeField]
		private PKFxFX _burningFx;

		private const float DURATION = 1f;

		private float _lifeCount;

		private bool _isEnd;

		public override void awakeEmitObject()
		{
			if (_burningFx != null)
			{
				_burningFx.StopEffect();
				_burningFx.StartEffect();
			}
		}

		private void Update()
		{
			if (!_isEnd)
			{
				_lifeCount += Time.deltaTime;
				if (_lifeCount > 1f)
				{
					if (_burningFx != null)
					{
						_burningFx.StopEffect();
					}
					destoryEmitObject(1f);
					_isEnd = true;
				}
			}
		}

		private void LateUpdate()
		{
			base.transform.position = initTransform.position;
		}
	}
}
