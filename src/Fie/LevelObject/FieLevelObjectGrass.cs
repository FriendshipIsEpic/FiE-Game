using UnityEngine;

namespace Fie.LevelObject
{
	public class FieLevelObjectGrass : MonoBehaviour
	{
		public const string GRASS_DENSITY_NAME = "_MaxTessellation";

		private void Awake()
		{
			switch (QualitySettings.GetQualityLevel())
			{
			case 3:
				break;
			default:
				UnityEngine.Object.Destroy(base.gameObject);
				break;
			case 2:
			{
				float float2 = GetComponent<Renderer>().material.GetFloat("_MaxTessellation");
				GetComponent<Renderer>().material.SetFloat("_MaxTessellation", float2 - 1f);
				break;
			}
			case 4:
			{
				float @float = GetComponent<Renderer>().material.GetFloat("_MaxTessellation");
				GetComponent<Renderer>().material.SetFloat("_MaxTessellation", @float + 1f);
				break;
			}
			}
		}
	}
}
