using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FieCollider : MonoBehaviour
{
	[SerializeField]
	private FieGameCharacter _parentCharacter;

	[SerializeField]
	private bool _isRoot;

	[SerializeField]
	private bool _isAbsolutelyEnable;

	private bool _isEnable;

	private Collider _currentCollider;

	private List<FieDetector> _hittedDetecter = new List<FieDetector>();

	public bool isEnable
	{
		get
		{
			return _isEnable;
		}
		set
		{
			_isEnable = (value | _isAbsolutelyEnable);
			if (_isEnable)
			{
				if ((bool)_currentCollider)
				{
					_currentCollider.enabled = true;
				}
			}
			else if ((bool)_currentCollider)
			{
				_currentCollider.enabled = false;
			}
		}
	}

	public bool isRoot => _isRoot;

	public void Awake()
	{
		if (!(_parentCharacter == null))
		{
			_parentCharacter.colliderList.Add(this);
			_currentCollider = GetComponent<Collider>();
		}
	}

	public FieGameCharacter getParentGameCharacter()
	{
		return _parentCharacter;
	}

	public void unbindFromDetector()
	{
		if (_hittedDetecter.Count > 0)
		{
			foreach (FieDetector item in _hittedDetecter)
			{
				if (!(item == null))
				{
					item.OnTriggerExit(_currentCollider);
				}
			}
			_hittedDetecter = new List<FieDetector>();
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (!(collider == null))
		{
			FieDetector component = collider.gameObject.GetComponent<FieDetector>();
			if (!(component == null))
			{
				_hittedDetecter.Add(component);
			}
		}
	}
}
