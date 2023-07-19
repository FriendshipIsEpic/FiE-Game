// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// GameDataEditor.GDERotateGODemo
using UnityEngine;

namespace GameDataEditor
{
	public class GDERotateGODemo : MonoBehaviour
	{
		public float Speed = 1f;

		public Vector3 rotateVec3;

		private void Update()
		{
			base.transform.Rotate(rotateVec3 * Time.deltaTime * Speed);
		}
	}
}