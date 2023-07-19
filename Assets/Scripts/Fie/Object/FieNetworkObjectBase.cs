using Photon;
using UnityEngine;

namespace Fie.Object
{
	public abstract class FieNetworkObjectBase : Photon.MonoBehaviour, FieObjectInterface
	{
		private FieObjectBaseState _baseState;

		private FieObjectGroundState _groundState;

		[SerializeField]
		private FieObjectFlipState _flipState;

		private PhotonTransformView _photonTransformView;

		public PhotonTransformView photonTransformView => _photonTransformView;

		public FieObjectBaseState baseState
		{
			get
			{
				return _baseState;
			}
			set
			{
				_baseState = value;
			}
		}

		public FieObjectGroundState groundState
		{
			get
			{
				return _groundState;
			}
			set
			{
				_groundState = value;
			}
		}

		public FieObjectFlipState flipState
		{
			get
			{
				return _flipState;
			}
			set
			{
				_flipState = value;
			}
		}

		protected virtual void Awake()
		{
			_photonTransformView = GetComponent<PhotonTransformView>();
		}
	}
}
