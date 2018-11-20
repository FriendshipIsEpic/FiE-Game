#if defined(PK_VERTEX_SHADER)

struct SPixelInput
{
	float4 Position : SV_POSITION;
	float4 ProjPos : TEXCOORD0;
	float4 Color : COLOR;
	float2 UV : TEXCOORD1;
	float3 Normal : TEXCOORD2;
	float4 Pos : TEXCOORD3;
};
struct VS_IN
{
	float3 ObjPos : POSITION;
	float4 Color : COLOR;
	float2 UV : TEXCOORD0;
	float3 Normal : TEXCOORD1;
};
float4x4 matWVP : register(c0);
float4x4 matWV : register(c4);
SPixelInput main(VS_IN In)
{
	SPixelInput Out;
	Out.Position = mul(matWVP, float4(In.ObjPos, 1));
	Out.ProjPos = Out.Position;
	Out.Color = In.Color;
	Out.UV = In.UV;
	Out.Normal = In.Normal;
	Out.Normal.z = -Out.Normal.z;
	Out.Pos = float4(In.ObjPos, 1);
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
	float2 UV : TEXCOORD1;
	float3 Normal : TEXCOORD2;
	float4 Pos : TEXCOORD3;
};

cbuffer PK_PS : register(b1)
{
	float4 PointLightPos; // pos.x pos.y pos.z range
	float4 PointLightColor;
	float3 CameraPos;
	float4 DIFFUSE;
	float4 SPECULAR;
	float2 SHININESS;
}

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
Texture2D NormalTexture : register(t2);
SamplerState NormalSampler : register(s2);

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
	float4 texColor = float4(1.0, 1.0, 1.0, 1.0);
	if (HasDiffuse())
	{
		float2 uv = In.UV;
		texColor = ColorTexture.Sample(ColorSampler, uv);
	}
	
	float3 normMap = NormalTexture.Sample(NormalSampler, In.UV).xyz;
	normMap.z = - normMap.z;
	float3 norm = normalize(In.Normal + normMap);
	float3 lightVec = PointLightPos.xyz - In.Pos.xyz;
	float dist = length(lightVec);
	lightVec = normalize(lightVec);
	float attenuation = max(0.0, 1.0 - (dist / PointLightPos.w));
	float nxDir = max(0.0, dot(norm, lightVec));
	float4 diffuse = DIFFUSE * PointLightColor * attenuation * nxDir;
	
	float4 specular = float4(0.0, 0.0, 0.0, 0.0);
	if (SHININESS.y > 0 && nxDir != 0.0)
	{
		float3 camVec = normalize(CameraPos - In.Pos.xyz);
		float specularPow = pow(max(0.0, dot(camVec, reflect(-lightVec, norm))), SHININESS.x);
		specular = SPECULAR * PointLightColor * specularPow * attenuation;
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
	
	float4 outCol = In.Color * texColor;
	outCol = outCol + diffuse * texColor.a + specular * texColor.a;

	if (IsAlphaMultiply())
		outCol *= outCol.w;
	if (IsAdditive())
	{
		outCol *= depthfade;
		outCol.w = 0.0f;
	}
	else
	{
		outCol.w *= depthfade;
	}
	return outCol;
}

#endif

