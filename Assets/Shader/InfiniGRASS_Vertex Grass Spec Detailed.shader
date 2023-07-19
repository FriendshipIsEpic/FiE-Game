Shader "InfiniGRASS/Vertex Grass Spec Detailed" {
	Properties {
		_Diffuse ("Diffuse", 2D) = "white" {}
		_Normal ("Normal", 2D) = "bump" {}
		_Cutoff ("Alpha cutoff", Range(0, 1)) = 0.5
		_BulgeScale ("Bulge Scale", Float) = 0.2
		_BulgeShape ("Bulge Shape", Float) = 5
		_BulgeScale_copy ("Bulge Scale_copy", Float) = 1.2
		_WaveControl1 ("Waves", Vector) = (1,0.01,0.001,0)
		_TimeControl1 ("Time", Vector) = (1,10,0.02,100)
		_OceanCenter ("Ocean Center", Vector) = (0,0,0,0)
		_RandYScale ("Vary Height Ammount", Float) = 1
		_RippleScale ("Vary Height", Float) = 0
		_InteractPos ("Interact Position", Vector) = (0,0,0,1)
		_FadeThreshold ("Fade out Threshold", Float) = 100
		_StopMotionThreshold ("Stop motion Threshold", Float) = 10
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			o.Albedo = 1;
		}
		ENDCG
	}
	Fallback "Diffuse"
}