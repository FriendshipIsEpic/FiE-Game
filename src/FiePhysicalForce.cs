using CircularGravityForce;
using Fie.Utility;
using UnityEngine;

public class FiePhysicalForce : MonoBehaviour
{
	private const float PHYSICAL_FORCE_MAGNI = 2f;

	[SerializeField]
	private CGF _gravityObject;

	private Tweener<TweenTypesOutSine> _tweener;

	private float _initSize;

	public void Awake()
	{
		_tweener = new Tweener<TweenTypesOutSine>();
		_initSize = _gravityObject.Size;
	}

	public void SetPhysicalForce(Vector3 normalVec, float force, float duration = 0.2f)
	{
		if (normalVec != Vector3.zero)
		{
			base.transform.rotation = Quaternion.LookRotation(normalVec);
		}
		_tweener.InitTweener(duration, force, 0f);
	}

	public void Update()
	{
		if (!_tweener.IsEnd())
		{
			_gravityObject.ForcePower = _tweener.UpdateParameterFloat(Time.deltaTime) * 2f;
		}
	}
}
