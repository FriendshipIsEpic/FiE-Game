Shader "Hidden/Colorful/Fast Vignette" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Params ("Center (XY) Sharpness (Z) Darkness (W)", Vector) = (0.5,0.5,0.1,0.3)
		_Color ("Vignette Color (RGB)", Vector) = (0,0,0,0)
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		sampler2D _MainTex;
		fixed4 _Color;
		struct Input
		{
			float2 uv_MainTex;
		};
		
		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
}