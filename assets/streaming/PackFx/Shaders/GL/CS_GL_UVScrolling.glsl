#ifdef PK_VERTEX_SHADER

uniform mat4	MatWVP;												
																	
in vec3 InVertex;
in vec4 InColor;
in vec2 InTexcoord;
																	
out vec4		FragColor;										
out vec2		Texcoord;										
																	
void main()														
{																	
	gl_Position = MatWVP * vec4(InVertex, 1);						
	FragColor = InColor;											
	Texcoord = InTexcoord;											
}																	

#endif

//------------------------------------------------

#ifdef PK_PIXEL_SHADER

uniform sampler2D	Texture;										
																	
out vec4 OutFragColor;
																	
in vec4		FragColor;										
in vec2		Texcoord;	

uniform vec2		PK_UV_offset;									
																	
void main()														
{
	vec2 UV = Texcoord + PK_UV_offset;
	vec4 color = FragColor * texture2D(Texture, UV);			
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

