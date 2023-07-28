using System;
using System.Collections.Generic;
using PigeonCoopToolkit.Utillities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PigeonCoopToolkit.Effects.Trails
{
	public abstract class TrailRenderer_Base : MonoBehaviour
	{
		public PCTrailRendererData TrailData;

		public bool Emit;

		protected bool _emit;

		protected bool _noDecay;

		private PCTrail _activeTrail;

		private List<PCTrail> _fadingTrails;

		protected Transform _t;

		private static Dictionary<Material, List<PCTrail>> _matToTrailList;

		private static List<Mesh> _toClean;

		private static bool _hasRenderer;

		private static int GlobalTrailRendererCount;

		protected virtual void Awake()
		{
			GlobalTrailRendererCount++;
			if (GlobalTrailRendererCount == 1)
			{
				_matToTrailList = new Dictionary<Material, List<PCTrail>>();
				_toClean = new List<Mesh>();
			}
			_fadingTrails = new List<PCTrail>();
			_t = transform;
			_emit = Emit;
			if (_emit)
			{
				_activeTrail = new PCTrail(GetMaxNumberOfPoints());
				_activeTrail.IsActiveTrail = true;
				OnStartEmit();
			}
		}

		protected virtual void Start()
		{
		}

		protected virtual void LateUpdate()
		{
			if (_hasRenderer)
			{
				return;
			}
			_hasRenderer = true;
			foreach (KeyValuePair<Material, List<PCTrail>> current in _matToTrailList)
			{
				CombineInstance[] array = new CombineInstance[current.Value.Count];
				for (int i = 0; i < current.Value.Count; i++)
				{
					array[i] = new CombineInstance
					{
						mesh = current.Value[i].Mesh,
						subMeshIndex = 0,
						transform = Matrix4x4.identity
					};
				}
				Mesh mesh = new Mesh();
				mesh.CombineMeshes(array, true, false);
				_toClean.Add(mesh);
				DrawMesh(mesh, current.Key);
				current.Value.Clear();
			}
		}

		protected virtual void Update()
		{
			if (_hasRenderer)
			{
				_hasRenderer = false;
				if (_toClean.Count > 0)
				{
					for (int i = 0; i < _toClean.Count; i++)
					{
						Mesh mesh = _toClean[i];
						if (mesh != null)
						{
							if (Application.isEditor)
							{
								Object.DestroyImmediate(mesh, true);
							}
							else
							{
								Object.Destroy(mesh);
							}
						}
					}
				}
				_toClean.Clear();
			}
			if (!_matToTrailList.ContainsKey(TrailData.TrailMaterial))
			{
				_matToTrailList.Add(TrailData.TrailMaterial, new List<PCTrail>());
			}
			if (_activeTrail != null)
			{
				UpdatePoints(_activeTrail, Time.deltaTime);
				UpdateTrail(_activeTrail, Time.deltaTime);
				GenerateMesh(_activeTrail);
				_matToTrailList[TrailData.TrailMaterial].Add(_activeTrail);
			}
			for (int j = _fadingTrails.Count - 1; j >= 0; j--)
			{
				if (_fadingTrails[j] == null || !AnyElement(_fadingTrails[j].Points))
				{
					if (_fadingTrails[j] != null)
					{
						_fadingTrails[j].Dispose();
					}
					_fadingTrails.RemoveAt(j);
				}
				else
				{
					UpdatePoints(_fadingTrails[j], Time.deltaTime);
					UpdateTrail(_fadingTrails[j], Time.deltaTime);
					GenerateMesh(_fadingTrails[j]);
					_matToTrailList[TrailData.TrailMaterial].Add(_fadingTrails[j]);
				}
			}
			CheckEmitChange();
		}

		protected bool AnyElement(CircularBuffer<PCTrailPoint> InPoints)
		{
			for (int i = 0; i < InPoints.Count; i++)
			{
				PCTrailPoint pCTrailPoint = InPoints[i];
				if (pCTrailPoint.TimeActive() < TrailData.Lifetime)
				{
					return true;
				}
			}
			return false;
		}

		protected virtual void OnDestroy()
		{
			GlobalTrailRendererCount--;
			if (GlobalTrailRendererCount == 0)
			{
				if (_toClean != null && _toClean.Count > 0)
				{
					for (int i = 0; i < _toClean.Count; i++)
					{
						Mesh mesh = _toClean[i];
						if (mesh != null)
						{
							if (Application.isEditor)
							{
								Object.DestroyImmediate(mesh, true);
							}
							else
							{
								Object.Destroy(mesh);
							}
						}
					}
				}
				_toClean = null;
				_matToTrailList.Clear();
				_matToTrailList = null;
			}
			if (_activeTrail != null)
			{
				_activeTrail.Dispose();
				_activeTrail = null;
			}
			if (_fadingTrails != null)
			{
				for (int j = 0; j < _fadingTrails.Count; j++)
				{
					PCTrail pCTrail = _fadingTrails[j];
					if (pCTrail != null)
					{
						pCTrail.Dispose();
					}
				}
				_fadingTrails.Clear();
			}
		}

		protected virtual void OnStopEmit()
		{
		}

		protected virtual void OnStartEmit()
		{
		}

		protected virtual void OnTranslate(Vector3 t)
		{
		}

		protected abstract int GetMaxNumberOfPoints();

		protected virtual void Reset()
		{
			if (TrailData == null)
			{
				TrailData = new PCTrailRendererData();
			}
			TrailData.Lifetime = 1f;
			TrailData.UsingSimpleColor = false;
			TrailData.UsingSimpleSize = false;
			TrailData.ColorOverLife = new Gradient();
			TrailData.SimpleColorOverLifeStart = Color.white;
			TrailData.SimpleColorOverLifeEnd = new Color(1f, 1f, 1f, 0f);
			TrailData.SizeOverLife = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 0f));
			TrailData.SimpleSizeOverLifeStart = 1f;
			TrailData.SimpleSizeOverLifeEnd = 0f;
		}

		protected virtual void InitialiseNewPoint(PCTrailPoint newPoint)
		{
		}

		protected virtual void UpdateTrail(PCTrail trail, float deltaTime)
		{
		}

		protected void AddPoint(PCTrailPoint newPoint, Vector3 pos)
		{
			if (_activeTrail == null)
			{
				return;
			}
			newPoint.Position = pos;
			newPoint.PointNumber = ((_activeTrail.Points.Count == 0) ? 0 : (_activeTrail.Points[_activeTrail.Points.Count - 1].PointNumber + 1));
			InitialiseNewPoint(newPoint);
			newPoint.SetDistanceFromStart((_activeTrail.Points.Count == 0) ? 0f : (_activeTrail.Points[_activeTrail.Points.Count - 1].GetDistanceFromStart() + Vector3.Distance(_activeTrail.Points[_activeTrail.Points.Count - 1].Position, pos)));
			if (TrailData.UseForwardOverride)
			{
				newPoint.Forward = (TrailData.ForwardOverrideRelative ? _t.TransformDirection(TrailData.ForwardOverride.normalized) : TrailData.ForwardOverride.normalized);
			}
			_activeTrail.Points.Add(newPoint);
		}

		private void GenerateMesh(PCTrail trail)
		{
			trail.Mesh.Clear(false);
			Vector3 vector = (Camera.main != null) ? Camera.main.transform.forward : Vector3.forward;
			if (TrailData.UseForwardOverride)
			{
				vector = TrailData.ForwardOverride.normalized;
			}
			trail.activePointCount = NumberOfActivePoints(trail);
			if (trail.activePointCount < 2)
			{
				return;
			}
			int num = 0;
			for (int i = 0; i < trail.Points.Count; i++)
			{
				PCTrailPoint pCTrailPoint = trail.Points[i];
				float num2 = pCTrailPoint.TimeActive() / TrailData.Lifetime;
				if (pCTrailPoint.TimeActive() <= TrailData.Lifetime)
				{
					if (TrailData.UseForwardOverride && TrailData.ForwardOverrideRelative)
					{
						vector = pCTrailPoint.Forward;
					}
					Vector3 a = Vector3.zero;
					a = i < trail.Points.Count - 1 ? Vector3.Cross((trail.Points[i + 1].Position - pCTrailPoint.Position).normalized, vector).normalized : Vector3.Cross((pCTrailPoint.Position - trail.Points[i - 1].Position).normalized, vector).normalized;
					Color color = TrailData.StretchColorToFit ? (TrailData.UsingSimpleColor ? Color.Lerp(TrailData.SimpleColorOverLifeStart, TrailData.SimpleColorOverLifeEnd, 1f - num / (float)trail.activePointCount / 2f) : TrailData.ColorOverLife.Evaluate(1f - num / (float)trail.activePointCount / 2f)) : (TrailData.UsingSimpleColor ? Color.Lerp(TrailData.SimpleColorOverLifeStart, TrailData.SimpleColorOverLifeEnd, num2) : TrailData.ColorOverLife.Evaluate(num2));
					float d = TrailData.StretchSizeToFit ? (TrailData.UsingSimpleSize ? Mathf.Lerp(TrailData.SimpleSizeOverLifeStart, TrailData.SimpleSizeOverLifeEnd, 1f - num / (float)trail.activePointCount / 2f) : TrailData.SizeOverLife.Evaluate(1f - num / (float)trail.activePointCount / 2f)) : (TrailData.UsingSimpleSize ? Mathf.Lerp(TrailData.SimpleSizeOverLifeStart, TrailData.SimpleSizeOverLifeEnd, num2) : TrailData.SizeOverLife.Evaluate(num2));
					trail.verticies[num] = pCTrailPoint.Position + a * d;
					if (TrailData.MaterialTileLength <= 0f)
					{
						trail.uvs[num] = new Vector2(num / (float)trail.activePointCount / 2f, 0f);
					}
					else
					{
						trail.uvs[num] = new Vector2(pCTrailPoint.GetDistanceFromStart() / TrailData.MaterialTileLength, 0f);
					}
					trail.normals[num] = vector;
					trail.colors[num] = color;
					num++;
					trail.verticies[num] = pCTrailPoint.Position - a * d;
					if (TrailData.MaterialTileLength <= 0f)
					{
						trail.uvs[num] = new Vector2(num / (float)trail.activePointCount / 2f, 1f);
					}
					else
					{
						trail.uvs[num] = new Vector2(pCTrailPoint.GetDistanceFromStart() / TrailData.MaterialTileLength, 1f);
					}
					trail.normals[num] = vector;
					trail.colors[num] = color;
					num++;
				}
			}
			Vector2 v = trail.verticies[num - 1];
			for (int j = num; j < trail.verticies.Length; j++)
			{
				trail.verticies[j] = v;
			}
			int num3 = 0;
			for (int k = 0; k < 2 * (trail.activePointCount - 1); k++)
			{
				if (k % 2 == 0)
				{
					trail.indicies[num3] = k;
					num3++;
					trail.indicies[num3] = k + 1;
					num3++;
					trail.indicies[num3] = k + 2;
				}
				else
				{
					trail.indicies[num3] = k + 2;
					num3++;
					trail.indicies[num3] = k + 1;
					num3++;
					trail.indicies[num3] = k;
				}
				num3++;
			}
			int num4 = trail.indicies[num3 - 1];
			for (int l = num3; l < trail.indicies.Length; l++)
			{
				trail.indicies[l] = num4;
			}
			trail.Mesh.vertices = trail.verticies;
			trail.Mesh.SetIndices(trail.indicies, MeshTopology.Triangles, 0);
			trail.Mesh.uv = trail.uvs;
			trail.Mesh.normals = trail.normals;
			trail.Mesh.colors = trail.colors;
		}

		private void DrawMesh(Mesh trailMesh, Material trailMaterial)
		{
			Graphics.DrawMesh(trailMesh, Matrix4x4.identity, trailMaterial, gameObject.layer);
		}

		private void UpdatePoints(PCTrail line, float deltaTime)
		{
			for (int i = 0; i < line.Points.Count; i++)
			{
				line.Points[i].Update(_noDecay ? 0f : deltaTime);
			}
		}

		[Obsolete("UpdatePoint is deprecated, you should instead override UpdateTrail and loop through the individual points yourself (See Smoke or Smoke Plume scripts for how to do this).", true)]
		protected virtual void UpdatePoint(PCTrailPoint pCTrailPoint, float deltaTime)
		{
		}

		private void CheckEmitChange()
		{
			if (_emit != Emit)
			{
				_emit = Emit;
				if (_emit)
				{
					if (_activeTrail == null || _activeTrail.NumPoints != GetMaxNumberOfPoints())
					{
						if (_activeTrail != null)
						{
							_activeTrail.Dispose();
						}
						_activeTrail = new PCTrail(GetMaxNumberOfPoints());
					}
					else
					{
						_activeTrail.Points.Clear();
					}
					_activeTrail.IsActiveTrail = true;
					OnStartEmit();
				}
				else
				{
					OnStopEmit();
					_activeTrail.IsActiveTrail = false;
					_fadingTrails.Add(_activeTrail);
				}
			}
		}

		private int NumberOfActivePoints(PCTrail line)
		{
			int num = 0;
			for (int i = 0; i < line.Points.Count; i++)
			{
				if (line.Points[i].TimeActive() < TrailData.Lifetime)
				{
					num++;
				}
			}
			return num;
		}

		[ContextMenu("Toggle inspector size input method")]
		protected void ToggleSizeInputStyle()
		{
			TrailData.UsingSimpleSize = !TrailData.UsingSimpleSize;
		}

		[ContextMenu("Toggle inspector color input method")]
		protected void ToggleColorInputStyle()
		{
			TrailData.UsingSimpleColor = !TrailData.UsingSimpleColor;
		}

		public void LifeDecayEnabled(bool enabled)
		{
			_noDecay = !enabled;
		}

		public void Translate(Vector3 t)
		{
			if (_activeTrail != null)
			{
				for (int i = 0; i < _activeTrail.Points.Count; i++)
				{
					_activeTrail.Points[i].Position += t;
				}
			}
			if (_fadingTrails != null)
			{
				for (int j = 0; j < _fadingTrails.Count; j++)
				{
					PCTrail pCTrail = _fadingTrails[j];
					if (pCTrail != null)
					{
						for (int k = 0; k < pCTrail.Points.Count; k++)
						{
							pCTrail.Points[k].Position += t;
						}
					}
				}
			}
			OnTranslate(t);
		}

		public void CreateTrail(Vector3 from, Vector3 to, float distanceBetweenPoints)
		{
			float num = Vector3.Distance(from, to);
			Vector3 normalized = (to - from).normalized;
			float num2 = 0f;
			CircularBuffer<PCTrailPoint> circularBuffer = new CircularBuffer<PCTrailPoint>(GetMaxNumberOfPoints());
			int num3 = 0;
			while (num2 < num)
			{
				PCTrailPoint pCTrailPoint = new PCTrailPoint();
				pCTrailPoint.PointNumber = num3;
				pCTrailPoint.Position = from + normalized * num2;
				circularBuffer.Add(pCTrailPoint);
				InitialiseNewPoint(pCTrailPoint);
				num3++;
				if (distanceBetweenPoints <= 0f)
				{
					break;
				}
				num2 += distanceBetweenPoints;
			}
			PCTrailPoint pCTrailPoint2 = new PCTrailPoint();
			pCTrailPoint2.PointNumber = num3;
			pCTrailPoint2.Position = to;
			circularBuffer.Add(pCTrailPoint2);
			InitialiseNewPoint(pCTrailPoint2);
			PCTrail pCTrail = new PCTrail(GetMaxNumberOfPoints());
			pCTrail.Points = circularBuffer;
			_fadingTrails.Add(pCTrail);
		}

		public void ClearSystem(bool emitState)
		{
			if (_fadingTrails != null)
			{
				for (int i = 0; i < _fadingTrails.Count; i++)
				{
					PCTrail pCTrail = _fadingTrails[i];
					if (pCTrail != null && pCTrail != _activeTrail)
					{
						pCTrail.Dispose();
					}
				}
				_fadingTrails.Clear();
			}
			Emit = emitState;
			_emit = !emitState;
			CheckEmitChange();
		}

		public int NumSegments()
		{
			int num = 0;
			if (_activeTrail != null && NumberOfActivePoints(_activeTrail) != 0)
			{
				num++;
			}
			return num + _fadingTrails.Count;
		}
	}
}
