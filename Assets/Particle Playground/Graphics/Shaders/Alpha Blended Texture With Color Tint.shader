Shader "Playground/Alpha Blended Texture With Color Tint" {
	Properties {
		_Color ("Main Color", Color) = (1, 1, 1, 1)
	    _MainTex ("Texture", 2D) = "white" {}
	}
	 
	Category {
	    Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	    Blend SrcAlpha OneMinusSrcAlpha
	    Cull Off Lighting Off ZWrite Off Fog {Mode Off}
	   
	    BindChannels {
	        Bind "Color", color
	        Bind "Vertex", vertex
	        Bind "TexCoord", texcoord
	    }
	   
	    SubShader {
	        Pass {
	            SetTexture [_MainTex] {
					combine texture * primary
	            }
	            SetTexture [_MainTex] {
	          		constantColor [_Color]
					combine previous * constant
	            }
	        }
	    }
	}
}