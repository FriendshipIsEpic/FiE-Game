using Fie.Manager;
using System.Collections.Generic;

namespace Fie.UI
{
	public class FieGameUIDamageCounterManager : FieGameUIComponentManagerBase
	{
		private Dictionary<int, FieGameUIDamageCounter> _damageCounterList = new Dictionary<int, FieGameUIDamageCounter>();

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
				int instanceID = targetCharacter.gameObject.GetInstanceID();
				if (!_damageCounterList.ContainsKey(instanceID))
				{
					FieGameUIDamageCounter fieGameUIDamageCounter = FieManagerBehaviour<FieGUIManager>.I.CreateGui<FieGameUIDamageCounter>(targetCharacter);
					if (fieGameUIDamageCounter == null)
					{
						return;
					}
					_damageCounterList[instanceID] = fieGameUIDamageCounter;
				}
				else
				{
					_damageCounterList[instanceID].uiActive = true;
				}
				_damageCounterList[instanceID].uiCamera = FieManagerBehaviour<FieGUIManager>.I.uiCamera;
				_damageCounterList[instanceID].ownerCharacter = targetCharacter;
				_damageCounterList[instanceID].Initialize();
			}
		}

		private void cleanupListsData()
		{
			List<int> list = new List<int>();
			foreach (KeyValuePair<int, FieGameUIDamageCounter> damageCounter in _damageCounterList)
			{
				if (damageCounter.Value.ownerCharacter == null)
				{
					list.Add(damageCounter.Key);
				}
			}
			foreach (int item in list)
			{
				_damageCounterList.Remove(item);
			}
		}
	}
}
