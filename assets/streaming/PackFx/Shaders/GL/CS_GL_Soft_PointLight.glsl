#ifdef PK_VERTEX_SHADER

uniform mat4	MatWVP;

in vec3		InVertex;
in vec4		InColor;
in vec2		InTexcoord;
in vec3		InNormal;

out vec4		FragColor;
out vec2		Texcoord;
out vec4		ProjPos;
out vec3		Normal;
out vec4		Pos;

void main()
{	
	vec4 position = MatWVP  * vec4(InVertex, 1);
	gl_Position = position;
	ProjPos = position;
	FragColor = InColor;
	Texcoord = InTexcoord;
	Normal = InNormal;
	Normal.z = -Normal.z;
	Pos = vec4(InVertex, 1);
}

#endif

#ifdef PK_PIXEL_SHADER

uniform sampler2D	Texture;
uniform sampler2D	DepthRT;
uniform sampler2D	NormalTexture;
uniform vec4		ZBufferParams;
uniform float		InvSoftnessDistance;

uniform vec4		PK_PointLightPos; // pos.x pos.y pos.z range
uniform vec4		PK_PointLightColor;
uniform vec3		PK_CameraPos;
uniform vec4		PK_DIFFUSE;
uniform vec4		PK_SPECULAR;
uniform vec2		PK_SHININESS;

out vec4	OutFragColor;

in vec4		FragColor;
in vec2		Texcoord;
in vec4		ProjPos;
in vec3		Normal;
in vec4		Pos;

float	clipToLinearDepth(float depth)
{
	float zNear = ZBufferParams.x;
	float zFar = ZBufferParams.y;
	return (2.0 * zNear * zFar) / (zFar + zNear - depth * (zFar - zNear));
}

void main()
{
	vec3 normMap = texture2D(NormalTexture, Texcoord).xyz;
	normMap.z = -normMap.z;
	vec3 norm = normalize(Normal + normMap);
	vec3 lightVec = PK_PointLightPos.xyz - Pos.xyz;
	float dist = distance(PK_PointLightPos.xyz, Pos.xyz);
	lightVec = normalize(lightVec);
	float attenuation = max(0.0, 1.0 - (dist / PK_PointLightPos.w));
	float nxDir = max(0.0, dot(norm, lightVec));
	//float nxDir = 1.0;
	vec4 diffuse = PK_DIFFUSE * PK_PointLightColor * attenuation * nxDir;
	
	vec4 specular = vec4(0.0, 0.0, 0.0, 0.0);
	if (PK_SHININESS.y > 0 && nxDir != 0.0)
	{
		vec3 camVec = normalize(PK_CameraPos - Pos.xyz);
		float specularPow = pow(max(0.0, dot(camVec, reflect(-lightVec, norm))), PK_SHININESS.x);
		specular = PK_SPECULAR * PK_PointLightColor * specularPow * attenuation;
	}

	float rcpw = 1.0 / ProjPos.w;
	vec2 screenUV = ProjPos.xy * rcpw * 0.5 + 0.5;
	float sceneDepth_cs = texture2D(DepthRT, screenUV).x;
	float sceneDepth = clipToLinearDepth(sceneDepth_cs * 2.0 - 1.0);
	float fragDepth = clipToLinearDepth(ProjPos.z * rcpw);
	float fade = clamp(InvSoftnessDistance * (sceneDepth - fragDepth), 0.0, 1.0);
	
	vec4 texColor = texture2D(Texture, Texcoord);
	vec4 outCol = FragColor * texColor;
	outCol = outCol + diffuse * texColor.a + specular * texColor.a;
	outCol *= fade;
	outCol.a = min(1.0, outCol.a);
	OutFragColor = outCol;
}

#endif