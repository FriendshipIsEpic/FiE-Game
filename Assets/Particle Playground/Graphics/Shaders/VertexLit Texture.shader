Shader "Playground/Vertex Lit Texture" {
	Properties {
		_Color ("Main Color", Color) = (1, 1, 1, 1)
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader {
		Tags { "Queue" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		Lighting On
		ColorMaterial AmbientAndDiffuse
		Zwrite On
		BindChannels {
			Bind "Color", color
			Bind "Vertex", vertex
			Bind "normal", normal
			Bind "TexCoord", texcoord
		}
		Material {
			Diffuse [_Color]
			Ambient [_Color]
			Emission[_Vertex]
		}
        Pass {
            SetTexture [_MainTex] {
                combine texture * primary
            }
        }
	}
}