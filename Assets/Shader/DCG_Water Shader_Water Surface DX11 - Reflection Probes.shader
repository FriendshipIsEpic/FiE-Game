Shader "DCG/Water Shader/Water Surface DX11 - Reflection Probes" {
	Properties {
		[Header(Reflection Settings)] [Space] _ReflectionFresnel ("Reflection Fresnel", Float) = 3
		_ReflectionIntensity ("Reflection Intensity", Range(0, 1)) = 0.8611565
		_ReflectionRoughness ("Reflection Roughness", Range(0, 1)) = 0
		[Space] [Header(Water Settings)] [Space] _ShoreOpacity ("Shore Opacity", Float) = 0.5
		_WaterColor ("Water Color", Vector) = (0.2941176,1,1,1)
		_Density ("Density", Float) = 1
		_FadeLevel ("Fade Level", Float) = 4
		[Space] [Header(Foam Settings)] [Space] _FoamMap ("Foam Map", 2D) = "white" {}
		_FoamIntensity ("Foam Intensity", Float) = 1
		_WavesFoamIntensity ("Waves Foam Intensity", Float) = 0.3
		_FoamScale ("Foam Scale", Float) = 1
		_FoamSpeed ("Foam Speed", Float) = 1
		_FoamDistance ("Foam Distance", Float) = 2
		[Space] [Header(Waves Settings)] [Space] _WavesNormalMap ("Waves Normal Map", 2D) = "bump" {}
		_WavesTileScale ("Waves Tile Scale", Float) = 0.3
		_WavesSpeed ("Waves Speed", Float) = 1
		_WavesNormalIntensity ("Waves Normal Intensity", Range(0, 1)) = 1
		_DetailWavesNormalMap ("Detail Waves Normal Map", 2D) = "bump" {}
		_DetailWavesTileScale ("Detail Waves Tile Scale", Float) = 3
		_DetailWavesSpeed ("Detail Waves Speed", Float) = 1
		_DetailWavesNormalIntensity ("Detail Waves Normal Intensity", Range(0, 1)) = 0
		[Space] [Header(Light Refraction Settings)] [Space] _RefractionLevel ("Refraction Level", Range(0, 0.3)) = 0.1142179
		[Space] [Header(Light Reflection Settings)] [Space] _Gloss ("Gloss", Range(0, 1)) = 0.7
		_GlossBleedSharpness ("Gloss Bleed Sharpness", Range(0, 1)) = 0.5203999
		_GlossBleedIntensity ("Gloss Bleed Intensity", Range(0, 1)) = 0.0995139
		_SpecularIntensity ("Specular Intensity", Float) = 1
		[Space] [Header(Displacement Settings)] [Space] _DisplacementIntensity ("Displacement Intensity", Float) = 1
		_WaveHeightSineSpeed ("Wave Height Sine Speed", Float) = 2
		_WaveHeightSineIntensity ("Wave Height Sine Intensity", Float) = 0.12
		_MaxTessellation ("Max Tessellation", Float) = 8
		[HideInInspector] _Cutoff ("Alpha cutoff", Range(0, 1)) = 0.5
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
	Fallback "Legacy Shaders/VertexLit"
	//CustomEditor "ShaderForgeMaterialInspector"
}