using Fie.Manager;
using System.Collections.Generic;

namespace Fie.UI
{
	public class FieGameUIEnemyLifeGaugeManager : FieGameUIComponentManagerBase
	{
		private Dictionary<int, FieGameUIEnemyLifeGauge> _lifeGaugeList = new Dictionary<int, FieGameUIEnemyLifeGauge>();

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
			if (!(base.componentManagerOwner.detector.enemyTag != targetCharacter.tag) && !(targetCharacter == null))
			{
				cleanupListsData();
				int instanceID = targetCharacter.gameObject.GetInstanceID();
				if (!_lifeGaugeList.ContainsKey(instanceID))
				{
					FieGameUIEnemyLifeGauge fieGameUIEnemyLifeGauge = FieManagerBehaviour<FieGUIManager>.I.CreateGui<FieGameUIEnemyLifeGauge>(targetCharacter);
					if (fieGameUIEnemyLifeGauge == null)
					{
						return;
					}
					_lifeGaugeList[instanceID] = fieGameUIEnemyLifeGauge;
				}
				else
				{
					_lifeGaugeList[instanceID].uiActive = true;
				}
				_lifeGaugeList[instanceID].uiCamera = FieManagerBehaviour<FieGUIManager>.I.uiCamera;
				_lifeGaugeList[instanceID].ownerCharacter = targetCharacter;
				_lifeGaugeList[instanceID].Initialize();
				_lifeGaugeList[instanceID].currentLayer = FieGUIManager.FieUILayer.BACKWORD_SECOND;
			}
		}

		private void cleanupListsData()
		{
			List<int> list = new List<int>();
			foreach (KeyValuePair<int, FieGameUIEnemyLifeGauge> lifeGauge in _lifeGaugeList)
			{
				if (lifeGauge.Value.ownerCharacter == null)
				{
					list.Add(lifeGauge.Key);
				}
			}
			foreach (int item in list)
			{
				_lifeGaugeList.Remove(item);
			}
		}
	}
}
