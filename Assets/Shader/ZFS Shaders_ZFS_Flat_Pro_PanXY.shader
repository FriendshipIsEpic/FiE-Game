Shader "ZFS Shaders/ZFS_Flat_Pro_PanXY" {
	Properties {
		_Brightness ("Brightness", Float) = 1
		_Intensity ("Intensity", Float) = 1
		[MaterialToggle] _Gradient_Or_Solid_Color ("Gradient_Or_Solid_Color", Float) = 1
		_Gradient_Color ("Gradient_Color", 2D) = "white" {}
		_Solid_Color ("Solid_Color", Vector) = (0.1764706,0.5229208,1,1)
		_Texture ("Texture", 2D) = "white" {}
		_Gradient_Texture_Decay ("Gradient_Texture_Decay", 2D) = "white" {}
		_Decay ("Decay", Range(0.05, 0.95)) = 0.3
		[MaterialToggle] _Mask ("Mask", Float) = 1
		[MaterialToggle] _Make_Same_As_Mask ("Make_Same_As_Mask", Float) = 0
		_Mask_Texture ("Mask_Texture", 2D) = "white" {}
		[MaterialToggle] _Edge_Detection ("Edge_Detection", Float) = 1
		_Edge_Detection_Distance ("Edge_Detection_Distance", Float) = 3
		_Gradient_Edge_Detection ("Gradient_Edge_Detection", 2D) = "white" {}
		[MaterialToggle] _Soft_Texture ("Soft_Texture", Float) = -1.398039
		_EmissionGain ("Emission Gain", Range(0, 1)) = 0.3
		_Pan_SpeedY ("Pan_SpeedY", Float) = 0.1
		_Pan_SpeedX ("Pan_SpeedX", Float) = 0
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
	//CustomEditor "ShaderForgeMaterialInspector"
}