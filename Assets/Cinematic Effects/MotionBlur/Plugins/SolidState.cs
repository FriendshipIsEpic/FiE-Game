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
using UnityEngine;
using UnityEngine.Profiling;
#if !UNITY_4
using UnityEngine.Rendering;
#endif

namespace AmplifyMotion
{
internal class SolidState : AmplifyMotion.MotionState
{
	public MeshRenderer m_meshRenderer;

	public Matrix4x4 m_prevLocalToWorld;
	public Matrix4x4 m_currLocalToWorld;

	public Vector3 m_lastPosition;
	public Quaternion m_lastRotation;
	public Vector3 m_lastScale;

	private Mesh m_mesh;

	private MaterialDesc[] m_sharedMaterials;

	public bool m_moved = false;
	private bool m_wasVisible;

	private static HashSet<AmplifyMotionObjectBase> m_uniqueWarnings = new HashSet<AmplifyMotionObjectBase>();

	public SolidState( AmplifyMotionCamera owner, AmplifyMotionObjectBase obj )
		: base( owner, obj )
	{
		m_meshRenderer = m_obj.GetComponent<MeshRenderer>();
	}

	internal override void Initialize()
	{
		MeshFilter meshFilter = m_obj.GetComponent<MeshFilter>();
		if ( meshFilter == null || meshFilter.mesh == null )
		{
			if ( !m_uniqueWarnings.Contains( m_obj ) )
			{
				Debug.LogWarning( "[AmplifyMotion] Invalid MeshFilter/Mesh in object " + m_obj.name + ". Skipping." );
				m_uniqueWarnings.Add( m_obj );
			}
			m_error = true;
			return;
		}

		base.Initialize();

		m_mesh = meshFilter.mesh;

		m_sharedMaterials = ProcessSharedMaterials( m_meshRenderer.sharedMaterials );

		m_wasVisible = false;
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

		Profiler.BeginSample( "Solid.Update" );

		if ( !starting && m_wasVisible )
			m_prevLocalToWorld = m_currLocalToWorld;

		m_moved = true;
		if ( !m_owner.Overlay )
		{
			Vector3 position = m_transform.position;
			Quaternion rotation = m_transform.rotation;
			Vector3 scale = m_transform.lossyScale;

			m_moved = starting ||
				VectorChanged( position, m_lastPosition ) ||
				RotationChanged( rotation, m_lastRotation ) ||
				VectorChanged( scale, m_lastScale );

			if ( m_moved )
			{
				m_lastPosition = position;
				m_lastRotation = rotation;
				m_lastScale = scale;
			}
		}

		m_currLocalToWorld = m_transform.localToWorldMatrix;

		if ( starting || !m_wasVisible )
			m_prevLocalToWorld = m_currLocalToWorld;

		m_wasVisible = m_meshRenderer.isVisible;

		Profiler.EndSample();
	}

#if UNITY_4
	internal override void RenderVectors( Camera camera, float scale, AmplifyMotion.Quality quality )
	{
		if ( m_initialized && !m_error && m_meshRenderer.isVisible )
		{
			Profiler.BeginSample( "Solid.Render" );

			bool mask = ( m_owner.Instance.CullingMask & ( 1 << m_obj.gameObject.layer ) ) != 0;
			if ( !mask || ( mask && m_moved ) )
			{
				const float rcp255 = 1 / 255.0f;
				int objectId = mask ? m_owner.Instance.GenerateObjectId( m_obj.gameObject ) : 255;

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
						m_owner.Instance.SolidVectorsMaterial.mainTexture = matDesc.material.mainTexture;
						if ( matDesc.cutoff )
							m_owner.Instance.SolidVectorsMaterial.SetFloat( "_Cutoff", matDesc.material.GetFloat( "_Cutoff" ) );
					}

					if ( m_owner.Instance.SolidVectorsMaterial.SetPass( pass ) )
						Graphics.DrawMeshNow( m_mesh, m_transform.localToWorldMatrix, i );
				}
			}

			Profiler.EndSample();
		}
	}
#else
	internal override void RenderVectors( Camera camera, CommandBuffer renderCB, float scale, AmplifyMotion.Quality quality )
	{
		if ( m_initialized && !m_error && m_meshRenderer.isVisible )
		{
			Profiler.BeginSample( "Solid.Render" );

			bool mask = ( m_owner.Instance.CullingMask & ( 1 << m_obj.gameObject.layer ) ) != 0;
			if ( !mask || ( mask && m_moved ) )
			{
				const float rcp255 = 1 / 255.0f;
				int objectId = mask ? m_owner.Instance.GenerateObjectId( m_obj.gameObject ) : 255;

				Matrix4x4 prevModelViewProj;
				if ( m_obj.FixedStep )
					prevModelViewProj = m_owner.PrevViewProjMatrixRT * m_currLocalToWorld;
				else
					prevModelViewProj = m_owner.PrevViewProjMatrixRT * m_prevLocalToWorld;

				renderCB.SetGlobalMatrix( "_AM_MATRIX_PREV_MVP", prevModelViewProj );
				renderCB.SetGlobalFloat( "_AM_OBJECT_ID", objectId * rcp255 );
				renderCB.SetGlobalFloat( "_AM_MOTION_SCALE", mask ? scale : 0 );

				// TODO: cache property blocks

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

					renderCB.DrawMesh( m_mesh, m_transform.localToWorldMatrix, m_owner.Instance.SolidVectorsMaterial, i, pass, matDesc.propertyBlock );
				}
			}

			Profiler.EndSample();
		}
	}
#endif
}
}
