#if defined(PK_VERTEX_SHADER)

float4x4	matWVP : register(c0);						
														
struct		VS_IN										
{														
	float4	ObjPos	: POSITION;							
	float2	UV		: TEXCOORD;							
	float4	Color	: COLOR;							
};														
														
struct		VS_OUT										
{														
	float4	ProjPos	 : POSITION;						
	float2	UV		 : TEXCOORD0;						
	float4	Color	 : TEXCOORD1;						
	float4  Position : TEXCOORD2;						
};														
														
VS_OUT		main(VS_IN In)								
{														
	VS_OUT	Out;										
	Out.ProjPos = mul(matWVP, In.ObjPos);				
	Out.UV = In.UV;										
	Out.Color = In.Color;								
	Out.Position = Out.ProjPos;							
	return Out;											
}														

#endif

//------------------------------------------------

#if defined(PK_PIXEL_SHADER)

sampler2D	ColorTexture : register(s0);

float2		PK_PS_UV_offset : register(c0);
				
																
struct		PS_IN												
{																
	float2	UV			: TEXCOORD0;							
	float4	Color		: TEXCOORD1;							
};																
																
																
float4	main(PS_IN In) : COLOR									
{
	In.UV += PK_PS_UV_offset;										
	float4 outCol = In.Color * tex2D(ColorTexture, In.UV);		
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

