Shader "Hidden/Ultimate/Sampling"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	 
	CGINCLUDE
	 
	#pragma target 3.0

	#include "UnityCG.cginc"

	float4 _OffsetInfos;
	float4 _Tint;
	float _Intensity;
	sampler2D _MainTex;
	sampler2D _AdditiveTexture;

	struct v2f 
	{
		half4 pos : SV_POSITION;
		half2 uv : TEXCOORD0;
	};

	struct v2f_opts 
	{
		half4 pos : SV_POSITION;
		half2 uv[7] : TEXCOORD0;
	};

	v2f vert( appdata_img v ) 
	{
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv =  v.texcoord.xy;
		return o;
	} 

	struct v2fLow {
		half4 pos : POSITION;
		half2 uv : TEXCOORD0;
		half4 uv01 : TEXCOORD1;
		half4 uv23 : TEXCOORD2;
		half4 uv45 : TEXCOORD3;
		half4 uv67 : TEXCOORD4;
		half4 uv89 : TEXCOORD5;
	};
	

	v2fLow vertLow( appdata_img v ) 
	{
		v2fLow o; 
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv =  v.texcoord.xy;

		o.uv01 =  v.texcoord.xyxy + _OffsetInfos.xyxy * half4(1,1, -1,-1);
		o.uv23 =  v.texcoord.xyxy + _OffsetInfos.xyxy * half4(1,1, -1,-1) * 2;
		o.uv45 =  v.texcoord.xyxy + _OffsetInfos.xyxy * half4(1,1, -1,-1) * 3;
		o.uv67 =  v.texcoord.xyxy + _OffsetInfos.xyxy * half4(1,1, -1,-1) * 4;
		o.uv89 =  v.texcoord.xyxy + _OffsetInfos.xyxy * half4(1,1, -1,-1) * 5;

		return o;
	} 

	inline float ComputeLuma( float3 c )
	{
		return dot( c, fixed3(0.299, 0.587, 0.114) );
	}


	float Gaussian(float Scale, int iSamplePoint)
	{
		float sigma = (Scale-1.0)/5;
		float g = 1.0f / sqrt(2.0f * 3.14159 * sigma * sigma);  
		return (g * exp(-(iSamplePoint * iSamplePoint) / (2 * sigma * sigma)));
	}

	float4 Upsample(half2 uv)
	{
		half4 f0 =  tex2D (_AdditiveTexture, uv + half2(_OffsetInfos.z,_OffsetInfos.w));
		half4 f1 =  tex2D (_AdditiveTexture, uv + half2(-_OffsetInfos.z,_OffsetInfos.w));
		half4 f2 =  tex2D (_AdditiveTexture, uv + half2(-_OffsetInfos.z,-_OffsetInfos.w));
		half4 f3 =  tex2D (_AdditiveTexture, uv + half2(_OffsetInfos.z,-_OffsetInfos.w));

		return (f0+f1+f2+f3)*0.25;
	}


	half4 fragGaussBlurVeryHigh (v2f i) : SV_Target 
	{
		half4 color = half4 (0,0,0,0);

		float Scale = 31;

		float2 gUV = i.uv;
		float Offset = 0;
		 
		color += 0.1480461 * tex2D (_MainTex, gUV);
		color += 0.1451146 * tex2D (_MainTex, gUV + _OffsetInfos.xy * 1);
		color += 0.1451146 * tex2D (_MainTex, gUV - _OffsetInfos.xy * 1);
		color += 0.1366637 * tex2D (_MainTex, gUV + _OffsetInfos.xy * 2);
		color += 0.1366637 * tex2D (_MainTex, gUV - _OffsetInfos.xy * 2);
		color += 0.1236585 * tex2D (_MainTex, gUV + _OffsetInfos.xy * 3);
		color += 0.1236585 * tex2D (_MainTex, gUV - _OffsetInfos.xy * 3);
		color += 0.1075035 * tex2D (_MainTex, gUV + _OffsetInfos.xy * 4);
		color += 0.1075035 * tex2D (_MainTex, gUV - _OffsetInfos.xy * 4);
		color += 0.08979447 * tex2D (_MainTex, gUV + _OffsetInfos.xy * 5);
		color += 0.08979447 * tex2D (_MainTex, gUV - _OffsetInfos.xy * 5);
		color += 0.07206175 * tex2D (_MainTex, gUV + _OffsetInfos.xy * 6);
		color += 0.07206175 * tex2D (_MainTex, gUV - _OffsetInfos.xy * 6);
		color += 0.05556333 * tex2D (_MainTex, gUV + _OffsetInfos.xy * 7);
		color += 0.05556333 * tex2D (_MainTex, gUV - _OffsetInfos.xy * 7);
		color += 0.04116233 * tex2D (_MainTex, gUV + _OffsetInfos.xy * 8);
		color += 0.04116233 * tex2D (_MainTex, gUV - _OffsetInfos.xy * 8);
		color += 0.02929812 * tex2D (_MainTex, gUV + _OffsetInfos.xy * 9);
		color += 0.02929812 * tex2D (_MainTex, gUV - _OffsetInfos.xy * 9);
		color += 0.02003586 * tex2D (_MainTex, gUV + _OffsetInfos.xy * 10);
		color += 0.02003586 * tex2D (_MainTex, gUV - _OffsetInfos.xy * 10);
		color += 0.01316449 * tex2D (_MainTex, gUV + _OffsetInfos.xy * 11);
		color += 0.01316449 * tex2D (_MainTex, gUV - _OffsetInfos.xy * 11);
		color += 0.008310529 * tex2D (_MainTex, gUV + _OffsetInfos.xy * 12);
		color += 0.008310529 * tex2D (_MainTex, gUV - _OffsetInfos.xy * 12);
		color += 0.005040591 * tex2D (_MainTex, gUV + _OffsetInfos.xy * 13);
		color += 0.005040591 * tex2D (_MainTex, gUV - _OffsetInfos.xy * 13);
		color += 0.002937396 * tex2D (_MainTex, gUV + _OffsetInfos.xy * 14);
		color += 0.002937396 * tex2D (_MainTex, gUV - _OffsetInfos.xy * 14);
		color += 0.001644643 * tex2D (_MainTex, gUV + _OffsetInfos.xy * 15);
		color += 0.001644643 * tex2D (_MainTex, gUV - _OffsetInfos.xy * 15);
		

		color.a = 1.0;
		return color * _Tint * _Intensity + Upsample(i.uv);
	}

	half4 fragGaussBlurHigher (v2f i) : SV_Target 
	{
		half4 color = half4 (0,0,0,0);

		float Scale = 31;

		float2 gUV = i.uv;
		float Offset = 0;
		 
		color += 0.1562562 * tex2D (_MainTex, gUV);
		color += 0.1527989 * tex2D (_MainTex, gUV + _OffsetInfos.xy * 1);
		color += 0.1527989 * tex2D (_MainTex, gUV - _OffsetInfos.xy * 1);
		color += 0.1428793 * tex2D (_MainTex, gUV + _OffsetInfos.xy * 2);
		color += 0.1428793 * tex2D (_MainTex, gUV - _OffsetInfos.xy * 2);
		color += 0.1277568 * tex2D (_MainTex, gUV + _OffsetInfos.xy * 3);
		color += 0.1277568 * tex2D (_MainTex, gUV - _OffsetInfos.xy * 3);
		color += 0.1092358 * tex2D (_MainTex, gUV + _OffsetInfos.xy * 4);
		color += 0.1092358 * tex2D (_MainTex, gUV - _OffsetInfos.xy * 4);
		color += 0.08931243 * tex2D (_MainTex, gUV + _OffsetInfos.xy * 5);
		color += 0.08931243 * tex2D (_MainTex, gUV - _OffsetInfos.xy * 5);
		color += 0.06982721 * tex2D (_MainTex, gUV + _OffsetInfos.xy * 6);
		color += 0.06982721 * tex2D (_MainTex, gUV - _OffsetInfos.xy * 6);
		color += 0.05220396 * tex2D (_MainTex, gUV + _OffsetInfos.xy * 7);
		color += 0.05220396 * tex2D (_MainTex, gUV - _OffsetInfos.xy * 7);
		color += 0.03732055 * tex2D (_MainTex, gUV + _OffsetInfos.xy * 8);
		color += 0.03732055 * tex2D (_MainTex, gUV - _OffsetInfos.xy * 8);
		color += 0.02551284 * tex2D (_MainTex, gUV + _OffsetInfos.xy * 9);
		color += 0.02551284 * tex2D (_MainTex, gUV - _OffsetInfos.xy * 9);
		color += 0.01667767 * tex2D (_MainTex, gUV + _OffsetInfos.xy * 10);
		color += 0.01667767 * tex2D (_MainTex, gUV - _OffsetInfos.xy * 10);
		color += 0.01042505 * tex2D (_MainTex, gUV + _OffsetInfos.xy * 11);
		color += 0.01042505 * tex2D (_MainTex, gUV - _OffsetInfos.xy * 11);
		color += 0.006231415 * tex2D (_MainTex, gUV + _OffsetInfos.xy * 12);
		color += 0.006231415 * tex2D (_MainTex, gUV - _OffsetInfos.xy * 12);
		color += 0.003561732 * tex2D (_MainTex, gUV + _OffsetInfos.xy * 13);
		color += 0.003561732 * tex2D (_MainTex, gUV - _OffsetInfos.xy * 13);
		

		color.a = 1.0;
		return color * _Tint * _Intensity + Upsample(i.uv);
	}

	half4 fragGaussBlurHigh (v2f i) : SV_Target 
	{
		half4 color = half4 (0,0,0,0);

		float Scale = 31;

		float2 gUV = i.uv;
		float Offset = 0;
		 
		color += 0.1820341 * tex2D (_MainTex, gUV);
		color += 0.1764335 * tex2D (_MainTex, gUV + _OffsetInfos.xy * 1);
		color += 0.1764335 * tex2D (_MainTex, gUV - _OffsetInfos.xy * 1);
		color += 0.1606445 * tex2D (_MainTex, gUV + _OffsetInfos.xy * 2);
		color += 0.1606445 * tex2D (_MainTex, gUV - _OffsetInfos.xy * 2);
		color += 0.1374065 * tex2D (_MainTex, gUV + _OffsetInfos.xy * 3);
		color += 0.1374065 * tex2D (_MainTex, gUV - _OffsetInfos.xy * 3);
		color += 0.1104092 * tex2D (_MainTex, gUV + _OffsetInfos.xy * 4);
		color += 0.1104092 * tex2D (_MainTex, gUV - _OffsetInfos.xy * 4);
		color += 0.08334126 * tex2D (_MainTex, gUV + _OffsetInfos.xy * 5);
		color += 0.08334126 * tex2D (_MainTex, gUV - _OffsetInfos.xy * 5);
		color += 0.05909781 * tex2D (_MainTex, gUV + _OffsetInfos.xy * 6);
		color += 0.05909781 * tex2D (_MainTex, gUV - _OffsetInfos.xy * 6);
		color += 0.03936763 * tex2D (_MainTex, gUV + _OffsetInfos.xy * 7);
		color += 0.03936763 * tex2D (_MainTex, gUV - _OffsetInfos.xy * 7);
		color += 0.02463563 * tex2D (_MainTex, gUV + _OffsetInfos.xy * 8);
		color += 0.02463563 * tex2D (_MainTex, gUV - _OffsetInfos.xy * 8);
		color += 0.01448254 * tex2D (_MainTex, gUV + _OffsetInfos.xy * 9);
		color += 0.01448254 * tex2D (_MainTex, gUV - _OffsetInfos.xy * 9);
		color += 0.007998019 * tex2D (_MainTex, gUV + _OffsetInfos.xy * 10);
		color += 0.007998019 * tex2D (_MainTex, gUV - _OffsetInfos.xy * 10);
		color += 0.004149318 * tex2D (_MainTex, gUV + _OffsetInfos.xy * 11);
		color += 0.004149318 * tex2D (_MainTex, gUV - _OffsetInfos.xy * 11);
		

		color.a = 1.0;
		return color * _Tint * _Intensity + Upsample(i.uv);
	}

	half4 fragGaussBlurMedium (v2f i) : SV_Target 
	{
		half4 color = half4 (0,0,0,0);

		float Scale = 17;

		float2 gUV = i.uv;
		float Offset = 0;

		color += 0.2605744 * tex2D (_MainTex, gUV);
		color += 0.242882 * tex2D (_MainTex, gUV + _OffsetInfos.xy * 1);
		color += 0.242882 * tex2D (_MainTex, gUV - _OffsetInfos.xy * 1);
		color += 0.1966919 * tex2D (_MainTex, gUV + _OffsetInfos.xy * 2);
		color += 0.1966919 * tex2D (_MainTex, gUV - _OffsetInfos.xy * 2);
		color += 0.13839 * tex2D (_MainTex, gUV + _OffsetInfos.xy * 3);
		color += 0.13839 * tex2D (_MainTex, gUV - _OffsetInfos.xy * 3);
		color += 0.08459612 * tex2D (_MainTex, gUV + _OffsetInfos.xy * 4);
		color += 0.08459612 * tex2D (_MainTex, gUV - _OffsetInfos.xy * 4);
		color += 0.04492867 * tex2D (_MainTex, gUV + _OffsetInfos.xy * 5);
		color += 0.04492867 * tex2D (_MainTex, gUV - _OffsetInfos.xy * 5);
		color += 0.02073118 * tex2D (_MainTex, gUV + _OffsetInfos.xy * 6);
		color += 0.02073118 * tex2D (_MainTex, gUV - _OffsetInfos.xy * 6);
		color += 0.008310967 * tex2D (_MainTex, gUV + _OffsetInfos.xy * 7);
		color += 0.008310967 * tex2D (_MainTex, gUV - _OffsetInfos.xy * 7);
		color += 0.002894721 * tex2D (_MainTex, gUV + _OffsetInfos.xy * 8);
		color += 0.002894721 * tex2D (_MainTex, gUV - _OffsetInfos.xy * 8);


		return color * _Tint * _Intensity + Upsample(i.uv);
	}

	half4 fragGaussBlurLow (v2f i) : SV_Target 
	{
		half4 color = half4 (0,0,0,0);

		float Scale = 17;

		float2 gUV = i.uv;
		float Offset = 0;

		color += 0.3098615 * tex2D (_MainTex, gUV);
		color += 0.2789662 * tex2D (_MainTex, gUV + _OffsetInfos.xy * 1);
		color += 0.2789662 * tex2D (_MainTex, gUV - _OffsetInfos.xy * 1);
		color += 0.2035652 * tex2D (_MainTex, gUV + _OffsetInfos.xy * 2);
		color += 0.2035652 * tex2D (_MainTex, gUV - _OffsetInfos.xy * 2);
		color += 0.1203992 * tex2D (_MainTex, gUV + _OffsetInfos.xy * 3);
		color += 0.1203992 * tex2D (_MainTex, gUV - _OffsetInfos.xy * 3);
		color += 0.05771804 * tex2D (_MainTex, gUV + _OffsetInfos.xy * 4);
		color += 0.05771804 * tex2D (_MainTex, gUV - _OffsetInfos.xy * 4);
		color += 0.02242682 * tex2D (_MainTex, gUV + _OffsetInfos.xy * 5);
		color += 0.02242682 * tex2D (_MainTex, gUV - _OffsetInfos.xy * 5);
		color += 0.00706304 * tex2D (_MainTex, gUV + _OffsetInfos.xy * 6);
		color += 0.00706304 * tex2D (_MainTex, gUV - _OffsetInfos.xy * 6);


		return color * _Tint * _Intensity + Upsample(i.uv);
	}

	half4 fragGaussBlurVeryLow (v2fLow i) : SV_Target 
	{
		half4 color = half4 (0,0,0,0);

		float2 gUV = i.uv;
		float Offset = 0;

		color += 0.4310208 * tex2D (_MainTex, i.uv);

		color += 0.3403002 * tex2D (_MainTex,  i.uv01.xy);
		color += 0.3403002 * tex2D (_MainTex, i.uv01.zw);

		color += 0.1674766 * tex2D (_MainTex,  i.uv23.xy);
		color += 0.1674766 * tex2D (_MainTex, i.uv23.zw);

		color += 0.05137766 * tex2D (_MainTex,  i.uv45.xy);
		color += 0.05137766 * tex2D (_MainTex, i.uv45.zw);

		color += 0.009824769 * tex2D (_MainTex,  i.uv67.xy);
		color += 0.009824769 * tex2D (_MainTex, i.uv67.zw);

		return color * _Tint * _Intensity + Upsample(i.uv);
	}


	ENDCG

	SubShader
	{
		Pass // #0 Simple Downscaling
		{
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
			
			CGPROGRAM

			#pragma vertex vert_img
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest 
			#include "UnityCG.cginc"


			fixed4 frag(v2f i):COLOR
			{
				float2 UV[4];

				UV[0] = i.uv + float2(-1.0 * _OffsetInfos.x, -1.0 * _OffsetInfos.y);
				UV[1] = i.uv + float2( 1.0 * _OffsetInfos.x, -1.0 * _OffsetInfos.y);
				UV[2] = i.uv + float2(-1.0 * _OffsetInfos.x,  1.0 * _OffsetInfos.y);
				UV[3] = i.uv + float2( 1.0 * _OffsetInfos.x,  1.0 * _OffsetInfos.y);

				
				fixed4 Sample[4];

				for(int j = 0; j < 4; ++j)
				{
					Sample[j] = tex2D(_MainTex, UV[j]);
				}

				return (Sample[0] + Sample[1] + Sample[2] + Sample[3]) * 1.0/4;
			}

			ENDCG
		}

		Pass // #1 Complex Downscaling
		{
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
			
			CGPROGRAM

			#pragma vertex vert_img
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest 
			#include "UnityCG.cginc"

			fixed4 frag(v2f i):COLOR
			{
			 
				float2 UV[9];

				UV[0] = i.uv;

				UV[1] = i.uv + float2( -2.0 * _OffsetInfos.x, -2.0 * _OffsetInfos.y);
				UV[2] = i.uv + float2( 0.0 * _OffsetInfos.x,  -2.0 * _OffsetInfos.y);
				UV[3] = i.uv + float2( 2.0 * _OffsetInfos.x,  -2.0 * _OffsetInfos.y);
				UV[4] = i.uv + float2( -2.0 * _OffsetInfos.x, 2.0 * _OffsetInfos.y);
				UV[5] = i.uv + float2( 0.0 * _OffsetInfos.x,  2.0 * _OffsetInfos.y);
				UV[6] = i.uv + float2( 2.0 * _OffsetInfos.x,  2.0 * _OffsetInfos.y);
				UV[7] = i.uv + float2( -2.0 * _OffsetInfos.x,  0.0 * _OffsetInfos.y);
				UV[8] = i.uv + float2( 2.0 * _OffsetInfos.x,  0.0 * _OffsetInfos.y);


				
				fixed4 Sample[9];

				for(int j = 0; j < 9; ++j)
				{
					Sample[j] = tex2D(_MainTex, UV[j]);
				}

				half4 sum = half4(0,0,0,0);
				for(int j = 0; j < 9; ++j)
				{
					sum += Sample[j];
				}

				return sum* 1.0/9;
			}

			ENDCG
		}

		Pass // #2 Gaussian Sampling Very High
		{
			CGPROGRAM
      
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma exclude_renderers flash
			#pragma vertex vert_img
			#pragma fragment fragGaussBlurVeryHigh
      
			ENDCG
		}

		Pass // #3 Gaussian Sampling Medium
		{
			CGPROGRAM
      
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma exclude_renderers flash
			#pragma vertex vert_img
			#pragma fragment fragGaussBlurMedium
      
			ENDCG
		}

		Pass // #4 Gaussian Sampling Very Low
		{
			CGPROGRAM
      
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma exclude_renderers flash
			#pragma vertex vertLow
			#pragma fragment fragGaussBlurVeryLow
      
			ENDCG
		}


		Pass // #5 Filmic curve sampling
		{
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
			
			CGPROGRAM

			#pragma vertex vert_img
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest 
			#include "UnityCG.cginc"

			// Curve mapping
			half4 _Toe;
			half4 _Shoulder;
			half _K;
			half _Crossover;
			half _MaxValue;
			half _CurveExposure;
			float Map(half x)
			{
				float4 data;
				float endAdd;

				if (x > _Crossover)
				{
					data = _Shoulder;
					endAdd = _K;
				}
				else
				{
					data = _Toe;
					endAdd = 0;
				}


				float2 numDenum = data.xy * x + data.zw;
				return numDenum.x / numDenum.y + endAdd;
			}
			
			
			fixed4 frag(v2f i):COLOR
			{
				float2 UV[4];

				UV[0] = i.uv + float2(-1.0 * _OffsetInfos.x, -1.0 * _OffsetInfos.y);
				UV[1] = i.uv + float2( 1.0 * _OffsetInfos.x, -1.0 * _OffsetInfos.y);
				UV[2] = i.uv + float2(-1.0 * _OffsetInfos.x,  1.0 * _OffsetInfos.y);
				UV[3] = i.uv + float2( 1.0 * _OffsetInfos.x,  1.0 * _OffsetInfos.y);

				fixed4 Sample[4];

				for(int j = 0; j < 4; ++j)
				{
					Sample[j] = tex2D(_MainTex, UV[j]);
				}

				half4 color =  (Sample[0] + Sample[1] + Sample[2] + Sample[3]) * 1.0/4;

				half intensity = ComputeLuma(color);

				

				half bloomIntensity = intensity/max(1.0+intensity*_CurveExposure,0.01);
				
				bloomIntensity = Map(bloomIntensity) * _MaxValue;

				return clamp(color * bloomIntensity/intensity,0,65000);
			}

			ENDCG
		}

		Pass // #6 Low Gaussian Filmic Curve
		{
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
			
			CGPROGRAM

			#pragma vertex vert_img
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest 
			#include "UnityCG.cginc"

			// Curve mapping
			half4 _Toe;
			half4 _Shoulder;
			half _K;
			half _Crossover;
			float Map(half x)
			{
				float4 data;
				float endAdd;

				if (x > _Crossover)
				{
					data = _Shoulder;
					endAdd = _K;
				}
				else
				{
					data = _Toe;
					endAdd = 0;
				}


				float2 numDenum = data.xy * x + data.zw;
				return numDenum.x / numDenum.y + endAdd;
			}

			fixed4 frag(v2f i):COLOR
			{
				half4 color = half4 (0,0,0,0);

				float Scale = 9;

				float2 gUV = i.uv;
				float Offset = 0;

				color += 0.4005 * tex2D (_MainTex, gUV);
				color += 0.3294 * tex2D (_MainTex, gUV + _OffsetInfos.xy * 1.0);
				color += 0.3294 * tex2D (_MainTex, gUV - _OffsetInfos.xy * 1.0);
				color += 0.1833 * tex2D (_MainTex, gUV + _OffsetInfos.xy * 2.0);
				color += 0.1833 * tex2D (_MainTex, gUV - _OffsetInfos.xy * 2.0);
				color += 0.0691 * tex2D (_MainTex, gUV + _OffsetInfos.xy * 3.0);
				color += 0.0691 * tex2D (_MainTex, gUV - _OffsetInfos.xy * 3.0);
				color += 0.0175 * tex2D (_MainTex, gUV + _OffsetInfos.xy * 4.0);
				color += 0.0175 * tex2D (_MainTex, gUV - _OffsetInfos.xy * 4.0);

				
				

				half intensity = dot(color, half3(0.3,0.3,0.3));
				half bloomIntensity = Map(intensity);

				return color * bloomIntensity/intensity;
			 }

			ENDCG
		}

		Pass // #7 Simple Blur
		{
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
			
			CGPROGRAM

			#pragma vertex vert_img
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest 
			#include "UnityCG.cginc"


			fixed4 frag(v2f i):COLOR
			{
			 
			 
				float2 UV[3];

				UV[0] = i.uv;
				UV[1] = i.uv + 1.5*_OffsetInfos;
				UV[2] = i.uv - 1.5*_OffsetInfos;
				//UV[2] = i.uv + 2*_OffsetInfos;
				//UV[3] = i.uv - 2*_OffsetInfos;

				
				fixed4 Sample[3];

				for(int j = 0; j < 3; ++j)
				{
					Sample[j] = tex2D(_MainTex, UV[j]);
				}

				return (Sample[0] + Sample[1]  + Sample[2]) * 1.0/3;
			}

			ENDCG
		}

		Pass // #8 Gaussian Sampling Small
		{
			CGPROGRAM
      
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma exclude_renderers flash
			#pragma vertex vert_img
			#pragma fragment fragGaussBlurLow
      
			ENDCG
		}

		Pass // #9 Gaussian Sampling High
		{
			CGPROGRAM
      
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma exclude_renderers flash
			#pragma vertex vert_img
			#pragma fragment fragGaussBlurHigh
      
			ENDCG
		}

		Pass // #10 Gaussian Sampling Higher
		{
			CGPROGRAM
      
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma exclude_renderers flash
			#pragma vertex vert_img
			#pragma fragment fragGaussBlurHigher
      
			ENDCG
		}

		Pass // #11 Temporal Stable Downsampling
		{
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
			
			CGPROGRAM

			#pragma vertex vert_img
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest 
			#include "UnityCG.cginc"
			float2 _MainTex_TexelSize;

			fixed4 frag(v2f i):COLOR
			{
				float4 offsets = _MainTex_TexelSize.xyxy * float4(-1.0, -1.0, +1.0, +1.0);
				half3 c0 = tex2D(_MainTex, i.uv + offsets.xy);
				half3 c1 = tex2D(_MainTex, i.uv + offsets.zy);
				half3 c2 = tex2D(_MainTex, i.uv + offsets.xw);
				half3 c3 = tex2D(_MainTex, i.uv + offsets.zw);
				half w0 = 1.0 / (ComputeLuma(c0) + 1.0);
				half w1 = 1.0 / (ComputeLuma(c1) + 1.0);
				half w2 = 1.0 / (ComputeLuma(c2) + 1.0);
				half w3 = 1.0 / (ComputeLuma(c3) + 1.0);
				half div = 1.0 / max(w0 + w1 + w2 + w3, 0.01);
				float3 color = (c0 * w0 + c1 * w1 + c2 * w2 + c3 * w3) * div;

				return float4(clamp(color,0,65000), 1);

				//half intensity = dot(color, half3(0.3,0.3,0.3));
			
				//half bloomIntensity = intensity/max(1.0+intensity*_CurveExposure,0.01);

				//return float4(clamp(color * bloomIntensity/intensity,0,65000), 1);
			}

			ENDCG
		}


		Pass // #12 Temporal Stable Downsampling with filmic curve
		{
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
			
			CGPROGRAM

			#pragma vertex vert_img
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest 
			#include "UnityCG.cginc"
			float2 _MainTex_TexelSize;

			// Curve mapping
			half4 _Toe;
			half4 _Shoulder;
			half _K;
			half _Crossover;
			half _MaxValue;
			half _CurveExposure;
			float Map(half x)
			{
				float4 data;
				float endAdd;

				if (x > _Crossover)
				{
					data = _Shoulder;
					endAdd = _K;
				}
				else
				{
					data = _Toe;
					endAdd = 0;
				}


				float2 numDenum = data.xy * x + data.zw;
				return numDenum.x / numDenum.y + endAdd;
			}

			fixed4 frag(v2f i):COLOR
			{
				float4 offsets = _MainTex_TexelSize.xyxy * float4(-1.0, -1.0, +1.0, +1.0);
				half3 c0 = tex2D(_MainTex, i.uv + offsets.xy);
				half3 c1 = tex2D(_MainTex, i.uv + offsets.zy);
				half3 c2 = tex2D(_MainTex, i.uv + offsets.xw);
				half3 c3 = tex2D(_MainTex, i.uv + offsets.zw);
				half w0 = 1.0 / (ComputeLuma(c0) + 1.0);
				half w1 = 1.0 / (ComputeLuma(c1) + 1.0);
				half w2 = 1.0 / (ComputeLuma(c2) + 1.0);
				half w3 = 1.0 / (ComputeLuma(c3) + 1.0);
				half div = 1.0 / max(w0 + w1 + w2 + w3, 0.01);
				float3 color = (c0 * w0 + c1 * w1 + c2 * w2 + c3 * w3) * div;

				half intensity = ComputeLuma(color);
			
				half bloomIntensity = intensity/max(1.0+intensity*_CurveExposure,0.01);
				bloomIntensity = Map(bloomIntensity) * _MaxValue;

				return float4(clamp(color * bloomIntensity/intensity,0,65000), 1);
			}

			ENDCG
		}

	}

	FallBack off
}
