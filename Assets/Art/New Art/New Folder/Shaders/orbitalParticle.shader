Shader "Finch/orbitalParticle"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}

        _Color1("Color 1", Color) = (1,1,1,1)
        _Color2("Color 2", Color) = (1,1,1,1)

        /*_ColorAddR("Color Add R", Color) = (1,1,1,1)
        _ColorAddG("Color Add G", Color) = (1,1,1,1)
        _ColorAddB("Color Add B", Color) = (1,1,1,1)*/
                
    }
        SubShader
        {
            Tags { "Queue" = "Transparent" }

            //  Blend SrcAlpha OneMinusSrcAlpha // Traditional transparency
                Blend One One // Additive
                // Cull Off
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
                        float2 uv : TEXCOORD0;
                        float4 color : TEXCOORD1;

                        float4 vertex : SV_POSITION;
                        float4 test : TEXCOORD4;

                        float3 normal : TEXCOORD2;
                        float3 V : TEXCOORD3;
                    };

                    sampler2D _MainTex;
                    float4 _MainTex_ST;
                    float4 _Color1;
                    float4 _Color2;
                    /*float4 _ColorAddR;
                    float4 _ColorAddG;
                    float4 _ColorAddB;*/


                    v2f vert(appdata v)
                    {
                        v2f o;

                        
                        o.vertex = UnityObjectToClipPos(v.vertex);
                        o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                        o.normal = normalize(v.normal);
                        o.color = v.color;



                        float3 wpos = mul(UNITY_MATRIX_M, v.vertex);


                        float3 center = 0;

                        float distance_center = length(_WorldSpaceCameraPos - center);
                        float distance = length(wpos - _WorldSpaceCameraPos);

                         float3 Target = normalize(wpos - center);


                        float3 V = -normalize(wpos - _WorldSpaceCameraPos);
                        o.V = V;

                        float TdotV = dot(Target, V) * 0.5 + 0.5;

                        float intensity = 0.1 + TdotV * TdotV * 0.9;

                        /*
                        if (frac(_Time.y*0.2) < 0.5 )//|| o.vertex.x<0
                        {
                            intensity = 1;
                        }
                        else {*/
                            intensity *= 10;
                            // }

                            float k_alpha = sin(_Time + wpos.y * 2 + (wpos.z + wpos.x) * 0.03) * 0.5 + 0.5;
                            o.color.a *= lerp(_Color1, _Color2, k_alpha);

                             o.color.a = max(0,intensity);

                             return o;
                         }

                         float freshnel(float3 N, float3 V)
                         {
                             float VdotN = dot(N, V);
                             float k = saturate(1 - VdotN);
                             return pow(k,3);
                         }



                         float4 frag(v2f i) : SV_Target
                         {
                             float4 masks = tex2D(_MainTex, i.uv);
                             float4 vert_colors = i.color;
                             float3 V = i.V;

                             float A = masks.r;
                             float3 C = masks.r * 0.3;

                            /* float3 color_add = _ColorAddR;
                             color_add = lerp(color_add, _ColorAddG, vert_colors.g);
                             color_add = lerp(color_add, _ColorAddB, vert_colors.b);

                             
                             C += color_add;*/

                            /* C += _Color1 + _Color2;*/

                             float F = freshnel(i.normal, V);

                             F = max(0, F);
                             F *= 5;

                             C *= 0.5;
                             C += F * 0.2 * (1 - 0.5 * vert_colors.b);

                             C *= i.color.a;

                             C *= F;
                             C *= _Color1 + _Color2;
                             
                             //C = i.test.r;

                              return float4(C, saturate(A * 5 + 0.1));
                          }
                          ENDCG
                      }
        }
}