using Photon;
using UnityEngine;

namespace Fie.Manager
{
	[FieManagerExists(FieManagerExistSceneFlag.ANYTIME_DESTROY)]
	public abstract class FieManagerBase : Photon.MonoBehaviour
	{
		private bool _isEndStartup;

		private bool _isDestroyed;

		public bool isDestroyed => _isDestroyed;

		protected virtual void StartUpEntity()
		{
		}

		public void StartUp()
		{
			if (!_isEndStartup)
			{
				StartUpEntity();
				_isEndStartup = true;
			}
		}

		public void Destroy()
		{
			UnityEngine.Object.Destroy(base.transform.gameObject);
			_isDestroyed = true;
		}
	}
}
