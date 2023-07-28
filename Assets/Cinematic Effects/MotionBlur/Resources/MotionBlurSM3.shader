// Amplify Motion - Full-scene Motion Blur for Unity Pro
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

Shader "Hidden/Amplify Motion/MotionBlurSM3" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_MotionTex ("Motion (RGB)", 2D) = "white" {}
	}	
	CGINCLUDE
		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma exclude_renderers flash

		#include "MotionBlurShared.cginc"
	ENDCG
	SubShader {
		ZTest Always Cull Off ZWrite Off Fog { Mode off }
		Pass {
			Name "MOB"
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag_mobile
				#pragma target 2.0
			ENDCG
		}
		Pass {
			Name "STD"
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag_sm2
				#pragma target 2.0
			ENDCG
		}
		Pass {
			Name "STD_SM3"
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag_sm3
				#pragma target 3.0
				#pragma exclude_renderers d3d11_9x
			ENDCG
		}
		Pass {
			Name "SOFT_SM3"
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag_soft_sm3
				#pragma target 3.0
				#pragma exclude_renderers d3d11_9x
			ENDCG
		}
	}	
	Fallback Off
}
