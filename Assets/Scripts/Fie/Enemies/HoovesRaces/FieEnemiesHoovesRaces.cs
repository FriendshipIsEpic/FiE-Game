using Spine;
using UnityEngine;
using Event = Spine.Event;

namespace Fie.Enemies.HoovesRaces
{
	public abstract class FieEnemiesHoovesRaces : FieObjectEnemies
	{
		[SerializeField]
		private Transform _torsoTransform;

		[SerializeField]
		private Transform _leftFrontHoofTransform;

		[SerializeField]
		private Transform _rightFrontHoofTransform;

		[SerializeField]
		private Transform _leftBackHoofTransform;

		[SerializeField]
		private Transform _rightBackHoofTransform;

		[SerializeField]
		private Transform _hornTransform;

		[SerializeField]
		private Transform _mouthTransform;

		public Transform torsoTransform => _torsoTransform;

		public Transform leftFrontHoofTransform => _leftFrontHoofTransform;

		public Transform rightFrontHoofTransform => _rightFrontHoofTransform;

		public Transform leftBackHoofTransform => _leftBackHoofTransform;

		public Transform rightBackHoofTransform => _rightBackHoofTransform;

		public Transform hornTransform => _hornTransform;

		public Transform mouthTransform => _mouthTransform;

		public new void Awake()
		{
			base.Awake();
			base.damageSystem.staggerEvent += delegate
			{
				setStateToStatheMachine(typeof(FieStateMachineEnemiesHoovesRacesStagger), isForceSet: true, isDupulicate: true);
			};
			base.gameObject.SetActive(value: true);
		}

		protected new void Start()
		{
			base.Start();
			base.animationManager.commonAnimationEvent += AnimationManager_commonAnimationEvent;
		}

		private void AnimationManager_commonAnimationEvent(TrackEntry entry)
		{
			if (entry != null)
			{
				entry.Event += delegate(TrackEntry state, Event trackIndex)
				{
					if (trackIndex.Data.Name == "footstep" && base.currentFootstepMaterial != null)
					{
						base.currentFootstepMaterial.playFootstepAudio(base.footstepPlayer);
					}
				};
			}
		}
	}
}
