Shader "Hidden/Colorful/Technicolor" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Exposure ("Exposure (Float)", Range(0, 8)) = 4
		_Balance ("Channel Balance (RGB)", Vector) = (0.75,0.75,0.75,1)
		_Amount ("Amount (Float)", Range(0, 1)) = 0.5
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		sampler2D _MainTex;
		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
}