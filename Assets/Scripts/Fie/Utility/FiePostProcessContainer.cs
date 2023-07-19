using System.Reflection;
using UnityEngine;

namespace Fie.Utility
{
	public class FiePostProcessContainer : MonoBehaviour
	{
		public virtual void AttachPostProcessEffect(GameObject targetObject)
		{
			MonoBehaviour[] components = base.transform.GetComponents<MonoBehaviour>();
			if (components != null && components.Length > 0)
			{
				MonoBehaviour[] array = components;
				foreach (MonoBehaviour monoBehaviour in array)
				{
					if (monoBehaviour.GetType() != typeof(UnityEngine.Camera) && monoBehaviour.GetType() != typeof(FiePostProcessContainer))
					{
						Component component = targetObject.GetComponent(monoBehaviour.GetType());
						if (component == null)
						{
							component = targetObject.AddComponent(monoBehaviour.GetType());
							if (component == null)
							{
								continue;
							}
						}
						FieldInfo[] fields = monoBehaviour.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
						int num = 0;
						for (num = 0; num < fields.Length; num++)
						{
							object value = fields[num].GetValue(monoBehaviour);
							fields[num].SetValue(component, value);
						}
					}
				}
			}
		}

		public virtual void PostHook(GameObject targetObject)
		{
		}
	}
}
