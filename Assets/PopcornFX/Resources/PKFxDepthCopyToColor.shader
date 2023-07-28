// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/PKFx Depth Copy to Color"
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

		// Writes to a single-component texture (TextureFormat.RHalf)
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

			fixed4 frag(v2f i) : SV_Target
			{
				float4 col = tex2D(_CameraDepthTexture, i.uv);
				return col;
			}
			ENDCG
		}
	}
}
