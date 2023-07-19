Shader "UBER - Metallic Setup/ Triplanar Selective" {
	Properties {
		[HDR] _Color ("Color", Vector) = (1,1,1,1)
		[HDR] _Color2 ("Color2", Vector) = (1,1,1,1)
		_MainTex ("Albedo", 2D) = "white" {}
		_MainTex2 ("Albedo2", 2D) = "white" {}
		[Enum(Hide Layers A and B,0, Show Layer A,1, Show Layer B,2, Show all decals,3)] _DecalMaskGUI ("Decal mask (GUI)", Float) = 3
		_DecalMask ("Decal Mask", Float) = 1
		[NoKeywordToggle] _Pierceable ("Pierceable", Float) = 0
		_Cutoff ("Alpha Cutoff", Range(0, 1)) = 0.5
		_CutoffEdgeGlow ("Edge glow", Vector) = (0,0,0,0.02)
		_Glossiness ("Smoothness", Range(0, 1)) = 1
		_Glossiness2 ("Smoothness2", Range(0, 1)) = 1
		[Gamma] _Metallic ("Metallic", Range(0, 1)) = 1
		[Gamma] _Metallic2 ("Metallic2", Range(0, 1)) = 1
		_MetallicGlossMap ("Metallic", 2D) = "white" {}
		_MetallicGlossMap2 ("Metallic2", 2D) = "white" {}
		_BumpScale ("Scale", Float) = 1
		_BumpScale2 ("Scale2", Float) = 1
		_BumpMap ("Normal Map", 2D) = "bump" {}
		_BumpMap2 ("Normal Map2", 2D) = "bump" {}
		_Parallax ("Height Scale", Range(0, 0.2)) = 0.04
		_Parallax2 ("Height Scale2", Range(0, 0.2)) = 0.04
		_ParallaxMap ("Height Map", 2D) = "black" {}
		_ParallaxMap2 ("Height Map2", 2D) = "black" {}
		_OcclusionStrength ("Strength", Range(0, 1)) = 0
		_OcclusionStrength2 ("Strength2", Range(0, 1)) = 0
		_SecOcclusionStrength ("Strength (secondary)", Range(0, 1)) = 0
		_OcclusionMap ("Occlusion", 2D) = "white" {}
		_EmissionColor ("Color", Vector) = (0,0,0,1)
		_EmissionMap ("Emission", 2D) = "white" {}
		_DetailMask ("Detail Mask", 2D) = "white" {}
		_DetailAlbedoMap ("Detail Albedo x2", 2D) = "grey" {}
		_DetailNormalMapScale ("Scale", Float) = 1
		_DetailNormalMap ("Normal Map", 2D) = "bump" {}
		[Enum(UV0,0,UV1,1)] _UVSec ("UV Set for secondary textures", Float) = 0
		[Enum(UV0,0,UV1,1)] _UVSecOcclusion ("UV Set for occlusion", Float) = 0
		[NoKeywordToggle] _UVSecOcclusionLightmapPacked ("2ndary occlusion from UV1 - apply lightmap transform", Float) = 0
		[HideInInspector] _Mode ("__mode", Float) = 0
		[HideInInspector] _SrcBlend ("__src", Float) = 1
		[HideInInspector] _DstBlend ("__dst", Float) = 0
		[HideInInspector] _ZWrite ("__zw", Float) = 1
		_DiffuseScatter ("Diffuse Scatter", Float) = 1
		_DiffuseScatteringColor ("Diffuse scattering", Vector) = (0,0,0,0)
		_DiffuseScatteringColor2 ("Diffuse scattering2", Vector) = (0,0,0,0)
		_DiffuseScatteringExponent ("Diffuse scattering exponent", Range(2, 20)) = 8
		_DiffuseScatteringOffset ("Diffuse scattering offset", Range(-0.5, 0.5)) = 0
		_GlossMin ("Gloss Min", Range(0, 1)) = 0
		_GlossMax ("Gloss Max", Range(0, 1)) = 1
		_BendNormalsFreq ("Bend normals frequency", Float) = 4
		_BendNormalsStrength ("Bend normals strangth", Range(0, 0.2)) = 0.05
		_DetailUVMult ("Detail mask tiling mult", Float) = 1
		_DetailNormalLerp ("Detail normalmap override", Range(0, 1)) = 0
		_DetailColor ("Tint (RGB+A - opacity)", Vector) = (1,1,1,1)
		_DetailEmissiveness ("Detail emissiveness", Range(0, 1)) = 0
		_SpecularRGBGlossADetail ("Specular(RGB) Gloss(A)", 2D) = "white" {}
		_DetailSpecLerp ("Spec/Gloss override", Range(0, 1)) = 1
		_DetailSpecGloss ("Spec/gloss Tint (RGB,A)", Vector) = (1,1,1,1)
		_MetallicGlossMapDetail ("Metallic(R) Gloss(A)", 2D) = "white" {}
		_DetailMetalness ("Detail metallic", Range(0, 1)) = 1
		_DetailGloss ("Detail smoothness", Range(0, 1)) = 1
		[NoKeywordToggle] _PanEmissionMask ("Pan Emission Mask", Float) = 0
		_PanUSpeed ("   Pan U Speed", Float) = 0
		_PanVSpeed ("   Pan V Speed", Float) = 0
		[NoKeywordToggle] _PulsateEmission ("Pulsate Emission", Float) = 0
		_EmissionPulsateSpeed ("   Emission Pulsate Frequency", Float) = 0
		_MinPulseBrightness ("   Min Pulse Brightness", Range(0, 1)) = 0
		[NoKeywordToggle] _Translucency ("Translucency", Float) = 0
		[Foldout] _TranslucencyShown ("", Float) = 1
		_TranslucencyColor ("Translucency color", Vector) = (1,1,1,1)
		_TranslucencyColor2 ("Translucency color2", Vector) = (1,1,1,1)
		_TranslucencyStrength ("Translucency strength", Float) = 4
		_TranslucencyPointLightDirectionality ("Point lights directionality", Range(0, 1)) = 0.7
		_TranslucencyConstant ("Translucency constant", Range(0, 0.5)) = 0.1
		_TranslucencyNormalOffset ("Translucency normal offset", Range(0, 0.3)) = 0.05
		_TranslucencyExponent ("Translucency exponent", Range(2, 100)) = 30
		_TranslucencyOcclusion ("Translucency occlusion", Range(0, 1)) = 0
		_TranslucencySuppressRealtimeShadows ("Translucency shadows suppression", Range(0, 20)) = 0.5
		_TranslucencyNDotL ("Translucency NdotL", Range(0, 1)) = 0
		[Enum(Translucency Setup 1, 0, Translucency Setup 2, 1, Translucency Setup 3, 2, Translucency Setup 4, 3)] _TranslucencyDeferredLightIndex ("Translucency deferred light index", Float) = 0
		[NoKeywordToggle] _POM ("POM", Float) = 0
		[Foldout] _POMShown ("", Float) = 1
		[NoKeywordToggle] _DepthWrite ("Zwrite", Float) = 0
		_Depth ("Depth", Range(0.001, 2)) = 0.1
		_DistSteps ("Max relief steps", Float) = 64
		_ReliefMIPbias ("Relief MIP offset", Range(0, 2)) = 0
		_ObjectNormalsTex ("Object normals tex", 2D) = "grey" {}
		_ObjectTangentsTex ("Object tangents tex", 2D) = "grey" {}
		_CurvatureMultOffset ("Curvature", Vector) = (1,1,0,0)
		_Tan2ObjectMultOffset ("Tex2Object", Vector) = (1,1,0,0)
		_UV_Clip_Borders ("UV clip borders", Vector) = (0,0,1,1)
		_POM_BottomCut ("Bottom cut value", Range(0, 1)) = 0
		[NoKeywordToggle] _POM_MeshIsVolume ("Depth per vertex", Float) = 0
		[NoKeywordToggle] _POM_ExtrudeVolume ("Extruded mesh", Float) = 0
		[NoKeywordToggle] _POMPrecomputedFlag ("POM precomputed", Float) = 0
		[Enum(Basic,0,Mapped,1)] _POMCurvatureType ("Type of curvature mapping", Float) = 0
		_DepthReductionDistance ("Depth reduction distance", Range(1, 100)) = 20
		_CurvatureCustomU ("Curvature custom U", Float) = 0
		_CurvatureCustomV ("Curvature custom V", Float) = 0
		_CurvatureMultU ("Curvature mult U", Float) = 1
		_CurvatureMultV ("Curvature mult V", Float) = 1
		_Tan2ObjCustomU ("Tex U to Object space", Float) = 1
		_Tan2ObjCustomV ("Tex V to Object space", Float) = 1
		_Tan2ObjMultU ("Tex U to Object space mult", Float) = 1
		_Tan2ObjMultV ("Tex V to Object space mult", Float) = 1
		[NoKeywordToggle] _UV_Clip ("UV Clip", Float) = 0
		[NoKeywordToggle] _POMShadows ("Self-shadowing", Float) = 0
		_ShadowStrength ("   Self-shadow strength", Range(0, 1)) = 1
		_DistStepsShadows ("   Max shadow steps", Float) = 64
		_ShadowMIPbias ("   Shadow MIP offset", Range(0, 2)) = 0
		_Softness ("   Softness", Range(6, 0)) = 2
		_SoftnessFade ("   Softness fade", Range(0.2, 1.5)) = 0.4
		_Refraction ("Refration", Range(0, 0.5)) = 0.02
		_RefractionBumpScale ("Refraction bump scale", Range(0, 2)) = 0.5
		_RefractionChromaticAberration ("Chromatic Aberration", Range(0, 0.1)) = 0
		[NoKeywordToggle] _Wetness ("Wetness", Float) = 0
		[Foldout] _WetnessShown ("", Float) = 1
		_WetnessLevel ("Level (Height Map dependent)", Range(0, 1.25)) = 1
		_WetnessConst ("Const (Height Map independent)", Range(0, 1)) = 0
		_WetnessColor ("Wetness Color (RGB Tint, A Opacity)", Vector) = (0,0,0,0)
		_WetnessDarkening ("Wetness darkening", Range(0, 8)) = 2
		_WetnessSpecGloss ("Wetness Specular (RGB) Gloss (A)", Vector) = (0.05,0.05,0.05,0.7)
		_WetnessEmissiveness ("Wetness emissiveness", Range(0, 20)) = 0
		_WetnessNormalInfluence ("Wetness normal override", Range(0, 1)) = 0.5
		_WetnessUVMult ("Wetness - Detail mask tiling", Float) = 1
		_RippleMap ("Ripple Map", 2D) = "bump" {}
		[NoKeywordToggle] _WetRipples ("Ripples (vertex color B)", Float) = 0
		_RippleMapWet ("Ripple Map wet (UI)", 2D) = "bump" {}
		_RippleStrength ("Strength", Range(0.01, 2)) = 0.5
		_RippleTiling ("Tiling", Float) = 1
		_RippleSpecFilter ("Spec filtering", Float) = 0.02
		_RippleAnimSpeed ("Anim speed", Float) = 0.5
		_FlowCycleScale ("Cycle scale", Float) = 2
		_RippleRefraction ("Refraction", Range(0, 2)) = 0.3
		_WetnessNormMIP ("Flow normal MIP level", Range(1, 8)) = 5
		_WetnessNormStrength ("Flow normal strength", Range(0, 4)) = 2
		[NoKeywordToggle] _WetnessEmissivenessWrap ("Emissiveness normal wrap", Float) = 0
		[NoKeywordToggle] _WetnessLevelFromGlobal ("Water level controlled globally", Float) = 0
		[NoKeywordToggle] _WetnessConstFromGlobal ("Water const controlled globally", Float) = 0
		[NoKeywordToggle] _WetnessFlowGlobalTime ("Water flow global time", Float) = 1
		[NoKeywordToggle] _WetnessMergeWithSnowPerMaterial ("Water level from snow level (per material)", Float) = 0
		[NoKeywordToggle] _RippleStrengthFromGlobal ("Flow ripple normal strength controlled globally", Float) = 0
		[NoKeywordToggle] _RainIntensityFromGlobal ("Rain Intensity controlled globally (multiplied)", Float) = 0
		[NoKeywordToggle] _WetDroplets ("Droplets (vertex color B)", Float) = 0
		_DropletsMap ("Droplets Map", 2D) = "bump" {}
		_RainIntensity ("Rain Intensity", Range(0, 1)) = 1
		_DropletsTiling ("Tiling", Float) = 1
		_DropletsAnimSpeed ("Anim speed", Float) = 10
		[NoKeywordToggle] _RainIntensityFromGlobal ("Rain Intensity controlled globally (multiplied)", Float) = 0
		_TessDepth ("Depth", Float) = 0.05
		_TessOffset ("Offset", Range(0, 1)) = 0.1
		_Tess ("Tessellation", Range(1, 60)) = 9
		_TessEdgeLengthLimit ("Tessellation edge limit", Range(2, 50)) = 5
		minDist ("Min camera disance", Float) = 1
		maxDist ("Max camera disance", Float) = 10
		_Phong ("Phong", Range(0, 1)) = 0
		[NoKeywordToggle] _Snow ("Snow", Float) = 0
		[Foldout] _SnowShown ("", Float) = 1
		_RippleMapSnow ("Ripple Map snow (UI)", 2D) = "bump" {}
		_SnowColorAndCoverage ("Color (RGB Tint, A Level)", Vector) = (1,1,1,1)
		_Frost ("Frost", Range(0, 1)) = 0
		_SnowSpecGloss ("Specular (RGB) Gloss (A)", Vector) = (0.1,0.1,0.1,0.15)
		_SnowSlopeDamp ("Slope damp", Range(0, 6)) = 2
		_SnowDiffuseScatteringColor ("Diffuse scattering", Vector) = (1,1,1,0)
		_SnowDiffuseScatteringExponent ("Diffuse scattering exponent", Range(2, 20)) = 6
		_SnowDiffuseScatteringOffset ("Diffuse scattering offset", Range(-0.5, 0.5)) = 0.4
		_SnowDeepSmoothen ("Deep smoothen", Range(0, 8)) = 4
		_SparkleMapSnow ("Snow detail (UI)", 2D) = "black" {}
		_SnowEmissionTransparency ("Snow emission transparency", Vector) = (0.1,0.1,0.1,1)
		_SnowMicroTiling ("Micro tiling", Float) = 4
		_SnowBumpMicro ("  Bumps micro", Range(0.001, 0.2)) = 0.08
		_SnowMacroTiling ("Macro tiling", Float) = 1
		_SnowBumpMacro ("  Bumps macro", Range(0.001, 0.5)) = 0.1
		[NoKeywordToggle] _SnowWorldMapping ("Snow World mapping", Float) = 0
		_SnowDissolve ("Dissolve", Range(0, 4)) = 2
		_SnowDissolveMaskOcclusion ("Dissolve mask occlusion", Range(0, 1)) = 0
		_SnowTranslucencyColor ("Translucency color", Vector) = (0.75,1,1,1)
		_SnowGlitterColor ("Glitter Color", Vector) = (0.8,0.8,0.8,0.2)
		_SnowHeightThreshold ("Height threshold", Float) = -10000
		_SnowHeightThresholdTransition ("Height threshold transition", Range(10, 4000)) = 1000
		[NoKeywordToggle] _SnowLevelFromGlobal ("Snow level controlled globally", Float) = 0
		[NoKeywordToggle] _FrostFromGlobal ("Frost controlled globally", Float) = 0
		[NoKeywordToggle] _SnowBumpMicroFromGlobal ("Snow bumps micro controlled globally", Float) = 0
		[NoKeywordToggle] _SnowDissolveFromGlobal ("Snow dissolve controlled globally", Float) = 0
		[NoKeywordToggle] _SnowSpecGlossFromGlobal ("Snow spec/gloss controlled globally", Float) = 0
		[NoKeywordToggle] _SnowGlitterColorFromGlobal ("Snow glitter color controlled globally", Float) = 0
		[NoKeywordToggle] _Glitter ("Glitter", Float) = 0
		[Foldout] _GlitterShown ("", Float) = 1
		_SparkleMap ("Sparkle & snow detail", 2D) = "black" {}
		_SparkleMapGlitter ("Glitter map (UI)", 2D) = "black" {}
		_GlitterColor ("Glitter Color", Vector) = (0.8,0.8,0.8,0.2)
		_GlitterColor2 ("Glitter Color2", Vector) = (0.8,0.8,0.8,0.2)
		_GlitterColorization ("Random colorization", Range(0, 1)) = 0.2
		_GlitterDensity ("Density", Range(0, 1)) = 0.2
		_GlitterTiling ("Tiling", Float) = 1
		_GlitterAnimationFrequency ("Animation frequency", Float) = 0.02
		_GlitterFilter ("Filtering", Range(-4, 4)) = -1
		_GlitterMask ("Masking", Range(0, 1)) = 0
		[Foldout] _TriplanarShown ("", Float) = 1
		_MainTexAverageColor ("Albdeo texture average color", Vector) = (0.5,0.5,0.5,0)
		_MainTex2AverageColor ("Albdeo 2 texture average color", Vector) = (0.5,0.5,0.5,0)
		_TriplanarBlendSharpness ("Blend sharpness", Range(1, 100)) = 10
		_TriplanarNormalBlendSharpness ("Blend sharpness normal", Range(0, 1)) = 0
		_TriplanarHeightmapBlendingValue ("Heightmap blending value", Range(0, 1)) = 0.3
		_TriplanarBlendAmbientOcclusion ("Ambient occlusion", Range(0, 1)) = 0.5
		[NoKeywordToggle] _TriplanarWorldMapping ("World mapping", Float) = 0
		[Foldout] _MainShown ("", Float) = 1
		[Foldout] _SecondaryShown ("", Float) = 1
		[Foldout] _PresetsShown ("", Float) = 0
		_Occlusion_from_albedo_alpha ("", Float) = 0
		[HideInInspector] _ShadowCull ("__shadowcull", Float) = 2
		heightMapTexelSize ("heightMap TexelSize fix for substance", Vector) = (1,1,1,1)
		bumpMapTexelSize ("bumpMap TexelSize fix for substance", Vector) = (1,1,1,1)
		bumpMap2TexelSize ("bumpMap2 TexelSize fix for substance", Vector) = (1,1,1,1)
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
	Fallback "Standard"
	//CustomEditor "UBER_StandardShaderGUI"
}