using UnityEngine;

namespace Fie.Utility
{
	[RequireComponent(typeof(AudioSource))]
	public class FieBgmBehaviour : MonoBehaviour
	{
		[SerializeField]
		private float _startTime;

		[SerializeField]
		private float _loopTime;

		[SerializeField]
		private float _endTime;

		private AudioSource _currentBgmSource;

		private void Awake()
		{
			_currentBgmSource = GetComponent<AudioSource>();
		}

		private void Update()
		{
			if (_currentBgmSource != null && _currentBgmSource.isPlaying && _currentBgmSource.time >= _endTime)
			{
				_currentBgmSource.time = _loopTime;
			}
		}
	}
}
