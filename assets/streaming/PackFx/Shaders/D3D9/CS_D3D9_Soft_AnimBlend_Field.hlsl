#if defined(PK_VERTEX_SHADER)

float4x4	matWVP : register(c0);						
														
struct		VS_IN										
{														
	float4	ObjPos			: POSITION;						
	float4	Color			: COLOR;						
	float2	UV0				: TEXCOORD0;					
	float2	UV1				: TEXCOORD1;					
	float	AtlasID			: TEXCOORD2;
	float	PK_LifeRatio	: TEXCOORD3;	
};														
														
struct		VS_OUT										
{														
	float4	ProjPos		: POSITION;						
	float4	UV			: TEXCOORD0;					
	float	FrameLerp	: TEXCOORD1;					
	float4	Color		: TEXCOORD2;					
	float4  Position	: TEXCOORD3;
	float	LifeRatio	: TEXCOORD4;	
};														
														
VS_OUT		main(VS_IN In)								
{														
	VS_OUT	Out;										
	Out.ProjPos = mul(matWVP, In.ObjPos);				
	Out.UV = float4(In.UV0, In.UV1);					
	Out.FrameLerp = frac(In.AtlasID);					
	Out.Color = In.Color;								
	Out.Position = Out.ProjPos;
	Out.LifeRatio = In.PK_LifeRatio;	
	return Out;											
}														

#endif

//------------------------------------------------

#if defined(PK_PIXEL_SHADER)

float4		ZBufferParams : register(c0);									
float		SoftnessDistance : register(c1);								
sampler2D	ColorTexture : register(s0);									
sampler2D	DepthTexture : register(s1);									
																			
struct		PS_IN															
{																			
	float4	UV			: TEXCOORD0;										
	float	FrameLerp	: TEXCOORD1;										
	float4	Color		: TEXCOORD2;										
	float4	ProjPos		: TEXCOORD3;
	float	LifeRatio	: TEXCOORD4;
};																			
																			
																			
float	clipToLinearDepth(float depth)											
{																				
	float zNear = ZBufferParams.x;												
	float zFar = ZBufferParams.y;												
	return (-zNear * zFar) / (depth * (zFar - zNear) - zFar);					
}																				
																			
float4	main(PS_IN In) : COLOR												
{																			
	float rcpw = 1.0 / In.ProjPos.w;										
	float2 screenUV = In.ProjPos.xy * rcpw * float2(0.5, -0.5) + 0.5;		
	float sceneDepth_cs = tex2D(DepthTexture, screenUV).x;					
	float sceneDepth = clipToLinearDepth(sceneDepth_cs);					
	float fragDepth = clipToLinearDepth(In.ProjPos.z * rcpw);				
	float fade = SoftnessDistance * (sceneDepth - fragDepth);				
	fade = saturate(fade);													
																			
																			
	float4 diffA = tex2D(ColorTexture, In.UV.xy);							
	float4 diffB = tex2D(ColorTexture, In.UV.zw);							
	float4 outCol = In.Color * lerp(diffA, diffB, In.FrameLerp);

	if (In.LifeRatio > 0.5f)
	{
		float tmp = outCol.g;
		outCol.g = outCol.r;
		outCol.r = tmp;
	}
	
	outCol *= fade;															
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

