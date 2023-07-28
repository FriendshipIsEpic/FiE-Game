// Amplify Motion - Full-scene Motion Blur for Unity Pro
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4  || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9
#define UNITY_4
#endif
#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4  || UNITY_5_5 || UNITY_5_6 || UNITY_5_7 || UNITY_5_8 || UNITY_5_9
#define UNITY_5
#endif

using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Profiling;
#if !UNITY_4
using UnityEngine.Rendering;
#endif

namespace AmplifyMotion
{
internal class ClothState : AmplifyMotion.MotionState
{
#if UNITY_4
	private InteractiveCloth m_cloth;
#else
	private Cloth m_cloth;
#endif
	private Renderer m_renderer;

	public Matrix4x4 m_prevLocalToWorld;
	public Matrix4x4 m_currLocalToWorld;

	private int m_targetVertexCount;
	private int[] m_targetRemap;
	private Vector3[] m_prevVertices;
	private Vector3[] m_currVertices;

	private Mesh m_clonedMesh;

	private MaterialDesc[] m_sharedMaterials;

	private bool m_starting;
	private bool m_wasVisible;

	private static HashSet<AmplifyMotionObjectBase> m_uniqueWarnings = new HashSet<AmplifyMotionObjectBase>();

	public ClothState( AmplifyMotionCamera owner, AmplifyMotionObjectBase obj )
		: base( owner, obj )
	{
	#if UNITY_4
	    m_cloth = m_obj.GetComponent<InteractiveCloth>();
	#else
		m_cloth = m_obj.GetComponent<Cloth>();
	#endif
	}

	internal override void Initialize()
	{
		if ( m_cloth.vertices == null )
		{
			if ( !m_uniqueWarnings.Contains( m_obj ) )
			{
				Debug.LogWarning( "[AmplifyMotion] Invalid " + m_cloth.GetType().Name + " vertices in object " + m_obj.name + ". Skipping." );
				m_uniqueWarnings.Add( m_obj );
			}
			m_error = true;
			return;
		}

	#if UNITY_4
		Mesh clothMesh = m_cloth.mesh;
	#else
		SkinnedMeshRenderer skinnedRenderer = m_cloth.gameObject.GetComponent<SkinnedMeshRenderer>();
		Mesh clothMesh = skinnedRenderer.sharedMesh;
	#endif

		if ( clothMesh == null || clothMesh.vertices == null || clothMesh.triangles == null )
		{
			Debug.LogError( "[AmplifyMotion] Invalid Mesh on Cloth-enabled object " + m_obj.name );
			m_error = true;
			return;
		}

		base.Initialize();

		m_renderer = m_cloth.gameObject.GetComponent<Renderer>();

		int meshVertexCount = clothMesh.vertexCount;
		Vector3[] meshVertices = clothMesh.vertices;
		Vector2[] meshTexcoords = clothMesh.uv;
		int[] meshTriangles = clothMesh.triangles;

		m_targetRemap = new int[ meshVertexCount ];

		if ( m_cloth.vertices.Length == clothMesh.vertices.Length )
		{
			for ( int i = 0; i < meshVertexCount; i++ )
				m_targetRemap[ i ] = i;
		}
		else
		{
			// a) May contains duplicated verts, optimization/cleanup is required
			Dictionary<Vector3, int> dict = new Dictionary<Vector3, int>();
			int original, vertexCount = 0;

			for ( int i = 0; i < meshVertexCount; i++ )
			{
				if ( dict.TryGetValue( meshVertices[ i ], out original ) )
					m_targetRemap[ i ] = original;
				else
				{
					m_targetRemap[ i ] = vertexCount;
					dict.Add( meshVertices[ i ], vertexCount++ );
				}
			}

			// b) Tear is activated, creates extra verts (NOT SUPPORTED, POOL OF VERTS USED, NO ACCESS TO TRIANGLES)
		}

		m_targetVertexCount = meshVertexCount;
		m_prevVertices = new Vector3[ m_targetVertexCount ];
		m_currVertices = new Vector3[ m_targetVertexCount ];

		m_clonedMesh = new Mesh();
		m_clonedMesh.vertices = meshVertices;
		m_clonedMesh.normals = meshVertices;
		m_clonedMesh.uv = meshTexcoords;
		m_clonedMesh.triangles = meshTriangles;

		m_sharedMaterials = ProcessSharedMaterials( m_renderer.sharedMaterials );

		m_wasVisible = false;
	}

	internal override void Shutdown()
	{
		Mesh.Destroy( m_clonedMesh );
	}

#if UNITY_4
	internal override void UpdateTransform( bool starting )
#else
	internal override void UpdateTransform( CommandBuffer updateCB, bool starting )
#endif
	{
		if ( !m_initialized )
		{
			Initialize();
			return;
		}

		Profiler.BeginSample( "Cloth.Update" );

		if ( !starting && m_wasVisible )
			m_prevLocalToWorld = m_currLocalToWorld;

	    bool isVisible = m_renderer.isVisible;
		if ( !m_error && ( isVisible || starting ) )
		{
			if ( !starting && m_wasVisible )
				Array.Copy( m_currVertices, m_prevVertices, m_targetVertexCount );
		}

	#if UNITY_4
		m_currLocalToWorld = Matrix4x4.identity;
	#else
		m_currLocalToWorld = Matrix4x4.TRS( m_transform.position, m_transform.rotation, Vector3.one );
	#endif

		if ( starting || !m_wasVisible )
			m_prevLocalToWorld = m_currLocalToWorld;

		m_starting = starting;
		m_wasVisible = isVisible;

		Profiler.EndSample();
	}

#if UNITY_4
	internal override void RenderVectors( Camera camera, float scale, AmplifyMotion.Quality quality )
	{
		if ( m_initialized && !m_error && m_renderer.isVisible )
		{
			Profiler.BeginSample( "Cloth.Render" );

			const float rcp255 = 1 / 255.0f;
			bool mask = ( m_owner.Instance.CullingMask & ( 1 << m_obj.gameObject.layer ) ) != 0;
			int objectId = mask ? m_owner.Instance.GenerateObjectId( m_obj.gameObject ) : 255;

			Vector3[] clothVertices = m_cloth.vertices;
			for ( int i = 0; i < m_targetVertexCount; i++ )
				m_currVertices[ i ] = clothVertices[ m_targetRemap[ i ] ];

			if ( m_starting || !m_wasVisible )
				Array.Copy( m_currVertices, m_prevVertices, m_targetVertexCount );

			m_clonedMesh.vertices = m_currVertices;
			m_clonedMesh.normals = m_prevVertices;

			Matrix4x4 prevModelViewProj;
			if ( m_obj.FixedStep )
				prevModelViewProj = m_owner.PrevViewProjMatrixRT * m_currLocalToWorld;
			else
				prevModelViewProj = m_owner.PrevViewProjMatrixRT * m_prevLocalToWorld;

			Shader.SetGlobalMatrix( "_AM_MATRIX_PREV_MVP", prevModelViewProj );
			Shader.SetGlobalFloat( "_AM_OBJECT_ID", objectId * rcp255 );
			Shader.SetGlobalFloat( "_AM_MOTION_SCALE", mask ? scale : 0 );

			int qualityPass = ( quality == AmplifyMotion.Quality.Mobile ) ? 0 : 2;

			for ( int i = 0; i < m_sharedMaterials.Length; i++ )
			{
				MaterialDesc matDesc = m_sharedMaterials[ i ];
				int pass = qualityPass + ( matDesc.coverage ? 1 : 0 );

				if ( matDesc.coverage )
				{
					m_owner.Instance.ClothVectorsMaterial.mainTexture = matDesc.material.mainTexture;
					if ( matDesc.cutoff )
						m_owner.Instance.ClothVectorsMaterial.SetFloat( "_Cutoff", matDesc.material.GetFloat( "_Cutoff" ) );
				}

				if ( m_owner.Instance.ClothVectorsMaterial.SetPass( pass ) )
				{
				#if UNITY_4
					Graphics.DrawMeshNow( m_clonedMesh, Matrix4x4.identity, i );
				#else
					Graphics.DrawMeshNow( m_clonedMesh, m_currLocalToWorld, i );
				#endif
				}
			}

			Profiler.EndSample();
		}
	}
#else
	internal override void RenderVectors( Camera camera, CommandBuffer renderCB, float scale, AmplifyMotion.Quality quality )
	{
		if ( m_initialized && !m_error && m_renderer.isVisible )
		{
			Profiler.BeginSample( "Cloth.Render" );

			const float rcp255 = 1 / 255.0f;
			bool mask = ( m_owner.Instance.CullingMask & ( 1 << m_obj.gameObject.layer ) ) != 0;
			int objectId = mask ? m_owner.Instance.GenerateObjectId( m_obj.gameObject ) : 255;

			Vector3[] clothVertices = m_cloth.vertices;
			for ( int i = 0; i < m_targetVertexCount; i++ )
				m_currVertices[ i ] = clothVertices[ m_targetRemap[ i ] ];

			if ( m_starting || !m_wasVisible )
				Array.Copy( m_currVertices, m_prevVertices, m_targetVertexCount );

			m_clonedMesh.vertices = m_currVertices;
			m_clonedMesh.normals = m_prevVertices;

			Matrix4x4 prevModelViewProj;
			if ( m_obj.FixedStep )
				prevModelViewProj = m_owner.PrevViewProjMatrixRT * m_currLocalToWorld;
			else
				prevModelViewProj = m_owner.PrevViewProjMatrixRT * m_prevLocalToWorld;

			renderCB.SetGlobalMatrix( "_AM_MATRIX_PREV_MVP", prevModelViewProj );
			renderCB.SetGlobalFloat( "_AM_OBJECT_ID", objectId * rcp255 );
			renderCB.SetGlobalFloat( "_AM_MOTION_SCALE", mask ? scale : 0 );

			int qualityPass = ( quality == AmplifyMotion.Quality.Mobile ) ? 0 : 2;

			for ( int i = 0; i < m_sharedMaterials.Length; i++ )
			{
				MaterialDesc matDesc = m_sharedMaterials[ i ];
				int pass = qualityPass + ( matDesc.coverage ? 1 : 0 );

				if ( matDesc.coverage )
				{
					Texture mainTex = matDesc.material.mainTexture;
					if ( mainTex != null )
						matDesc.propertyBlock.SetTexture( "_MainTex", mainTex );
					if ( matDesc.cutoff )
						matDesc.propertyBlock.SetFloat( "_Cutoff", matDesc.material.GetFloat( "_Cutoff" ) );
				}

				renderCB.DrawMesh( m_clonedMesh, m_currLocalToWorld, m_owner.Instance.ClothVectorsMaterial, i, pass, matDesc.propertyBlock );
			}

			Profiler.EndSample();
		}
	}
#endif
}
}
