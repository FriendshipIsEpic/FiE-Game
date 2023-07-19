Shader "Stereo 3D" {
	Properties {
		_LeftTex ("Left (RGB)", 2D) = "red" {}
		_RightTex ("Right (RGB)", 2D) = "white" {}
		_Balance_Left_R ("Balance Left R", Vector) = (0,0,0,0)
		_Balance_Left_G ("Balance Left G", Vector) = (0,0,0,0)
		_Balance_Left_B ("Balance Left B", Vector) = (0,0,0,0)
		_Balance_Right_R ("Balance Right R", Vector) = (0,0,0,0)
		_Balance_Right_G ("Balance Right G", Vector) = (0,0,0,0)
		_Balance_Right_B ("Balance Right B", Vector) = (0,0,0,0)
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
}