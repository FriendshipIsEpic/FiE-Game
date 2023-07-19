namespace Fie.Object
{
	public class FieEmotion
	{
		public enum EmotionStatus
		{
			AUTO,
			MANUAL
		}

		private FieGameCharacter _ownerCharacter;

		private int _nowDefaultEmoteAnimationID = -1;

		private FieSkeletonAnimationObject _latestAutoAnimation;

		private EmotionStatus _emotionState;

		public EmotionStatus emotionState => _emotionState;

		public FieEmotion(FieGameCharacter ownerCharacter)
		{
			_ownerCharacter = ownerCharacter;
		}

		public void StopAutoAnimation()
		{
			if (_latestAutoAnimation != null)
			{
				_ownerCharacter.animationManager.UnbindAnimation(_latestAutoAnimation.trackID);
			}
			_emotionState = EmotionStatus.MANUAL;
		}

		public void RestartAutoAnimation()
		{
			_emotionState = EmotionStatus.AUTO;
			if (_latestAutoAnimation != null)
			{
				SetEmoteAnimation(_latestAutoAnimation);
			}
		}

		public void SetDefaultEmoteAnimationID(int newDefaultEmoteAnimId)
		{
			FieSkeletonAnimationObject animationData = _ownerCharacter.animationManager.GetAnimationData(newDefaultEmoteAnimId);
			if (animationData != null)
			{
				_latestAutoAnimation = animationData;
				_nowDefaultEmoteAnimationID = newDefaultEmoteAnimId;
			}
		}

		public void RestoreEmotionFromDefaultData()
		{
			SetEmoteAnimation(_nowDefaultEmoteAnimationID);
		}

		public void SetEmoteAnimation(int emoteAnimId, bool isForceSet = false)
		{
			if (!(_ownerCharacter == null))
			{
				FieSkeletonAnimationObject animationData = _ownerCharacter.animationManager.GetAnimationData(emoteAnimId);
				if (animationData != null)
				{
					SetEmoteAnimation(animationData);
				}
			}
		}

		private void SetEmoteAnimation(FieSkeletonAnimationObject data, bool isForceSet = false)
		{
			if (data != null && data.trackID == 1 && !(_ownerCharacter == null))
			{
				_latestAutoAnimation = data;
				if (isForceSet || _emotionState == EmotionStatus.AUTO)
				{
					if (isForceSet)
					{
						_emotionState = EmotionStatus.AUTO;
						if (_ownerCharacter.voiceController != null)
						{
							_ownerCharacter.voiceController.Interrupt();
						}
					}
					_ownerCharacter.animationManager.SetAnimation(data, isLoop: true, isForceSet: true);
				}
			}
		}
	}
}
