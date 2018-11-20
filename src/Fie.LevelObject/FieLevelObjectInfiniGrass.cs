using UnityEngine;

namespace Fie.LevelObject
{
	public class FieLevelObjectInfiniGrass : MonoBehaviour
	{
		private void Awake()
		{
			switch (QualitySettings.GetQualityLevel())
			{
			case 1:
			case 2:
			case 3:
			case 4:
				break;
			default:
				UnityEngine.Object.Destroy(base.gameObject);
				break;
			}
		}
	}
}
