using Fie.Manager;
using System.Collections.Generic;

namespace Fie.UI
{
	public class FieGameUIIndicatorManager : FieGameUIComponentManagerBase
	{
		private List<FieGameUIIndicator> _indicatorList = new List<FieGameUIIndicator>();

		public override void setComponentManagerOwner(FieGameCharacter owner)
		{
			base.setComponentManagerOwner(owner);
			owner.detector.locatedEvent += Detector_locatedEvent;
			owner.detector.missedEvent += Detector_missedEvent;
		}

		private void OnDestroy()
		{
			if (base.componentManagerOwner != null)
			{
				base.componentManagerOwner.detector.locatedEvent -= Detector_locatedEvent;
				base.componentManagerOwner.detector.missedEvent -= Detector_missedEvent;
			}
		}

		private void Detector_missedEvent(FieGameCharacter targetCharacter)
		{
			cleanupListsData();
		}

		private void Detector_locatedEvent(FieGameCharacter targetCharacter)
		{
			if (!(targetCharacter == null))
			{
				cleanupListsData();
				FieGameUIIndicator fieGameUIIndicator = null;
				foreach (FieGameUIIndicator indicator in _indicatorList)
				{
					if (!indicator.uiActive)
					{
						fieGameUIIndicator = indicator;
						break;
					}
				}
				if (fieGameUIIndicator == null)
				{
					fieGameUIIndicator = FieManagerBehaviour<FieGUIManager>.I.CreateGui<FieGameUIIndicator>(targetCharacter);
					_indicatorList.Add(fieGameUIIndicator);
				}
				fieGameUIIndicator.uiActive = true;
				fieGameUIIndicator.uiCamera = FieManagerBehaviour<FieGUIManager>.I.uiCamera;
				fieGameUIIndicator.ownerCharacter = targetCharacter;
				fieGameUIIndicator.Initialize();
			}
		}

		private void cleanupListsData()
		{
			foreach (FieGameUIIndicator indicator in _indicatorList)
			{
				if (indicator.ownerCharacter == null)
				{
					indicator.Terminate();
					indicator.uiActive = false;
				}
			}
		}
	}
}
