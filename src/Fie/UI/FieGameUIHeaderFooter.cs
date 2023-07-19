using Spine.Unity;
using UnityEngine;

namespace Fie.UI
{
	[RequireComponent(typeof(SkeletonAnimation))]
	public class FieGameUIHeaderFooter : FieGameUIBase
	{
		public const string HEADER_FOOTER_SHOW_ANIIMATION_NAME = "show";

		public const string HEADER_FOOTER_HIDE_ANIIMATION_NAME = "hide";

		public GameObject header;

		public GameObject footer;

		public Transform headerRootTransform;

		public Transform footerRootTransform;

		public Transform Ability1;

		public Transform Ability2;

		public Transform Ability3;

		public SkeletonAnimation _mySkeleton;

		public void Awake()
		{
			_mySkeleton = GetComponent<SkeletonAnimation>();
		}

		public void Show()
		{
			if (!(_mySkeleton == null))
			{
				_mySkeleton.state.SetAnimation(0, "show", loop: false);
			}
		}

		public void Hide()
		{
			if (!(_mySkeleton == null))
			{
				_mySkeleton.state.SetAnimation(0, "hide", loop: false);
			}
		}
	}
}
