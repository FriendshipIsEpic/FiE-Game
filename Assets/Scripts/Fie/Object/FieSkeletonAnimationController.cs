using Spine;
using Spine.Unity;
using System.Collections.Generic;

namespace Fie.Object
{
	public class FieSkeletonAnimationController
	{
		public delegate void CommonAnimationEventDelegate(TrackEntry entry);

		private SkeletonUtility _skeletonUtility;

		private Dictionary<int, FieSkeletonAnimationObject> _animationObjectList = new Dictionary<int, FieSkeletonAnimationObject>();

		private Dictionary<int, TrackEntry> _currentAnimationEntries = new Dictionary<int, TrackEntry>();

		private Dictionary<int, TrackEntry> _currentEntryList = new Dictionary<int, TrackEntry>();

		public event CommonAnimationEventDelegate commonAnimationEvent;

		public FieSkeletonAnimationController(SkeletonUtility baseSkeleton, FieAnimationContainerBase animationContainer)
		{
			_skeletonUtility = baseSkeleton;
			_animationObjectList = animationContainer.getAnimationList();
		}

		public void SetAnimationTimeScale(float timeScale = 1f)
		{
			if (_skeletonUtility != null)
			{
				((SkeletonAnimation)_skeletonUtility.skeletonAnimation).timeScale = timeScale;
			}
		}

		public void AddAnimationContainer(FieAnimationContainerBase animationContainer)
		{
			if (animationContainer != null)
			{
				foreach (KeyValuePair<int, FieSkeletonAnimationObject> animation in animationContainer.getAnimationList())
				{
					_animationObjectList[animation.Key] = animation.Value;
				}
			}
		}

		public TrackEntry SetAnimation(int animationId, bool isLoop = false, bool isForceSet = false)
		{
			if (!_animationObjectList.ContainsKey(animationId))
			{
				return null;
			}
			return SetAnimation(_animationObjectList[animationId], isLoop, isForceSet);
		}

		public FieSkeletonAnimationObject GetAnimationData(int animationId)
		{
			if (!_animationObjectList.ContainsKey(animationId))
			{
				return null;
			}
			return _animationObjectList[animationId];
		}

		public TrackEntry SetAnimation(FieSkeletonAnimationObject animation, bool isLoop = false, bool isForceSet = false)
		{
			if (animation == null)
			{
				return null;
			}
			bool flag = false;
			if (!isForceSet)
			{
				foreach (KeyValuePair<int, TrackEntry> currentAnimationEntry in _currentAnimationEntries)
				{
					if (currentAnimationEntry.Key == animation.trackID && currentAnimationEntry.Value.Animation.name == animation.animationName)
					{
						flag = true;
					}
				}
			}
			if (flag)
			{
				return null;
			}
			if (((SkeletonAnimation)_skeletonUtility.skeletonAnimation).state == null)
			{
				return null;
			}
			TrackEntry trackEntry = ((SkeletonAnimation)_skeletonUtility.skeletonAnimation).state.SetAnimation(animation.trackID, animation.animationName, isLoop);
			if (trackEntry == null)
			{
				return null;
			}
			_currentAnimationEntries[animation.trackID] = trackEntry;
			_currentAnimationEntries.Remove(animation.trackID);
			_currentAnimationEntries.Add(animation.trackID, trackEntry);
			if (this.commonAnimationEvent != null)
			{
				this.commonAnimationEvent(trackEntry);
			}
			return trackEntry;
		}

		public bool IsEndAnimation(int trackID)
		{
			if (_currentAnimationEntries.ContainsKey(trackID) && _currentAnimationEntries[trackID] != null)
			{
				return _currentAnimationEntries[trackID].animationEnd <= _currentAnimationEntries[trackID].AnimationTime;
			}
			return true;
		}

		public bool SetAnimationTimescale(int animationID, float timescale)
		{
			if (!_animationObjectList.ContainsKey(animationID))
			{
				return false;
			}
			bool result = false;
			foreach (KeyValuePair<int, TrackEntry> currentAnimationEntry in _currentAnimationEntries)
			{
				if (currentAnimationEntry.Value.Animation.name == _animationObjectList[animationID].animationName)
				{
					currentAnimationEntry.Value.TimeScale = timescale;
					result = true;
				}
			}
			return result;
		}

		public void ResetAllAnimationTimescale()
		{
			if (_currentAnimationEntries.Count > 0)
			{
				foreach (KeyValuePair<int, TrackEntry> currentAnimationEntry in _currentAnimationEntries)
				{
					currentAnimationEntry.Value.TimeScale = 1f;
				}
			}
		}

		public void UnbindAnimation(int animationTrack = -1)
		{
			if (animationTrack >= 0)
			{
				TrackEntry current = ((SkeletonAnimation)_skeletonUtility.skeletonAnimation).state.GetCurrent(animationTrack);
				if (current != null)
				{
					((SkeletonAnimation)_skeletonUtility.skeletonAnimation).state.ClearTrack(animationTrack);
				}
			}
			else
			{
				((SkeletonAnimation)_skeletonUtility.skeletonAnimation).state.ClearTracks();
			}
			if (_currentAnimationEntries.ContainsKey(animationTrack))
			{
				_currentAnimationEntries.Remove(animationTrack);
			}
		}

		public string GetCurrentAnimationName(int animationTrack)
		{
			if (((SkeletonAnimation)_skeletonUtility.skeletonAnimation).state == null)
			{
				return string.Empty;
			}
			TrackEntry current = ((SkeletonAnimation)_skeletonUtility.skeletonAnimation).state.GetCurrent(animationTrack);
			if (current == null)
			{
				return string.Empty;
			}
			return current.ToString();
		}

		public TrackEntry SetAnimationChain(int firstAnimationID, int secondAnimationID, bool isLoop = false, bool isForceSet = false)
		{
			if (!_animationObjectList.ContainsKey(firstAnimationID))
			{
				return null;
			}
			if (!_animationObjectList.ContainsKey(secondAnimationID))
			{
				return null;
			}
			TrackEntry trackEntry = SetAnimation(firstAnimationID, isForceSet);
			if (trackEntry == null)
			{
				return null;
			}
			trackEntry.Complete += delegate
			{
				SetAnimation(secondAnimationID, isLoop);
			};
			return trackEntry;
		}
	}
}
