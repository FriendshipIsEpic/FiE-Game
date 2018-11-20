#if defined(PK_VERTEX_SHADER)

float4x4	matWVP : register(c0);						
														
struct		VS_IN										
{														
	float4	ObjPos		: POSITION;						
	float4	Color		: COLOR;						
	float2	UV			: TEXCOORD;						
};														
														
struct		VS_OUT										
{														
	float4	Position	: SV_POSITION;					
	float2	UV			: TEXCOORD0;					
	float4	Color		: COLOR;						
};														
														
VS_OUT		main(VS_IN In)								
{														
	VS_OUT	Out;										
	Out.Position = mul(matWVP, In.ObjPos);				
	Out.UV = In.UV;										
	Out.Color = In.Color;								
	return Out;											
}														

#endif

//------------------------------------------------

#if defined(PK_PIXEL_SHADER)

Texture2D		ColorTexture	: register(t0);				
SamplerState	ColorSampler	: register(s0);				
															
struct		PS_IN											
{															
	float4	ProjPos		: SV_POSITION;						
	float2	UV			: TEXCOORD0;						
	float4	Color		: COLOR;							
};															

cbuffer PK_PS : register(b1)
{
	float2	UV_offset;
};
															
float4	main(PS_IN In) : SV_TARGET							
{
	In.UV += UV_offset;
	float4	t = ColorTexture.Sample(ColorSampler, In.UV);	
	float4	outCol = In.Color * t;							
#if defined(MAT_ADDITIVE_ALPHA)							
	outCol *= outCol.w;										
	outCol.w = 0.0f;										
#endif														
#if defined(MAT_ADDITIVE_NOALPHA)							
	outCol.w = 0.0f;										
#endif														
	return outCol;											
}															

#endif

