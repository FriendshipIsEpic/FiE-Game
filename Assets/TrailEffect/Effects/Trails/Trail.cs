using System;
using UnityEngine;

namespace PigeonCoopToolkit.Effects.Trails
{
	[AddComponentMenu("Pigeon Coop Toolkit/Effects/Trail")]
	public class Trail : TrailRenderer_Base
	{
		public float MinVertexDistance = 0.1f;

		public int MaxNumberOfPoints = 50;

		private Vector3 _lastPosition;

		private float _distanceMoved;

		public void OnEnable()
		{
			base.Start();
			base.ClearSystem(true);
			this._lastPosition = this._t.position;
		}

		protected override void Update()
		{
			if (this._emit)
			{
				this._distanceMoved += Vector3.Distance(this._t.position, this._lastPosition);
				if (this._distanceMoved != 0f && this._distanceMoved >= this.MinVertexDistance)
				{
					base.AddPoint(new PCTrailPoint(), this._t.position);
					this._distanceMoved = 0f;
				}
				this._lastPosition = this._t.position;
			}
			base.Update();
		}

		protected override void OnStartEmit()
		{
			this._lastPosition = this._t.position;
			this._distanceMoved = 0f;
		}

		protected override void Reset()
		{
			base.Reset();
			this.MinVertexDistance = 0.1f;
		}

		protected override void OnTranslate(Vector3 t)
		{
			this._lastPosition += t;
		}

		protected override int GetMaxNumberOfPoints()
		{
			return this.MaxNumberOfPoints;
		}
	}
}
