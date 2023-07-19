using Fie.Object;
using UnityEngine;

namespace Fie.Utility
{
	public class FieUtilAutoPopcornFxActivator : MonoBehaviour
	{
		[SerializeField]
		private FieEmittableObjectBase emittableObject;

		[SerializeField]
		private PKFxFX fx;

		private void Awake()
		{
			if (emittableObject != null)
			{
				emittableObject.awakeningEvent += EmittableObject_awakeningEvent;
			}
		}

		private void EmittableObject_awakeningEvent(FieEmittableObjectBase emitObject)
		{
			if (fx != null)
			{
				fx.StopEffect();
				fx.StartEffect();
			}
		}
	}
}
