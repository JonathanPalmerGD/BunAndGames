Shader "Bubble" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_SubTex ("Sub-texture (RGB)", 2D) = "white" {}
		_EmisColor ("Emissive Color", Color) = (0.2,0.2,0.2,0)
		_Color ("Multiply Color", Color) = (0.0,0.0,0.0,1.0)
	}
	SubShader {
		Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
		LOD 200
		
		Lighting Off
		Material {
			Emission [_EmisColor]
		}
		ZWrite Off
		Cull Off
		Pass {
			Tags { "LIGHTMODE"="Vertex" "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
			Lighting Off
			Material {
				Emission [_EmisColor]
			}
			ZWrite Off
			Cull Off
			Blend SrcAlpha OneMinusSrcAlpha
			AlphaTest Greater 0.001
			ColorMask RGB
			ColorMaterial AmbientAndDiffuse
			SetTexture [_MainTex] { combine primary * texture }
		}
		CGPROGRAM
		#pragma surface surf Lambert alpha

		sampler2D _MainTex;
		sampler2D _SubTex;
		float4 _Color;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			half4 s = tex2D (_SubTex, IN.uv_MainTex);
			o.Albedo = s.rgb*_Color.rgb;
			o.Alpha = (c.a * _Color.a);
		}
		ENDCG
	} 
	FallBack "Particles/Additive(Soft)"
	}