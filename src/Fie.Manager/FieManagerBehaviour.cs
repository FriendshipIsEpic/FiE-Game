using UnityEngine;

namespace Fie.Manager
{
	public class FieManagerBehaviour<T> : FieManagerBase where T : FieManagerBase
	{
		[SerializeField]
		private bool _isAutoStartUp;

		private static T instance;

		public static T I
		{
			get
			{
				if ((UnityEngine.Object)instance != (UnityEngine.Object)null && instance.isDestroyed)
				{
					instance = (T)null;
				}
				if ((UnityEngine.Object)instance == (UnityEngine.Object)null)
				{
					instance = UnityEngine.Object.FindObjectOfType<T>();
					if ((UnityEngine.Object)instance == (UnityEngine.Object)null || instance.isDestroyed)
					{
						GameObject gameObject = new GameObject();
						UnityEngine.Object.DontDestroyOnLoad(gameObject);
						instance = gameObject.AddComponent<T>();
						gameObject.name = instance.GetType().Name;
					}
					FieManagerFactory.I.AddManager(instance);
				}
				return instance;
			}
			set
			{
				instance = value;
			}
		}

		private void Start()
		{
			if (_isAutoStartUp)
			{
				T i = I;
				i.StartUp();
			}
		}
	}
}
