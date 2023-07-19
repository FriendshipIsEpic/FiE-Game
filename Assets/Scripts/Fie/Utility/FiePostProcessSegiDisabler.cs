using UnityEngine;

namespace Fie.Utility
{
	public class FiePostProcessSegiDisabler : FiePostProcessContainer
	{
		public override void PostHook(GameObject targetObject)
		{
			SEGI component = targetObject.transform.GetComponent<SEGI>();
			if (!(component == null))
			{
				component.enabled = false;
			}
		}
	}
}
