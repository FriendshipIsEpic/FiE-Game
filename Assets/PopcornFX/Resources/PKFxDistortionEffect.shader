// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/PKFx Distortion"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_DistortionTex ("Distortion (RGB)", 2D) = "black" {}
	}

	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always
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
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			uniform sampler2D _MainTex;
			uniform sampler2D _DistortionTex;

			float4 SampleColorAt(float2 uvOffset)
			{
				return tex2D(_MainTex, uvOffset);
			}

			float4 frag (v2f i) : SV_Target
			{
				float4 _DistBlurFactor = float4(15.0f, 15.0f, 0.75f, 1.0f);
				float4 baseOffset = (tex2D(_DistortionTex, i.uv));
				baseOffset *= _DistBlurFactor;
			
				float4 color1 = SampleColorAt(baseOffset.xy * 0.01 + i.uv);
				float4 color2 = SampleColorAt(baseOffset.xy * 0.0125 + i.uv);
				float4 color3 = SampleColorAt(baseOffset.xy * 0.015 + i.uv);
				float4 color4 = SampleColorAt(baseOffset.xy * 0.0175 + i.uv);

				return float4(lerp(color1.x, color2.x, 0.5f), lerp(color2.y, color3.y, 0.5f), color4.z, 1);
			}
ENDCG
		}
	}

Fallback off

}
