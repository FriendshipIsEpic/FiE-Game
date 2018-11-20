//--------------------------------------------------------------------------------------
//	Vertex shader
//	Position + Color + Texcoord
//--------------------------------------------------------------------------------------

#if defined(PK_VERTEX_SHADER)

float4x4	matWVP : register(c0);

struct		VS_IN
{
	float4	ObjPos				: POSITION;
	float4	Color				: COLOR;
	float2	UV					: TEXCOORD0;
	float3	Normal				: TEXCOORD1;
};

struct		VS_OUT
{
	float4	ProjPos		: POSITION;	
	float2	UV			: TEXCOORD0;
	float4	Color		: TEXCOORD1;
	float4	Pos			: TEXCOORD2;
	float3	Normal		: TEXCOORD3;
	float4	ProjPos2	: TEXCOORD4;
};

VS_OUT		main(VS_IN In)
{
	VS_OUT	Out;
	Out.UV = In.UV;
	Out.Color = In.Color;
	Out.ProjPos = mul(matWVP, In.ObjPos);
	Out.Pos = In.ObjPos;
	Out.Normal = In.Normal;
	Out.Normal.z = -Out.Normal.z;
	Out.ProjPos2 = Out.ProjPos;
	return Out;
}

#endif


//-------------------------------------------------------------------------------------
//	Pixel shader
//	Color + Texture
//-------------------------------------------------------------------------------------

#if defined(PK_PIXEL_SHADER)

float4		ZBufferParams : register(c0);
float		SoftnessDistance : register(c1);
sampler2D	ColorTexture : register(s0);
sampler2D	DepthTexture : register(s1);
sampler2D	NormalTexture : register(s2);

float4		PK_PS_PointLightPos : register(c2); // pos.x pos.y pos.z range
float4		PK_PS_PointLightColor : register(c3);
float3		PK_PS_CameraPos : register(c4);
float4		PK_PS_DIFFUSE : register(c5);
float4		PK_PS_SPECULAR : register(c6);
float2		PK_PS_SHININESS : register(c7);

struct		PS_IN
{
	float2	UV			: TEXCOORD0;
	float4	Color		: TEXCOORD1;
	float4	Pos			: TEXCOORD2;
	float3	Normal		: TEXCOORD3;
	float4	ProjPos		: TEXCOORD4;
};

float	clipToLinearDepth(float depth)
{
	float zNear = ZBufferParams.x;
	float zFar = ZBufferParams.y;
	return (-zNear * zFar) / (depth * (zFar - zNear) - zFar);
}

float4	main(PS_IN In) : COLOR
{
	float3 normMap = tex2D(NormalTexture, In.UV).xyz;
	normMap.z = -normMap.z;
	float3 norm = normalize(In.Normal + normMap);
	float3 lightVec = PK_PS_PointLightPos.xyz - In.Pos.xyz;
	float dist = length(lightVec);
	lightVec = normalize(lightVec);
	float attenuation = max(0.0, 1.0 - (dist / PK_PS_PointLightPos.w));
	float nxDir = max(0.0, dot(norm, lightVec));
	//float nxDir = 1.0;
	float4 diffuse = PK_PS_DIFFUSE * PK_PS_PointLightColor * attenuation * nxDir;
	
	float4 specular = float4(0.0, 0.0, 0.0, 0.0);
	if (PK_PS_SHININESS.y > 0 && nxDir != 0.0)
	{
		float3 camVec = normalize(PK_PS_CameraPos - In.Pos.xyz);
		float specularPow = pow(max(0.0, dot(camVec, reflect(-lightVec, norm))), PK_PS_SHININESS.x);
		specular = PK_PS_SPECULAR * PK_PS_PointLightColor * specularPow * attenuation;
	}
	
	float rcpw = 1.0 / In.Pos.w;
	float2 screenUV = In.ProjPos.xy * rcpw * float2(0.5, -0.5) + 0.5;
	float sceneDepth_cs = tex2D(DepthTexture, screenUV).x;
	float sceneDepth = clipToLinearDepth(sceneDepth_cs);
	float fragDepth = clipToLinearDepth(In.ProjPos.z * rcpw);
	float fade = SoftnessDistance * (sceneDepth - fragDepth);

	fade = saturate(fade);
	
	float4 texColor = tex2D(ColorTexture, In.UV);
	float4 outCol = In.Color * texColor;
	outCol = outCol + diffuse * texColor.a + specular * texColor.a;
	outCol *= fade;
	outCol.a = min(1.0, outCol.a);
	return outCol;
}

#endif