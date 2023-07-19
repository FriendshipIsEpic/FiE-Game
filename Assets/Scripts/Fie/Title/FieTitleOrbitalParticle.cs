using UnityEngine;

namespace Fie.Title
{
	public class FieTitleOrbitalParticle : MonoBehaviour
	{
		[SerializeField]
		private Transform _orbitCenterTransform;

		[SerializeField]
		private Transform _orbitRadiucTransform;

		[SerializeField]
		private PKFxFX _orbitalParticle;

		private PKFxManager.Attribute _orbitalAttribute;

		private bool _isEnable = true;

		private void Awake()
		{
			_orbitalAttribute = new PKFxManager.Attribute("OrbitalRadius", 1f);
			int qualityLevel = QualitySettings.GetQualityLevel();
			if (qualityLevel == 0 || qualityLevel == 1)
			{
				if (_orbitalParticle.Alive())
				{
					_orbitalParticle.KillEffect();
				}
				_isEnable = false;
			}
		}

		private void LateUpdate()
		{
			if (_isEnable && _orbitalParticle.AttributeExists(_orbitalAttribute.m_Descriptor))
			{
				_orbitalAttribute.ValueFloat = Vector3.Distance(_orbitCenterTransform.position, _orbitRadiucTransform.position);
				_orbitalParticle.SetAttribute(_orbitalAttribute);
			}
		}
	}
}
