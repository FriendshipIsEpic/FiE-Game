Shader "Hidden/PlawiusSSR" {
	Properties {
		_MainTex ("", any) = "" {}
		_Original ("", any) = "" {}
		_FresnelStart ("Fresnel Factor R0", Range(0, 1)) = 0
		_FaceViewerFactor ("Face Viewer Factor", Range(0, 1)) = 0.5
		_Cutoff_Start ("Cut-off start", Range(-1, 1)) = -0.2
		_Cutoff_End ("Cut-off end", Range(-1, 1)) = 0.2
		_LinearStepK ("Linear Step Coefficient", Range(1, 30)) = 30
		_Bias ("Z Bias", Range(0, 0.2)) = 0.0001
		_MaxIter ("Max Raymarch Iterations", Range(16, 256)) = 128
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