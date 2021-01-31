Shader "Finch/thumbShader"
{
    Properties
    {
        _Color1("Color 1", Color) = (1,1,1,1)
        _Color2("Color 2", Color) = (1,1,1,1)

        _ColorMax("Color Max", Color) = (0.7,0.8,0.9,1)
        _Power("Power", float) = 2.0
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

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
                float3 normal : NORMAL;

                float4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 color : TEXCOORD0;
            };

            float4 _Color1;
            float4 _Color2;
            float _Power;
            float4 _ColorMax;

            float freshnel(float3 N, float3 V)
            {
                float VdotN = dot(N, V);
                float k = saturate(1 - VdotN);
                return k * k * k;
            }
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex    = UnityObjectToClipPos(v.vertex);

                float3 wpos = mul(UNITY_MATRIX_M, v.vertex);
                float3 V    = normalize(_WorldSpaceCameraPos - wpos);

                float k_alpha = sin(_Time * 10 + wpos.y * 2 + (wpos.z + wpos.x) * 0.03) * 0.5 + 0.5;
                float4 vert_colors = v.color * lerp(_Color1, _Color2, k_alpha);

                float F = max(0, freshnel(normalize(v.normal), V));
                F *= max(0, 1.0 - 0.5 * vert_colors.b);

                o.color =  vert_colors.a * (F*3 + _Color2.rgb) * _Power;
                o.color = min(o.color, _ColorMax);

                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                return float4(i.color, 1);
            }
            ENDCG
        }
    }
}