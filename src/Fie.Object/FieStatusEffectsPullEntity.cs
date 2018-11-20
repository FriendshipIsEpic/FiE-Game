using System;
using UnityEngine;

namespace Fie.Object
{
	[Serializable]
	public class FieStatusEffectsPullEntity : FieStatusEffectEntityBase
	{
		public float x;

		public float y;

		public float z;

		public float pullDuration;

		public Vector3 pullPosition
		{
			get
			{
				return new Vector3(x, y, z);
			}
			set
			{
				x = value.x;
				y = value.y;
				z = value.z;
			}
		}
	}
}
