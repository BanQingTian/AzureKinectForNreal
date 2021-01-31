Shader "CustomShader/HollowwingFast"
{
    Properties
    {
		_MainTex("Texture", 2D) = "white" {}

		_Color("Color", Color) = (0.26,0.19,0.16,0.0)
		_Color2("Color2", Color) = (0.26,0.19,0.16,0.0)
		_Color2Pow("_Color2Pow", Float) = 1.0

		_Active("Active", Range(0.0,1.0)) = 0.0


		_AlphaRing("Alpha Ring", Float) = 1.0
		_AlphaRingInvert("Alpha Ring Invert", Float) = 0.0

		_Billborad("IsBilboard", Float) = 0.0

		_Intensity("Intensity", Float) = 1.0
			
    }
    SubShader
    {
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Tags { "DisableBatching" = "True" }
        LOD 100

		ZWrite Off
		Blend One One
		Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
			float4 _Color;
			float _Active;

			float _Color2Pow;
			float4 _Color2;


			float _AlphaRingInvert;
			float _AlphaRing;
			float _Billborad;

			float _Intensity;

            v2f vert (appdata v)
            {
                v2f o;

				if (_Billborad>0.5)
				{
					//float3 pos = mul(unity_ObjectToWorld, float4(0, 0, 0, 1));

					// billboard mesh towards camera

					float3 worldScale = float3(
						length(float3(unity_ObjectToWorld[0].x, unity_ObjectToWorld[1].x, unity_ObjectToWorld[2].x)), // scale x axis
						length(float3(unity_ObjectToWorld[0].y, unity_ObjectToWorld[1].y, unity_ObjectToWorld[2].y)), // scale y axis
						length(float3(unity_ObjectToWorld[0].z, unity_ObjectToWorld[1].z, unity_ObjectToWorld[2].z))  // scale z axis
						);


					o.vertex = mul(UNITY_MATRIX_P,
						mul(UNITY_MATRIX_MV, float4(0.0, 0.0, 0.0, 1.0))
						+ float4(v.vertex.x, v.vertex.y, 0.0, 0.0)
						* float4(worldScale.x, worldScale.y, 1.0, 1.0));
				}
				else 
				{
					o.vertex = UnityObjectToClipPos(v.vertex);
				}
                o.uv = v.uv;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {

				float AlphaRing = length(i.uv - 0.5);
				AlphaRing = saturate(pow(saturate(AlphaRing),0.5)*10* _AlphaRing);
				
				if (_AlphaRingInvert > 0.5)
				{
					AlphaRing = 1 - AlphaRing;
				}

				float2 uv = i.uv;
				float4 CA = tex2D(_MainTex, uv);
				float3 Grad = CA.rgb;
                


				float Alpha = CA.r*_Intensity;
				CA.rgb = _Color.rgb*_Color.a * 2;

				float3 color2 = _Color2.rgb * 4;
				CA.rgb = lerp(CA.rgb, color2, _Color2.a * saturate(10*pow(Grad, _Color2Pow)));

				CA *=  _Active* Alpha*AlphaRing;
				

				return float4(CA.rgb*(0.5+ Grad), 1);
            }
            ENDCG
        }
    }
}
