using UnityEngine;

namespace Fie.UI
{
	public class FieSkillTreeActivationEffect : MonoBehaviour
	{
		[SerializeField]
		private PKFxFX _activationEffect;

		[SerializeField]
		private AudioSource _activateSound;

		private PKFxManager.Attribute _activationEffectColorAttribute = new PKFxManager.Attribute("RGB", Vector3.one);

		public void Activate(FieSkillTreeEndPoint endPoint)
		{
			if (!(endPoint == null))
			{
				base.transform.position = endPoint.transform.position;
				_activationEffectColorAttribute.ValueFloat3 = new Vector3(endPoint.assigendSkillData.UIColor.R, endPoint.assigendSkillData.UIColor.G, endPoint.assigendSkillData.UIColor.B);
				_activationEffect.SetAttribute(_activationEffectColorAttribute);
				_activationEffect.StopEffect();
				_activationEffect.StartEffect();
				_activateSound.Play();
			}
		}
	}
}
