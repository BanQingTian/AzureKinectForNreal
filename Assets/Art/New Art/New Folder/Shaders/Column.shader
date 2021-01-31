// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Finch/Column (VR)"
{
    Properties
    {
        _BaseColor          ("Base color", Color) = (0.075, 0.075, 0.075, 1)
       // _AnimColor          ("Anim color", Color) = (0.25,0.25,0.05,1)

        _FresnelColor       ("Fresnel Color (RGB) | Amount (A", Color) = (0.5,0.75, 1, 0.1)

       // _PodiumOriginY      ("Podium Origin Y",     Float)  = -0.065

       // _FadeHeight         ("Fade Height",         Range(0.01, 3.0)) = 0.75
       // _FadeHeightMin      ("Fade Height Min",     Range(0.01, 1.0)) = 0.1
        //_FadeHeightPower    ("Fade Height Power",   Range(0.25, 4.0)) = 2.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }

        ZWrite  On
        ZTest   LEqual

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex   : POSITION;
                float2 uv       : TEXCOORD0;
                float3 normal   : NORMAL;
                float4 color    : COLOR;
            };

            struct v2f
            {
                float2 uv       : TEXCOORD0;
                float4 color    : TEXCOORD1;
                float4 vertex  : SV_POSITION;

                float3 normal : TEXCOORD2;
            };

            float4 _BaseColor;
           // float4 _AnimColor;
            float4 _FresnelColor;

            // float _FadeHeight;
          //  float _FadeHeightMin;
          //  float _FadeHeightPower;

           // float _PodiumOriginY;

            float freshnel(float3 N, float3 V)
            {
                float VdotN = dot(N, V);
                float k = saturate(1 - VdotN);
                return pow(k,3);
            }

            v2f vert(appdata v)
            {
                v2f o;
                
                o.vertex    = UnityObjectToClipPos(v.vertex);

                float3 wpos = mul (unity_ObjectToWorld, v.vertex).xyz;

                float3 V = - normalize(wpos - _WorldSpaceCameraPos);

                // float k_alpha   = sin(_Time * 40 + wpos.y * 40 + (wpos.z * 100 * 100 + wpos.x * 100 * 100) * 0.03) * 0.5 + 0.5;
              //  o.color.a       = k_alpha * _AnimColor.a * 10;
                o.color.r       = freshnel(v.normal, V) * _FresnelColor.a * 10;

               // o.color.g = pow(saturate(1-(wpos.y-_PodiumOriginY)  /_FadeHeight), _FadeHeightPower) * (1.0 - _FadeHeightMin) + _FadeHeightMin;

                return o;
            }


            float4 frag(v2f i) : SV_Target
            {
                float3 C = _BaseColor.rgb 
                //+ i.color.a * _AnimColor.rgb 
                         + i.color.r * _FresnelColor.rgb;
                
               // C *= i.color.g;

                return float4(C, 1);
            }
            ENDCG
        }
    }
}