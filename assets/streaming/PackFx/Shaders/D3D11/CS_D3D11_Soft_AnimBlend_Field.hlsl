#if defined(PK_VERTEX_SHADER)

struct SPixelInput
{
	float4 Position : SV_POSITION;
	float4 ProjPos : TEXCOORD0;
	float4 Color : COLOR;
	float4 UV : TEXCOORD1;
	float FrameLerp : TEXCOORD2;
	float LifeRatio : TEXCOORD3;
};
struct VS_IN
{
	float3 ObjPos : POSITION;
	float4 Color : COLOR;
	float2 UV0 : TEXCOORD0;
	float2 UV1 : TEXCOORD1;
	float AtlasID : TEXCOORD2;
	float PK_LifeRatio : TEXCOORD3;
};
float4x4 matWVP : register(c0);
float4x4 matWV : register(c4);
SPixelInput main(VS_IN In)
{
	SPixelInput Out;
	Out.Position = mul(matWVP, float4(In.ObjPos, 1));
	Out.ProjPos = Out.Position;
	Out.Color = In.Color;
	Out.UV = float4(In.UV0, In.UV1);
	Out.FrameLerp = frac(In.AtlasID);
	Out.LifeRatio = In.PK_LifeRatio;
	return Out;
}

#endif

//------------------------------------------------

#if defined(PK_PIXEL_SHADER)

struct SPixelInput
{
	float4 Position : SV_POSITION;
	float4 ProjPos : TEXCOORD0;
	float4 Color : COLOR;
	float4 UV : TEXCOORD1;
	float FrameLerp : TEXCOORD2;
	float LifeRatio	: TEXCOORD3;
};
float4 UniformsPack : register(c0);
bool HasDiffuse() { return (asuint(UniformsPack.x) &  1U ); }
bool IsAlphaMultiply() { return (asuint(UniformsPack.x) &  2U ); }
bool IsAdditive() { return (asuint(UniformsPack.x) &  4U ); }
bool IsOrthoCam() { return (asuint(UniformsPack.x) &  8U ); }
bool IsRotateTexture() { return (asuint(UniformsPack.x) &  16U ); }
bool IsDepthReversed() { return (asuint(UniformsPack.x) & 32U); }
float InvSoftnessDistance() { return asfloat(UniformsPack.y); }
float4 ZBufferParams : register(c1);
Texture2D ColorTexture : register(t0);
SamplerState ColorSampler : register(s0);
SamplerState DepthSampler : register(s1);
Texture2D DepthTexture : register(t1);
float clipToLinearDepth(float depth)
{
	float zNear = ZBufferParams.x;
	float zFar = ZBufferParams.y;
	if (IsOrthoCam())
		return depth * (zFar - zNear) + zNear;
	else
		return (-zNear * zFar) / (depth * (zFar - zNear) - zFar);
}
float4 main(SPixelInput In) : SV_TARGET
{
	float4 diffuse = float4(1.0, 1.0, 1.0, 1.0);
	if (HasDiffuse())
	{
		float2 uv0 = In.UV.xy;
		float2 uv1 = In.UV.zw;
		float4 diffuseA = ColorTexture.Sample(ColorSampler, uv0);
		float4 diffuseB = ColorTexture.Sample(ColorSampler, uv1);
		diffuse = lerp(diffuseA, diffuseB, In.FrameLerp);
	}
	float4 projPos = In.ProjPos;
	float rcpw = 1.0 / projPos.w;
	float fragDepth_cs = projPos.z * rcpw;
	if (IsDepthReversed())
		fragDepth_cs = 1.0 - fragDepth_cs;
	float2 screenUV = projPos.xy * rcpw * float2(0.5, -0.5) + 0.5;
	float sceneDepth_cs = DepthTexture.Sample(DepthSampler, screenUV).x;
	float sceneDepth = clipToLinearDepth(sceneDepth_cs);
	float fragDepth = clipToLinearDepth(fragDepth_cs);
	float depthfade = saturate((sceneDepth - fragDepth) * InvSoftnessDistance());
	diffuse *= In.Color;
	
	if (In.LifeRatio > 0.5f)
	{
		float tmp = diffuse.g;
		diffuse.g = diffuse.r;
		diffuse.r = tmp;
	}
	
	if (IsAlphaMultiply())
		diffuse *= diffuse.w;
	if (IsAdditive())
	{
		diffuse *= depthfade;
		diffuse.w = 0.0f;
	}
	else
	{
		diffuse.w *= depthfade;
	}
	return diffuse;
}

#endif

