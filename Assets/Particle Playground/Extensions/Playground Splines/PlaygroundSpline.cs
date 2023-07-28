using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Collection of methods for working with splines.
/// This is based on the great learning tutorial Curves and Splines by Jasper Flick.
/// 
/// References:
/// http://catlikecoding.com/unity/tutorials/curves-and-splines/
/// http://answers.unity3d.com/questions/374333/positioning-an-object-on-a-spline-relative-to-play.html
/// </summary>
namespace PlaygroundSplines {
	
	/// <summary>
	/// Holds information about a spline and contains functions for working with the nodes and bezier handles.
	/// </summary>
	[ExecuteInEditMode()]
	public class PlaygroundSpline : MonoBehaviour {
		/// <summary>
		/// The list of nodes and bezier handles making the spline.
		/// </summary>
		[SerializeField]
		private List<Vector3> points = new List<Vector3>();
		/// <summary>
		/// The modes of the bezier handles.
		/// </summary>
		[SerializeField]
		private List<BezierControlPointMode> modes = new List<BezierControlPointMode>();
		/// <summary>
		/// Determines if the spline is looping.
		/// </summary>
		[SerializeField]
		private bool loop;
		
		/// <summary>
		/// The list of transform nodes to set positions live of an existing node.
		/// </summary>
		[HideInInspector] public List<TransformNode> transformNodes = new List<TransformNode>();
		/// <summary>
		/// Determines if the spline time should be reversed. If you'd like to physically reverse the arrays making the spline then call ReverseAllNodes().
		/// </summary>
		[HideInInspector] public bool reverse;
		/// <summary>
		/// The time offset of the spline.
		/// </summary>
		[HideInInspector] public float timeOffset;
		/// <summary>
		/// The position offset of the spline in relation to its transform.
		/// </summary>
		[HideInInspector] public Vector3 positionOffset;
		
		[HideInInspector] public Transform splineTransform;
		[HideInInspector] public Matrix4x4 splineTransformMx;
		[HideInInspector] public List<Transform> usedBy = new List<Transform>();
		[HideInInspector] public float fixedVelocityOnNewNode = .5f;
		[HideInInspector] public bool moveTransformsAsBeziers = false;
		[HideInInspector] public bool exportWithNodeStructure = false;
		
		// Gizmos
		public static bool drawSplinePreviews = true;
		[HideInInspector] public bool drawGizmo = true;
		[HideInInspector] public float bezierWidth = 2f;
		
		#if UNITY_EDITOR
		void OnDrawGizmos () {
			if (drawSplinePreviews && drawGizmo) {
				Color innerBezier = new Color(1f,1f,1f,1f);
				Color outerBezier = new Color(.5f,.5f,0,.2f);
				Vector3 p0 = ShowPoint(0);
				for (int i = 1; i < ControlPointCount; i += 3) {
					Vector3 p1 = ShowPoint(i);
					Vector3 p2 = ShowPoint(i + 1);
					Vector3 p3 = ShowPoint(i + 2);
					UnityEditor.Handles.DrawBezier(p0, p3, p1, p2, innerBezier, null, bezierWidth);
					UnityEditor.Handles.DrawBezier(p0, p3, p1, p2, outerBezier, null, bezierWidth*10f);
					p0 = p3;
				}
			}
		}
		Vector3 ShowPoint (int index) {
			return transformNodes[index].IsAvailable()? GetPoint(index)+positionOffset : splineTransform.TransformPoint(GetInversePoint(index)+positionOffset);
		}
		#endif
		
		Vector3 previousPosition;
		Quaternion previousRotation;
		Vector3 previousScale;
		bool isReady;
		
		public bool IsReady () {
			return isReady;
		}
		
		/// <summary>
		/// Adds a user to the spline. This helps keeping track of which objects are using the spline.
		/// </summary>
		/// <returns><c>true</c>, if user was added, <c>false</c> otherwise.</returns>
		/// <param name="">.</param>
		public bool AddUser (Transform thisTransform) {
			if (!usedBy.Contains(thisTransform)) {
				usedBy.Add (thisTransform);
				return true;
			}
			return false;
		}
		
		/// <summary>
		/// Removes a user from the spline. This helps keeping track of which objects are using the spline.
		/// </summary>
		/// <returns><c>true</c>, if user was removed, <c>false</c> otherwise.</returns>
		/// <param name="">.</param>
		public bool RemoveUser (Transform thisTransform) {
			if (usedBy.Contains(thisTransform)) {
				usedBy.Remove (thisTransform);
				return true;
			}
			return false;
		}
		
		/// <summary>
		/// Determines whether this spline has the user of passed in transform.
		/// </summary>
		/// <returns><c>true</c> if this spline has the user of the passed in transform; otherwise, <c>false</c>.</returns>
		/// <param name="thisTransform">This transform.</param>
		public bool HasUser (Transform thisTransform) {
			return usedBy.Contains (thisTransform);
		}
		
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="PlaygroundSplines.PlaygroundSpline"/> is set to loop.
		/// </summary>
		/// <value><c>true</c> if set to loop; otherwise, <c>false</c>.</value>
		public bool Loop {
			get {
				return loop;
			}
			set {
				loop = value;
				if (value == true && NodeCount>1) {
					modes[modes.Count - 1] = modes[0];
					SetControlPoint(0, points[0]);
				}
			}
		}
		
		
		/// <summary>
		/// Gets the control point count.
		/// </summary>
		/// <value>The control point count.</value>
		public int ControlPointCount {
			get {
				return points.Count;
			}
		}
		
		/// <summary>
		/// Gets the control point.
		/// </summary>
		/// <returns>The control point.</returns>
		/// <param name="index">Index.</param>
		public Vector3 GetControlPoint (int index) {
			return GetPoint(index);
		}
		
		/// <summary>
		/// Sets the control point and withdraws the offset.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="point">Point.</param>
		/// <param name="offset">Offset.</param>
		public void SetControlPoint (int index, Vector3 point, Vector3 offset) {
			SetControlPoint(index, point-offset);
		}
		
		/// <summary>
		/// Sets the control point.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="point">Position.</param>
		public void SetControlPoint (int index, Vector3 point) {
			if (index<0) index = 0;
			if (index % 3 == 0) {
				Vector3 delta = (point - GetPoint(index));
				Vector3 v;
				if (loop) {
					if (index == 0) {
						//if (!PointHasTransform(1))
						{v = GetPoint(1); SetPoint(1, v+delta);}
						//if (!PointHasTransform(points.Count-2))
						{v = GetPoint(points.Count-2); SetPoint(points.Count-2, v+delta);}
						if (moveTransformsAsBeziers || !PointHasTransform(points.Count-1))
						{SetPoint(points.Count-1, point);}
					} else 
					if (index == points.Count - 1) {
						//if (!PointHasTransform(0))
						{SetPoint(0, point);}
						//if (!PointHasTransform(1))
						{v = GetPoint(1); SetPoint(1, v+delta);}
						//if (!PointHasTransform(index-1))
						{v = GetPoint(index-1); SetPoint(index-1, v+delta);}
					} else {
						//if (!PointHasTransform(index-1))
						{v = GetPoint(index-1); SetPoint(index-1, v+delta);}
						//if (!PointHasTransform(index+1))
						{v = GetPoint(index+1); SetPoint(index+1, v+delta);}
					}
				} else {
					if (index > 0) {
						if (moveTransformsAsBeziers || !PointHasTransform(index-1))
						{v = GetPoint(index-1); SetPoint(index-1, v+delta);}
					}
					if (index + 1 < points.Count) {
						if (moveTransformsAsBeziers || !PointHasTransform(index+1))
						{v = GetPoint(index+1); SetPoint(index+1, v+delta);}
					}
				}
			}
			SetPoint(index, point);
			EnforceMode(index);
		}
		
		/// <summary>
		/// Sets all points from an array. Please ensure the same length of your passed in vectors as PlaygroundSpline.ControlPointCount.
		/// </summary>
		/// <param name="vectors">Vectors.</param>
		public void SetPoints (Vector3[] vectors) {
			if (vectors.Length!=points.Count) {
				Debug.Log ("Please ensure the same length of your passed in vectors ("+vectors.Length+") as the current points ("+points.Count+"). Use PlaygroundSpline.ControlPointCount to get the current count.");
				return;
			}
			for (int i = 0; i<points.Count; i++) {
				points[i] = vectors[i];
			}
		}
		
		public bool PointHasTransform (int index) {
			return transformNodes[index].IsAvailable();
		}
		
		/// <summary>
		/// Moves the entire spline separate from its transform component. Use this if you'd like to offset the spline from its transform separately from the positionOffset.
		/// </summary>
		/// <param name="translation">The amount to move the spline in Units.</param>
		public void TranslateSpline (Vector3 translation) {
			for (int i = 0; i<points.Count; i++) {
				points[i] += translation;
			}
		}
		
		public Vector3 GetTransformPosition () {
			return previousPosition;
		}
		public Quaternion GetTransformRotation () {
			return previousRotation;
		}
		public Vector3 GetTransformScale () {
			return previousScale;
		}
		
		public BezierControlPointMode GetControlPointMode (int index) {
			return modes[(index + 1) / 3];
		}
		
		public void SetControlPointMode (int index, BezierControlPointMode mode) {
			int modeIndex = (index + 1) / 3;
			modes[modeIndex] = mode;
			if (loop) {
				if (modeIndex == 0) {
					modes[modes.Count - 1] = mode;
				}
				else if (modeIndex == modes.Count - 1) {
					modes[0] = mode;
				}
			}
			EnforceMode(index);
		}
		
		private void EnforceMode (int index) {
			int modeIndex = (index+1) / 3;
			BezierControlPointMode mode = modes[modeIndex];
			if (mode == BezierControlPointMode.Free || !loop && (modeIndex == 0 || modeIndex == modes.Count - 1)) {
				return;
			}
			
			int middleIndex = modeIndex * 3;
			int fixedIndex, enforcedIndex;
			if (index <= middleIndex) {
				fixedIndex = middleIndex - 1;
				if (fixedIndex < 0) {
					fixedIndex = points.Count - 2;
				}
				enforcedIndex = middleIndex + 1;
				if (enforcedIndex >= points.Count) {
					enforcedIndex = 1;
				}
			}
			else {
				fixedIndex = middleIndex + 1;
				if (fixedIndex >= points.Count) {
					fixedIndex = 1;
				}
				enforcedIndex = middleIndex - 1;
				if (enforcedIndex < 0) {
					enforcedIndex = points.Count - 2;
				}
			}
			
			Vector3 middle = GetPoint(middleIndex);
			Vector3 enforcedTangent = middle - GetPoint(fixedIndex);
			if (mode == BezierControlPointMode.Aligned) {
				enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middle, GetPoint(enforcedIndex));
			}
			if (moveTransformsAsBeziers || !PointHasTransform(enforcedIndex))
				SetPoint(enforcedIndex, middle + enforcedTangent);
		}
		
		public int NodeCount {
			get {
				return (points.Count - 1) / 3;
			}
		}
		
		/// <summary>
		/// Get position from time.
		/// </summary>
		/// <returns>The point in world space.</returns>
		/// <param name="t">Time.</param>
		public Vector3 GetPoint (float t) {
			int i;
			if (reverse) {
				t = 1f-t;
				t = (t-timeOffset)%1f;
				if (t<0)
					t = 1f+t;
			} else t = (t+timeOffset)%1f;
			
			if (t >= 1f) {
				//t = 1f;
				i = points.Count - 4;
			}
			else {
				t = Mathf.Clamp01(t) * NodeCount;
				i = (int)t;
				t -= i;
				i *= 3;
			}
			return splineTransformMx.MultiplyPoint3x4(Bezier.GetPoint(GetInversePoint(i), GetInversePoint(i + 1), GetInversePoint(i + 2), GetInversePoint(i + 3), t)+positionOffset);
		}
		
		public Vector3 GetVelocity (float t) {
			int i;
			if (reverse)
				t = 1f-t;
			t = (t+timeOffset)%1f;
			if (t >= 1f) {
				t = 1f;
				i = points.Count - 4;
			}
			else {
				t = Mathf.Clamp01(t) * NodeCount;
				i = (int)t;
				t -= i;
				i *= 3;
			}
			return splineTransformMx.MultiplyPoint3x4(Bezier.GetFirstDerivative(GetInversePoint(i), GetInversePoint(i + 1), GetInversePoint(i + 2), GetInversePoint(i + 3), t)+positionOffset) - previousPosition;
		}
		
		/// <summary>
		/// Get position from node index in the spline. If the node consists of an available transform its position will be returned, otherwise the user-specified Vector3 position.
		/// </summary>
		/// <returns>The point in world space.</returns>
		/// <param name="index">Index.</param>
		public Vector3 GetPoint (int index) {
			if (transformNodes[index].IsAvailable())
				return transformNodes[index].GetPosition();
			else return points[index];
		}
		
		public Vector3 GetInversePoint (int index) {
			if (transformNodes[index].IsAvailable())
				return transformNodes[index].GetInvsersePosition();
			else return points[index];
		}
		
		public Vector3 GetPointWorldSpace (int index) {
			if (transformNodes[index].IsAvailable())
				return transformNodes[index].GetPosition();
			else return splineTransformMx.MultiplyPoint3x4(points[index]+positionOffset);
		}
		
		/// <summary>
		/// Sets a point to specified position.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="position">Position.</param>
		void SetPoint (int index, Vector3 position) {
			if (transformNodes[index].IsAvailable())
				transformNodes[index].SetPosition(position);
			else points[index] = position;
		}
		
		/// <summary>
		/// Translates a point.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="translation">Translation.</param>
		void TranslatePoint (int index, Vector3 translation) {
			if (transformNodes[index].IsAvailable())
				transformNodes[index].Translate(translation);
			else points[index] += translation;
		}
		
		// Calculates the best fitting time in the given interval
		private float CPOB(Vector3 aP, float aStart, float aEnd, int aSteps)
		{
			aStart = Mathf.Clamp01(aStart);
			aEnd = Mathf.Clamp01(aEnd);
			float step = (aEnd-aStart) / (float)aSteps;
			float Res = 0;
			float Ref = float.MaxValue;
			for (int i = 0; i < aSteps; i++)
			{
				float t = aStart + step*i;
				float L = (GetPoint(t)-aP).sqrMagnitude;
				if (L < Ref)
				{
					Ref = L;
					Res = t;
				}
			}
			return Res;
		}
		
		public float ClosestTimeFromPoint (Vector3 aP) {
			float t = CPOB(aP, 0, 1, 10);
			float delta = 1.0f / 10.0f;
			for (int i = 0; i < 4; i++)
			{
				t = CPOB(aP, t - delta, t + delta, 10);
				delta /= 9;
			}
			return t;
		}
		
		public Vector3 ClosestPointFromPosition (Vector3 aP) {
			return GetPoint(ClosestTimeFromPoint(aP));
		}
		
		public Vector3 GetDirection (float t) {
			return (GetPoint(t+.001f)-GetPoint(t)).normalized;
		}
		
		/// <summary>
		/// Adds a node at the last position of the node index.
		/// </summary>
		public void AddNode () {
			AddNode ((points.Count-1)/3);
		}
		
		/// <summary>
		/// Adds a node at specified node index.
		/// </summary>
		/// <param name="index">Index.</param>
		public void AddNode (int index) {
			int nodeIndex = index*3;
			Vector3 point = GetPoint(nodeIndex);
			Vector3 direction;
			if (index>0) {
				direction = GetPoint(nodeIndex)-GetPoint(nodeIndex-1);
			} else direction = GetPoint(nodeIndex+1)-GetPoint(nodeIndex);
			
			direction*=fixedVelocityOnNewNode;
			
			points.InsertRange(nodeIndex+1, new Vector3[3]);
			point += direction;
			points[nodeIndex+2] = point;
			point += direction;
			points[nodeIndex+1] = point;
			point += direction;
			points[nodeIndex+3] = point;
			
			transformNodes.InsertRange(nodeIndex+1, new TransformNode[]{new TransformNode(), new TransformNode(), new TransformNode()});
			
			BezierControlPointMode currentIndexMode = modes[index];
			modes.Insert (index, new BezierControlPointMode());
			modes[index] = currentIndexMode;
			EnforceMode(index);
			
			SetControlPoint((index+1)*3, GetPoint((index+1)*3));
			
			if (loop) {
				points[points.Count - 1] = points[0];
				modes[modes.Count - 1] = modes[0];
				EnforceMode(0);
			}
		}
		
		/// <summary>
		/// Removes the first node in the node index.
		/// </summary>
		public void RemoveFirst () {
			RemoveNode(0);
		}
		
		/// <summary>
		/// Removes the last node in the node index.
		/// </summary>
		public void RemoveLast () {
			RemoveNode((points.Count-1)/3);
		}
		
		/// <summary>
		/// Removes a node at specified node index.
		/// </summary>
		/// <param name="index">Index.</param>
		public void RemoveNode (int index) {
			index = Mathf.Clamp (index, 0, points.Count-1);
			int pointIndex = index*3;
			if (points.Count<=4) return;
			if (pointIndex<points.Count-1) {
				points.RemoveRange(pointIndex, 3);
				transformNodes.RemoveRange(pointIndex, 3);
			} else {
				points.RemoveRange(pointIndex-2, 3);
				transformNodes.RemoveRange(pointIndex-2, 3);
			}
			modes.RemoveAt (index);
			EnforceMode (index-1);
			if (index>0)
				SetControlPoint((index-1)*3, GetPoint((index-1)*3));
			else
				SetControlPoint(0, GetPoint(0));
		}
		
		/// <summary>
		/// Reverses all nodes in the node index.
		/// </summary>
		public void ReverseAllNodes () {
			points.Reverse();
			transformNodes.Reverse();
			modes.Reverse();
		}
		
		public void SwapNodes (int from, int to) {
			Vector3[] fromPoints = points.GetRange (from, 3).ToArray();
			Vector3[] toPoints = points.GetRange (to, 3).ToArray();
			TransformNode[] fromTnode = transformNodes.GetRange (from, 3).ToArray();
			TransformNode[] toTnode = transformNodes.GetRange (to, 3).ToArray();
			BezierControlPointMode fromMode = modes[from];
			BezierControlPointMode toMode = modes[to];
			
			for (int i = from; i<3; i++) {
				points[i] = toPoints[i];
				transformNodes[i] = toTnode[i];
			}
			for (int i = to; i<3; i++) {
				points[i] = fromPoints[i];
				transformNodes[i] = fromTnode[i];
			}
			modes[from] = toMode;
			modes[to] = fromMode;
		}
		
		/// <summary>
		/// Exports all nodes to Transform[]. Enable exportWithNodeStructure to parent each bezier handle to their node.
		/// </summary>
		/// <returns>A built-in array of Transforms.</returns>
		public Transform[] ExportToTransforms () {
			Transform[] transforms = new Transform[points.Count];
			for (int i = 0; i<points.Count; i++) {
				int iNode = (i+1)/3;
				int iBezier = i<3?0:(((i)%3))%2;
				bool iIsNode = i==0||i%3==0;
				transforms[i] = new GameObject(iIsNode?"Node "+iNode:"Node "+iNode+" - Bezier "+iBezier).transform;
				transforms[i].parent = splineTransform;
				transforms[i].position = splineTransform.TransformPoint(GetInversePoint(i)+positionOffset);
			}
			if (exportWithNodeStructure) {
				for (int i = 2; i<transforms.Length; i++) {
					int iBezier = i<3?0:(((i)%3))%2;
					bool iIsNode = i==0||i%3==0;
					if (!iIsNode)
						transforms[i].parent = transforms[iBezier==0?i+1:i-1];
				}
				transforms[1].parent = transforms[0];
			}
			return transforms;
		}
		
		/// <summary>
		/// Exports all nodes to Vector3[].
		/// </summary>
		/// <returns>A built-in array of Vector3</returns>
		public Vector3[] ExportToVector3 () {
			Vector3[] vectors = new Vector3[points.Count];
			for (int i = 0; i<points.Count; i++) {
				vectors[i] = GetPoint(i)+positionOffset;
			}
			return vectors;
		}
		
		/// <summary>
		/// Reset this Playground Spline. Two nodes and two bezier handles will be created. 
		/// </summary>
		public void Reset () {
			points = new List<Vector3> {
				new Vector3(1f, 0f, 0f),
				new Vector3(2f, 0f, 0f),
				new Vector3(3f, 0f, 0f),
				new Vector3(4f, 0f, 0f)
			};
			modes = new List<BezierControlPointMode> {
				BezierControlPointMode.Aligned,
				BezierControlPointMode.Aligned
			};
			transformNodes = new List<TransformNode> {
				new TransformNode(),
				new TransformNode(),
				new TransformNode(),
				new TransformNode()
			};
		}
		
		/*************************************************************************************************************************************************
			MonoBehaviours
		*************************************************************************************************************************************************/
		
		void OnEnable () {
			isReady = false;
			splineTransform = transform;
			SetMatrix();
		}
		
		void Update () {
			SetMatrix();
			for (int i = 0; i<transformNodes.Count; i++)
				transformNodes[i].Update(splineTransform);
		}
		
		void SetMatrix () {
			if (previousPosition!=splineTransform.position || previousRotation!=splineTransform.rotation || previousScale!=splineTransform.localScale)
				splineTransformMx.SetTRS(splineTransform.position, splineTransform.rotation, splineTransform.localScale);
			previousPosition = splineTransform.position;
			previousRotation = splineTransform.rotation;
			previousScale = splineTransform.localScale;
			isReady = true;
		}
	}
	
	/// <summary>
	/// Class for common bezier operations on a spline.
	/// </summary>
	public static class Bezier {
		
		public static Vector3 GetPoint (Vector3 p0, Vector3 p1, Vector3 p2, float t) {
			t = Mathf.Clamp01(t);
			float oneMinusT = 1f - t;
			return
				oneMinusT * oneMinusT * p0 +
					2f * oneMinusT * t * p1 +
					t * t * p2;
		}
		
		public static Vector3 GetFirstDerivative (Vector3 p0, Vector3 p1, Vector3 p2, float t) {
			return
				2f * (1f - t) * (p1 - p0) +
					2f * t * (p2 - p1);
		}
		
		public static Vector3 GetPoint (Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
			t = Mathf.Clamp01(t);
			float OneMinusT = 1f - t;
			return
				OneMinusT * OneMinusT * OneMinusT * p0 +
					3f * OneMinusT * OneMinusT * t * p1 +
					3f * OneMinusT * t * t * p2 +
					t * t * t * p3;
		}
		
		public static Vector3 GetFirstDerivative (Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
			t = Mathf.Clamp01(t);
			float oneMinusT = 1f - t;
			return
				3f * oneMinusT * oneMinusT * (p1 - p0) +
					6f * oneMinusT * t * (p2 - p1) +
					3f * t * t * (p3 - p2);
		}
	}
	
	[Serializable]
	public class TransformNode {
		public bool enabled;
		public Transform transform;
		bool isAvailable;
		Vector3 position;
		Vector3 inversePosition;
		Vector3 previousPosition;
		
		public bool Update (Transform splineTransform) {
			if (enabled && transform!=null) {
				previousPosition = position;
				position = transform.position;
				inversePosition = splineTransform.InverseTransformPoint(transform.position);
				isAvailable = true;
				return true;
			}
			isAvailable = false;
			return false;
		}
		
		public bool IsAvailable () {
			return enabled&&isAvailable;
		}
		
		public Vector3 GetPosition () {
			return position;
		}
		
		public Vector3 GetInvsersePosition () {
			return inversePosition;
		}
		
		public void SetPosition (Vector3 newPosition) {
			if (transform==null) return;
			transform.position = newPosition;
		}
		
		public void Translate (Vector3 translation) {
			if (transform==null) return;
			transform.position += translation;
		}
		
		public Vector3 GetPositionDelta () {
			return previousPosition-position;
		}
	}
	
	public enum SplineMode {
		Vector3,
		Transform
	}
	
	/// <summary>
	/// The bezier mode for a spline node. This controls how one bezier handle acts in relation to the other.
	/// </summary>
	public enum BezierControlPointMode {
		/// <summary>
		/// Align the angle between the two bezier handles but keep individual lengths. Has a differential smooth in and out angle.
		/// </summary>
		Aligned,
		/// <summary>
		/// Align the angle and length between the two bezier handles. Has an equally smooth in and out angle.
		/// </summary>
		Mirrored,
		/// <summary>
		/// Bezier handles are freely aligned without consideration to the other. Ables you to have sharp angles.
		/// </summary>
		Free
	}
}