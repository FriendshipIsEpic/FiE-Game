using UnityEngine;

namespace Fie.LevelObject
{
	public class FieLevelObjectGlowInsect : MonoBehaviour
	{
		private enum PathCalcType
		{
			Increase = 1,
			Decrease = -1
		}

		[SerializeField]
		private FieLevelObjectTracePath _parentPath;

		[SerializeField]
		private int _startPathIndex;

		[SerializeField]
		private float _moveSpeedPerSec = 1f;

		[SerializeField]
		private float _wiggleLoopPerSec = 0.5f;

		[SerializeField]
		private float _wiggleForceLoopPerSec = 0.25f;

		[SerializeField]
		private float _wiggleForce = 0.25f;

		[SerializeField]
		private float _changePathPointBoundingSize = 0.5f;

		[SerializeField]
		private PathCalcType _calcType = PathCalcType.Increase;

		private int _currentPathIndex;

		private int _nextPathIndex;

		private float _wiggleAngle;

		private float _wiggleForceSinWave;

		private void Start()
		{
			if (_parentPath.pathObject != null && _parentPath.pathObject.Count > 0)
			{
				_currentPathIndex = Mathf.Clamp(_startPathIndex, 0, _parentPath.pathObject.Count - 1);
				base.transform.position = _parentPath.pathObject[_currentPathIndex].transform.position;
				_nextPathIndex = getNextPathPoint(_currentPathIndex, (int)_calcType);
			}
		}

		private int getNextPathPoint(int currentIndex, int incrementCount = 1)
		{
			return Mathf.RoundToInt(Mathf.Repeat((float)(currentIndex + incrementCount), (float)(_parentPath.pathObject.Count - 1)));
		}

		private void Update()
		{
			if (_parentPath.pathObject != null && _parentPath.pathObject.Count > 0)
			{
				if (Vector3.Distance(base.transform.position, _parentPath.pathObject[_nextPathIndex].transform.position) <= _changePathPointBoundingSize)
				{
					_currentPathIndex = _nextPathIndex;
					_nextPathIndex = getNextPathPoint(_currentPathIndex, (int)_calcType);
				}
				Vector3 vector = _parentPath.pathObject[_nextPathIndex].transform.position - base.transform.position;
				vector.Normalize();
				Vector3 a = base.transform.position + vector;
				a += Quaternion.AngleAxis(_wiggleAngle, vector) * Vector3.up;
				Vector3 b = (a - base.transform.position).normalized * Mathf.Sin(_wiggleForceSinWave);
				vector = Vector3.Lerp(vector, b, _wiggleForce);
				base.transform.position += vector * _moveSpeedPerSec * Time.deltaTime;
				_wiggleForceSinWave += 90f * _wiggleForceLoopPerSec * Time.deltaTime;
				_wiggleAngle += 360f * _wiggleLoopPerSec * Time.deltaTime;
				_wiggleForceSinWave = Mathf.Repeat(_wiggleForceSinWave, 360f);
				_wiggleAngle = Mathf.Repeat(_wiggleAngle, 360f);
			}
		}
	}
}
