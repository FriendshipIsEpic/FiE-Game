using System.Collections.Generic;
using UnityEngine;

namespace Fie.Object
{
	public class FieInGamePreloadEnemyContainer : MonoBehaviour
	{
		[SerializeField]
		public List<FieGameCharacter> _preloadList;
	}
}
