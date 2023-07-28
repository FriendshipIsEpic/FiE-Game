using UnityEngine;
using System.Collections;

namespace PlaygroundSplines {
	/// <summary>
	/// The PlaygroundSplineMesh class lets you create a mesh from a Playground Spline.
	/// </summary>
	[ExecuteInEditMode()]
	public class PlaygroundSplineMesh : MonoBehaviour {

		public PlaygroundSpline spline;

		[Range(2,1000)]
		public int points = 100;

		[Range(.01f, 100f)]
		public float width = 1f;

		public bool noise = false;
		[Range(.01f, 100f)]
		public float noiseStrength = 1f;
		public Vector2 noiseScale = new Vector2(1f,1f);
		public bool noiseDistribution;
		public AnimationCurve noiseDistributionL;
		public AnimationCurve noiseDistributionR;

		public bool followSurface = false;
		public bool followSurfaceRotation = false;
		public float surfaceOffset = .1f;
		public Vector3 meshUpDirection = Vector3.up;
		public Vector3 surfaceDirection = Vector3.down;

		int prevPoints;
		float prevWidth;
		bool prevNoise;
		float prevNoiseStrength;
		Vector2 prevNoiseScale;
		bool prevNoiseDistribution;
		bool prevFollowSurface;
		bool prevFollowSurfaceRotation;
		float prevSurfaceOffset;
		Vector3 prevMeshUpDirection;
		Vector3 prevSurfaceDirection;

		void OnEnable () 
		{
			if (noiseDistributionL == null || noiseDistributionL.keys.Length==0)
				SetAnimationCurveVals(ref noiseDistributionL);
			if (noiseDistributionR == null || noiseDistributionR.keys.Length==0)
				SetAnimationCurveVals(ref noiseDistributionR);
			if (GetComponent<Renderer>() == null)
				gameObject.AddComponent<MeshRenderer>();
			if (spline == null)
				spline = GetComponent<PlaygroundSpline>();
			if (spline != null)
				BuildSplineMesh(spline, points, width);
			SetVals();
		}

		void Update ()
		{
			if (NeedsUpdate ())
			{
				if (spline != null)
					BuildSplineMesh(spline, points, width);
				SetVals ();
			}
		}

		bool NeedsUpdate ()
		{
			return 	prevPoints!=points || 
					prevWidth!=width || 
					prevNoise!=noise || 
					noise && (prevNoiseScale!=noiseScale || prevNoiseStrength!=noiseStrength || prevNoiseDistribution!=noiseDistribution) || 
					prevFollowSurface!=followSurface || 
					prevFollowSurfaceRotation!=followSurfaceRotation ||
					prevSurfaceOffset!=surfaceOffset ||
					prevMeshUpDirection!=meshUpDirection ||
					prevSurfaceDirection!=surfaceDirection;
		}

		void SetVals ()
		{
			prevPoints = points;
			prevWidth = width;
			prevNoise = noise;
			prevNoiseStrength = noiseStrength;
			prevNoiseScale = noiseScale;
			prevNoiseDistribution = noiseDistribution;
			prevFollowSurface = followSurface;
			prevFollowSurfaceRotation = followSurfaceRotation;
			prevSurfaceOffset = surfaceOffset;
			prevMeshUpDirection = meshUpDirection;
			prevSurfaceDirection = surfaceDirection;
		}

		void SetAnimationCurveVals (ref AnimationCurve curve)
		{
			Keyframe[] reset = new Keyframe[2];
			reset[0].time = 0;
			reset[1].time = 1f;
			reset[0].value = 1f;
			reset[1].value = 1f;
			curve.keys = reset;
		}
		
		public void BuildSplineMesh (PlaygroundSpline spline, int points, float width) 
		{
			if (points<2)
				points = 2;
			int totalVertices = points*2;
			MeshFilter _mf = GetComponent<MeshFilter>()!=null? GetComponent<MeshFilter>() : gameObject.AddComponent<MeshFilter>();
			Mesh _m = new Mesh();
			Vector3[] verts = new Vector3[totalVertices];
			Vector2[] uvs = new Vector2[totalVertices];
			int[] tris = new int[(points-1)*6];

			Vector3 up = meshUpDirection;

			// Construct the mesh
			for (int i = 0; i<points; i++)
			{
				// Create a normalized time value
				float t = (i*1f)/(points-1);
				float tNext = ((i*1f)+1)/(points-1);
				if (t>=1 && !spline.Loop)
					t = .9999f;
				if (tNext>=1 && !spline.Loop)
					tNext = .99999f;

				// Get the current and next position from the spline on time
				Vector3 currentPosition = spline.GetPoint (t);
				Vector3 nextPosition = spline.GetPoint (tNext);

				// Raycast down to determine surface (especially practical for roads / rivers)
				if (followSurface || followSurfaceRotation)
				{
					RaycastHit hit;
					if (Physics.Raycast (currentPosition, surfaceDirection, out hit))
					{
						if (followSurfaceRotation)
							up = hit.normal;
						if (followSurface)
							currentPosition = hit.point + (hit.normal * surfaceOffset);
					}
					if (followSurface)
					{
						if (Physics.Raycast (nextPosition, surfaceDirection, out hit))
						{
							nextPosition = hit.point + (hit.normal * surfaceOffset);
						}
					}
				}

				// Calculate noise (if enabled)
				float noiseAmountL = noise? Mathf.PerlinNoise((t%1f)*noiseScale.x, 0)*noiseStrength : 0;
				float noiseAmountR = noise? Mathf.PerlinNoise((t%1f)*noiseScale.y, 0)*noiseStrength : 0;

				if (noise && noiseDistribution)
				{
					noiseAmountL *= noiseDistributionL.Evaluate(t);
					noiseAmountR *= noiseDistributionR.Evaluate(t);
				}

				// Create two width point references based on current and next position
				Vector3 dir = (Vector3.Cross(up, nextPosition - currentPosition)).normalized;
				Vector3 lPoint = currentPosition + dir * ((width/2)+noiseAmountL);
				Vector3 rPoint = currentPosition - dir * ((width/2)+noiseAmountR);

				// Draw debug
				Debug.DrawLine(lPoint, rPoint);

				verts[i*2] = lPoint;
				verts[(i*2)+1] = rPoint;
				uvs[i*2] = new Vector2(t,0);
				uvs[(i*2)+1] = new Vector2(t,1f);

				if (i>0)
				{
					int triIndex = (i-1)*6;
					int vertIndex = i*2;

					tris[triIndex] = vertIndex-2;
					tris[triIndex+1] = vertIndex-1;
					tris[triIndex+2] = vertIndex;
					tris[triIndex+3] = vertIndex;
					tris[triIndex+4] = vertIndex-1;
					tris[triIndex+5] = vertIndex+1;
				}
			}

			// Assign the data to the mesh
			_m.vertices = verts;
			_m.uv = uvs;
			_m.triangles = tris;
			_m.RecalculateNormals();

			// Assign the mesh to the MeshFilter
			_mf.mesh = _m;
		}
	}
}