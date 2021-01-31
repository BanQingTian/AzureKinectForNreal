// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Finch/floor" {
    Properties{
        _TintColor("Tint Color", Color) = (0.5,0.5,0.5,0.5)
        _MainTex("Particle Texture", 2D) = "white" {}
        /*_InvFade("Soft Particles Factor", Range(0.01,3.0)) = 1.0*/
        _MainColor("MainColor", Color) = (1,1,1,1)
        _FadeDistance("Fade Distance", Float) = 2.0
        _FogFar("Fog Far", Float) = 2.0
        _Intensity("Intensity", Float) = 5.0
    }

        Category{
            Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" /*"PreviewType" = "Plane"*/ }
            /*Blend SrcAlpha One*/
            Blend One One
            /*ColorMask RGB*/
             ZWrite off
			 ZTest LEqual

            SubShader {
                Pass {

                    CGPROGRAM
                    #pragma vertex vert
                    #pragma fragment frag
                   /* #pragma target 2.0
                    #pragma multi_compile_particles*/
                    /*#pragma multi_compile_fog*/

                    #include "UnityCG.cginc"

                    

                    sampler2D _MainTex;
                    float4 _TintColor;
                    float4 _MainColor;

                    struct appdata_t {
                        float4 vertex : POSITION;
                        float4 color : COLOR;
                        float2 texcoord : TEXCOORD0;
                        /*UNITY_VERTEX_INPUT_INSTANCE_ID*/
                    };

                    struct v2f {
                        float4 vertex : SV_POSITION;
                        float4 color : TEXCOORD1;
                        float2 texcoord : TEXCOORD0;
                        float3 V : TEXCOORD3;
                       /* UNITY_FOG_COORDS(1)*/
                        /*#ifdef SOFTPARTICLES_ON
                        float4 projPos : TEXCOORD2;
                        
                        #endif
                        UNITY_VERTEX_OUTPUT_STEREO*/
                    };

                    float4 _MainTex_ST;
                    float _FadeDistance;
                    float _FogFar;
                    float _Intensity;
                    v2f vert(appdata_t v)
                    {
                        v2f o;
                        /*unity_setup_instance_id(v);
                        unity_initialize_vertex_output_stereo(o);*/
                        o.vertex = UnityObjectToClipPos(v.vertex);
                        /*#ifdef SOFTPARTICLES_ON
                        o.projPos = ComputeScreenPos(o.vertex);
                        COMPUTE_EYEDEPTH(o.projPos.z);
                        #endif*/
                        o.color = v.color;
                        o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
                        /*UNITY_TRANSFER_FOG(o,o.vertex);*/
                        
                        float3 wpos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1)).xyz;

                        float3 center = 0;

                        float distance_center = length(_WorldSpaceCameraPos - center);
                        float distance = length(wpos - _WorldSpaceCameraPos);

                        float3 Target = normalize(wpos - center);


                        float3 V = -normalize(wpos - _WorldSpaceCameraPos);
                        o.V = V;

                        float TdotV = dot(Target, V) * 0.5 + 0.5;
                        
                        float intensity = 0.1 + TdotV * TdotV * 0.9;
                        
                        intensity *= 3;

                        o.color.rgb = pow(saturate(1 - length(wpos - center) / _FadeDistance), 4);

                        o.color.rgb *= pow(saturate(1 - length(wpos - _WorldSpaceCameraPos) / _FogFar), 4);


                        o.color.rgb = saturate(o.color.rgb);

                        return o;
                    }

                    /*UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
                    float _InvFade;*/

                    float4 frag(v2f i) : SV_Target
                    {
                        /*#ifdef SOFTPARTICLES_ON
                        float sceneZ = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
                        float partZ = i.projPos.z;
                        float fade = saturate(_InvFade * (sceneZ - partZ));
                        i.color.a *= fade;
                        float3 V = i.V;
                        #endif*/

                        float4 col = 2.0f * i.color * _TintColor * tex2D(_MainTex, i.texcoord);
                        /*col.a = saturate(col.a);*/ // alpha should not have double-brightness applied to it, but we can't fix that legacy behaior without breaking everyone's effects, so instead clamp the output to get sensible HDR behavior (case 967476)
                        col *= _MainColor;
                        /*UNITY_APPLY_FOG_COLOR(i.fogCoord, col, fixed4(0,0,0,0));*/ // fog towards black due to our blend mode
                        return col * _Intensity;
                    }
                    ENDCG
                }
            }
        }
}
