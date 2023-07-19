using Fie.Camera;
using Fie.Manager;
using UnityEngine;

namespace Fie.UI
{
	public abstract class FieGameUIBase : MonoBehaviour
	{
		private FieUICamera _uiCamera;

		private FieGameCharacter _ownerCharacter;

		private bool _active;

		public FieUICamera uiCamera
		{
			get
			{
				return _uiCamera;
			}
			set
			{
				_uiCamera = value;
			}
		}

		public FieGameCharacter ownerCharacter
		{
			get
			{
				return _ownerCharacter;
			}
			set
			{
				_ownerCharacter = value;
			}
		}

		public bool uiActive
		{
			get
			{
				return _active;
			}
			set
			{
				base.gameObject.SetActive(value);
				_active = value;
			}
		}

		public virtual void Initialize()
		{
		}

		public virtual void Terminate()
		{
		}

		public void SetUILayer(FieGUIManager.FieUILayer layer)
		{
			Transform transform = base.transform;
			Vector3 position = base.transform.position;
			float x = position.x;
			Vector3 position2 = base.transform.position;
			transform.position = new Vector3(x, position2.y, (float)layer);
		}
	}
}
