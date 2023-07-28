// Amplify Motion - Full-scene Motion Blur for Unity Pro
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

#ifndef AMPLIFY_MOTION_SHARED_INCLUDED
#define AMPLIFY_MOTION_SHARED_INCLUDED

#include "UnityCG.cginc"

sampler2D _CameraDepthTexture;
float4x4 _AM_MATRIX_PREV_MVP;
float4 _AM_ZBUFFER_PARAMS;
float _AM_OBJECT_ID;
float _AM_MOTION_SCALE;
float _AM_MIN_VELOCITY;
float _AM_MAX_VELOCITY;
float _AM_RCP_TOTAL_VELOCITY;

sampler2D _AM_PREV_VERTEX_TEX;
sampler2D _AM_CURR_VERTEX_TEX;

float4 _AM_VERTEX_TEXEL_SIZE;
float4 _AM_VERTEX_TEXEL_HALFSIZE;

inline bool DepthTest( float4 screen_pos )
{
	const float epsilon = 0.001f;
	float3 uv = screen_pos.xyz / screen_pos.w;
	float behind = SAMPLE_DEPTH_TEXTURE_PROJ( _CameraDepthTexture, screen_pos );
#if defined( SHADER_API_OPENGL ) || defined( SHADER_API_GLES ) || defined( SHADER_API_GLES3 )
	float front = uv.z * 0.5 + 0.5;
#else
	float front = uv.z;
#endif
	return ( behind >= front - epsilon );
}

inline half4 SolidMotionVector( half4 pos_prev, half4 pos_curr, half obj_id )
{
	pos_prev = pos_prev / pos_prev.w;
	pos_curr = pos_curr / pos_curr.w;
	half4 motion = ( pos_curr - pos_prev ) * _AM_MOTION_SCALE;

	motion.z = length( motion.xy );
	motion.xy = ( motion.xy / motion.z ) * 0.5f + 0.5f;
	motion.z = ( motion.z < _AM_MIN_VELOCITY ) ? 0 : motion.z;
	motion.z = max( min( motion.z, _AM_MAX_VELOCITY ) - _AM_MIN_VELOCITY, 0 ) * _AM_RCP_TOTAL_VELOCITY;
	return half4( motion.xyz, obj_id );
}

inline half4 DeformableMotionVector( half3 motion, half obj_id )
{
	motion.z = length( motion.xy );
	motion.xy = ( motion.xy / motion.z ) * 0.5f + 0.5f;
	motion.z = ( motion.z < _AM_MIN_VELOCITY ) ? 0 : motion.z;
	motion.z = max( min( motion.z, _AM_MAX_VELOCITY ) - _AM_MIN_VELOCITY, 0 ) * _AM_RCP_TOTAL_VELOCITY;
	return half4( motion.xyz, obj_id );
}

#endif
