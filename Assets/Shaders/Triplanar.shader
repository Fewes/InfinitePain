Shader "Triplanar"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		[NoScaleOffset] _Ceiling ("Ceiling", 2D) = "white" {}
		[NoScaleOffset] _Wall ("Wall", 2D) = "white" {}
		[NoScaleOffset] _Floor ("Floor", 2D) = "white" {}
		[NoScaleOffset] _Scale ("Scale", Range(0, 1)) = 1
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" }

		CGPROGRAM
		#pragma surface surf Lambert vertex:vert

		#include "UnityCG.cginc"

		struct Input
		{
			float2 uv_MainTex;
			float3 textureWeights;
		};

		sampler2D _MainTex;
		sampler2D _Ceiling;
		sampler2D _Wall;
		sampler2D _Floor;
		float _Scale;

		void vert (inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input,o);
			float3 worldNormal = mul(UNITY_MATRIX_M, v.normal);
			o.textureWeights.x = max(worldNormal.y, 0);
			// o.textureWeights.y = worldNormal.y < 0.25 && worldNormal.y > -0.25;
			o.textureWeights.y = 1-smoothstep(0.25, 0.75, worldNormal.y*0.5+0.5);
			// o.textureWeights.z = worldNormal.y > 0.25;
			o.textureWeights.z = max(-worldNormal.y, 0);
		}

		void surf (Input IN, inout SurfaceOutput o)
		{
			// o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
			o.Albedo = tex2D (_Wall, IN.uv_MainTex * _Scale).rgb;
			o.Albedo = lerp(o.Albedo, tex2D (_Floor, IN.uv_MainTex * _Scale).rgb, IN.textureWeights.x);
			o.Albedo = lerp(o.Albedo, tex2D (_Ceiling, IN.uv_MainTex * _Scale).rgb, IN.textureWeights.z);
		}
		ENDCG
	} 
	Fallback "Diffuse"
}