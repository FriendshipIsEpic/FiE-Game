using System.Collections.Generic;
using UnityEngine;

namespace Fie.LevelObject
{
	public class FieLevelObjectTracePath : MonoBehaviour
	{
		[SerializeField]
		private List<GameObject> _pathPoints;

		public List<GameObject> pathObject => _pathPoints;
	}
}
