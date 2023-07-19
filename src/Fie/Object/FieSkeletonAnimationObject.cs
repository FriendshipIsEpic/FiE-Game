namespace Fie.Object
{
	public class FieSkeletonAnimationObject
	{
		private int _trackID;

		private string _animationName;

		private float _animationSpeed = 1f;

		public int trackID => _trackID;

		public string animationName => _animationName;

		public float animationSpeed
		{
			get
			{
				return _animationSpeed;
			}
			set
			{
				_animationSpeed = value;
			}
		}

		public FieSkeletonAnimationObject(int trackID, string animationName)
		{
			_trackID = trackID;
			_animationName = animationName;
		}
	}
}
