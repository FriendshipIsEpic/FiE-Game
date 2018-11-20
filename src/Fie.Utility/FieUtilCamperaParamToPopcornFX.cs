using UnityEngine;

namespace Fie.Utility
{
	public class FieUtilCamperaParamToPopcornFX : MonoBehaviour
	{
		[SerializeField]
		private Transform CameraTransform;

		[SerializeField]
		private string CameraPositionAttributeName;

		[SerializeField]
		private string CameraDirectionAttributeName;

		[SerializeField]
		private PKFxFX fx;

		private PKFxManager.Attribute _cameraPositionAttribute;

		private PKFxManager.Attribute _cameraDirectrionAttribute;

		private void Awake()
		{
			_cameraPositionAttribute = new PKFxManager.Attribute(CameraPositionAttributeName, CameraTransform.position);
			_cameraDirectrionAttribute = new PKFxManager.Attribute(CameraDirectionAttributeName, CameraTransform.forward);
		}

		private void Update()
		{
			PKFxManager.Attribute cameraPositionAttribute = _cameraPositionAttribute;
			Vector3 position = CameraTransform.position;
			cameraPositionAttribute.m_Value0 = position.x;
			PKFxManager.Attribute cameraPositionAttribute2 = _cameraPositionAttribute;
			Vector3 position2 = CameraTransform.position;
			cameraPositionAttribute2.m_Value1 = position2.y;
			PKFxManager.Attribute cameraPositionAttribute3 = _cameraPositionAttribute;
			Vector3 position3 = CameraTransform.position;
			cameraPositionAttribute3.m_Value2 = position3.z;
			PKFxManager.Attribute cameraDirectrionAttribute = _cameraDirectrionAttribute;
			Vector3 forward = CameraTransform.forward;
			cameraDirectrionAttribute.m_Value0 = forward.x;
			PKFxManager.Attribute cameraDirectrionAttribute2 = _cameraDirectrionAttribute;
			Vector3 forward2 = CameraTransform.forward;
			cameraDirectrionAttribute2.m_Value1 = forward2.y;
			PKFxManager.Attribute cameraDirectrionAttribute3 = _cameraDirectrionAttribute;
			Vector3 forward3 = CameraTransform.forward;
			cameraDirectrionAttribute3.m_Value2 = forward3.z;
			fx.SetAttribute(_cameraPositionAttribute);
			fx.SetAttribute(_cameraDirectrionAttribute);
		}
	}
}
