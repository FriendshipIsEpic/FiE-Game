#ifdef PK_VERTEX_SHADER

uniform mat4	MatWVP;												
																	
in vec3 InVertex;
in vec4 InColor;
in vec2 InTexcoord;
in vec2 InTexcoord2;
in float InAtlasID;
in float PK_LifeRatio;
																	
out vec4		FragColor;										
out vec4		Texcoord;										
out float		FrameLerp;										
out vec4		ProjPos;
out float		LifeRatio;										
																	
void main()														
{																	
	vec4		position = MatWVP * vec4(InVertex, 1);				
	gl_Position = position;											
	ProjPos = position;												
	FragColor = InColor;											
	Texcoord = vec4(InTexcoord, InTexcoord2);						
	FrameLerp = fract(InAtlasID);		
	LifeRatio = PK_LifeRatio;		
}																	

#endif

//------------------------------------------------

#ifdef PK_PIXEL_SHADER

uniform sampler2D	Texture;													
uniform sampler2D	DepthRT;													
uniform vec4		ZBufferParams;												
uniform float		InvSoftnessDistance;	
in float			LifeRatio;									
																				
out vec4 OutFragColor;
																				
in vec4		FragColor;													
in vec4		Texcoord;													
in float		FrameLerp;													
in vec4		ProjPos;													
																				
float	clipToLinearDepth(float depth)											
{																				
	float zNear = ZBufferParams.x;												
	float zFar = ZBufferParams.y;												
	return (2.0 * zNear * zFar) / (zFar + zNear - depth * (zFar - zNear));		
}																				
																				
void main()																	
{																				
	float rcpw = 1.0 / ProjPos.w;												
	vec2 screenUV = ProjPos.xy * rcpw * 0.5 + 0.5;								
	float sceneDepth_cs = texture2D(DepthRT, screenUV).x;						
	float sceneDepth = clipToLinearDepth(sceneDepth_cs * 2.0 - 1.0);			
	float fragDepth = clipToLinearDepth(ProjPos.z * rcpw);						
	float fade = clamp(InvSoftnessDistance * (sceneDepth - fragDepth), 0.0, 1.0);
																				
	vec4 colorA = texture2D(Texture, Texcoord.xy);								
	vec4 colorB = texture2D(Texture, Texcoord.zw);								
																				
	vec4 color = FragColor * mix(colorA, colorB, FrameLerp);			

	if (LifeRatio > 0.5f)
	{
		float tmp = color.g;
		color.g = color.r;
		color.r = tmp;
	}
	
	color *= fade;																
																				
#ifdef ADDITIVE_ALPHA															
	color *= color.w;															
	color.w = 0.f;																
#endif																			
																				
#ifdef ADDITIVE_NOALPHA														
	color.w = 0.f;																
#endif																			
																				
	OutFragColor = color;														
}					

#endif

