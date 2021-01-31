Shader "Hyperspace Teleport/Standard1" {
	Properties {
		_MainTex    ("Main", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0, 1)) = 0.5
		_Metallic   ("Metallic", Range(0, 1)) = 0
		_NoiseTex   ("Noise", 2D) = "black" {}
		_ClipPlane  ("Clip Plane", Vector) = (0, 0, 0, 0)
		_Clip       ("Clip", Float) = 1
		_Halo       ("Halo", Float) = 0.5
		_HaloColor  ("Halo Color", Color) = (1, 1, 0, 1)
		_Bloom      ("Bloom", Float) = 1.5
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }
		Cull Off
		CGPROGRAM
		#pragma surface surf Standard addshadow finalcolor:teleport
		#pragma target 3.0
		#pragma multi_compile HT_DIR_X HT_DIR_Y HT_DIR_Z
		#pragma multi_compile HT_FORWARD HT_BACKWARD
		#pragma multi_compile _ HT_NOISE
		#include "Core.cginc"
		float _Bloom;
		struct Input
		{
			float2 uv_MainTex;
			float3 worldPos;
		};
		void surf (Input IN, inout SurfaceOutputStandard o)
		{
			HTClipFrag(IN.worldPos, IN.uv_MainTex);
			float4 tc = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = tc.rgb;
			o.Alpha = tc.a;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
		}
		void teleport (Input IN, SurfaceOutputStandard o, inout fixed4 color)
		{
			float s = HTClipFrag(IN.worldPos, IN.uv_MainTex);
			color.rgb = lerp(color.rgb, _HaloColor.rgb * _Bloom, s);
		}
		ENDCG
	}
	FallBack "Diffuse"
}
