// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/PKFx Depth Copy"
{
	Properties
	{
		_Flip("Set to 1 when in forward rendering.", Range(0, 1)) = 0.0
	}
	SubShader
	{
		// No culling or depth
		ZTest Always
		ZWrite On

		// Writes to a single-component texture (TextureFormat.Depth)
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				fixed2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			float _Flip;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv * (1 - _Flip) + _Flip * float2(v.uv.x, 1 - v.uv.y);
				return o;
			}

			sampler2D _CameraDepthTexture;

#if !defined(SHADER_API_D3D9) && !defined(SHADER_API_D3D11_9X)
			fixed frag(v2f i) : SV_Depth
			{
				fixed4 col = tex2D(_CameraDepthTexture, i.uv);
#if defined(UNITY_REVERSED_Z)
				return 1.0f - col.r;
#else
				return col.r;
#endif
			}
#else
			void frag(v2f i, out float4 dummycol:COLOR, out float depth : DEPTH)
			{
				fixed4 col = tex2D(_CameraDepthTexture, i.uv);
				dummycol = col;

#if defined(UNITY_REVERSED_Z)
				depth = 1.0f - col.r;
#else
				depth = col.r;
#endif
			}
#endif
			ENDCG
		}
	}
}
