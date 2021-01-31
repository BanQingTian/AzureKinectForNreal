// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Finch/podium" {
Properties {
    _TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
    _MainTex ("Particle Texture", 2D) = "white" {}
    //_InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
    _MainColor("MainColor", Color) = (1,1,1,1)
}

Category {
    Tags { "Queue"="Transparent"} //"IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
    Blend OneMinusDstColor One // Soft Additive
    
    //ColorMask RGB
    Cull Off Lighting Off 
        
    ZWrite On

    ZTest LEqual

    SubShader {
        Pass {

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            fixed4 _TintColor;
            float4 _MainColor;

            struct appdata_t {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;

                float4 tints : TEXCOORD1;
            };

            float4 _MainTex_ST;

            v2f vert (appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
           
                o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);

                o.tints = 2.0f * v.color * _TintColor * _MainColor;

                return o;
            }


            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = i.tints * tex2D(_MainTex, i.texcoord);
                col.a = saturate(col.a); 
                return col;
            }
            ENDCG
        }
    }
}
}
