// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Finch/particles"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        _TintR("Tint R", Color) = (1,1,1,1)
        _TintG("Tint G", Color) = (1,1,1,1)
        _TintB("Tint B", Color) = (1,1,1,1)

        _ColorAddR("Color Add R", Color) = (1,1,1,1)
        _ColorAddG("Color Add G", Color) = (1,1,1,1)
        _ColorAddB("Color Add B", Color) = (1,1,1,1)


		
		
            //g_fFogFar       ("Fog Far", 		Float) = 120


        //g_fAlphaReintegrationEnabled ("Alpha Reintegration Enabled", Range (0, 1)) = 0.0
			
        //g_fAabbMin       ("Aabb Min", Float) = -0.5
        //g_fAabbMax       ("Aabb Max", Float) = 0.5
        g_vOrigin        ("Origin", Vector) = (0, 0, 0, 0)
		
         //g_bDebugAABB     ("Debug Aabb", Float) = 1.0
        
        //g_fDissolveAlpha ("Dissolve Alpha", Range (0, 1)) = 1.0
        //g_fDissolveRange ("Dissolve Range", Range (0, 1)) = 0.35
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" }

    //  Blend SrcAlpha OneMinusSrcAlpha // Traditional transparency
	
       // Blend One One // Additive
        Blend OneMinusDstColor One
       
	   // Cull Off

        ZWrite On
        ZTest LEqual

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex 	: POSITION;
                float2 uv 		: TEXCOORD0;
                float3 normal 	: NORMAL;

                float4 color : COLOR;
            };

            struct v2f
            {
                //float alpha : TEXCOORD5;
				//float3 wpos  : TEXCOORD6;
                float2 uv    : TEXCOORD0;
                float4 color : TEXCOORD1;

                float4 vertex : SV_POSITION;
               // float4 test : TEXCOORD4;

                float  F : TEXCOORD2;


                float3 tint: TEXCOORD3;
				float3 add  : TEXCOORD4;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;


            float4 _TintR;
            float4 _TintG;
            float4 _TintB;

            float4 _ColorAddR;
            float4 _ColorAddG;
            float4 _ColorAddB;



			//float g_fAabbMin;
            //float g_fAabbMax;
            //float g_bDebugAABB;
           
            //float g_fDissolveAlpha;
            //float g_fDissolveRange;
 
            float3 g_vOrigin;

            //float g_fFogFar;
			
            //float g_fAlphaReintegrationEnabled;

            float freshnel(float3 N, float3 V)
            {
                float VdotN = dot(N, V);
                float k = saturate(1 - VdotN);
                return k * k * k;
            }

            v2f vert (appdata v)
            {
                v2f o;

                float x = sin(
                    _Time.x * 60 * (1 + v.color.g) 
                    + v.vertex.x
                    + v.color.b 
                    + v.color.r * 16
                );

                float3 offset = normalize(v.vertex.xyz) * x*0.0005;

                v.vertex.xyz += offset;


                o.vertex    = UnityObjectToClipPos(v.vertex);
                o.uv        = TRANSFORM_TEX(v.uv, _MainTex);
                o.color     = v.color;


               float3 worldPos  = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1)).xyz;
				//o.wpos = worldPos;
				
                //float p0y = worldPos.y; //  g_vOrigin.y
               
                //float h 	= saturate((p0y - g_fAabbMin) / (g_fAabbMax - g_fAabbMin)); // linstep (inverse lerp)
                //h = 1 - h;				
                //float alpha =  saturate( (g_fDissolveAlpha * 2.0 - h) / g_fDissolveRange);
               
               // if(g_bDebugAABB > 0.5)
               // {
                //    alpha = h;
               // }
				
				
               // o.alpha = alpha;
				
                float3 center = g_vOrigin.xyz;
                float3 Target = normalize(worldPos - center);
                
                float3 V = - normalize(worldPos - _WorldSpaceCameraPos);

                float TdotV     = dot(Target, V) * 0.5 + 0.5;
                float intensity = TdotV * TdotV * 8.1 + 0.9;
				
                o.color.a  = max(0, intensity);
			    // o.color.a *= pow(saturate(1 - length(worldPos - _WorldSpaceCameraPos) / g_fFogFar) , 4 );
			

                float4 vert_colors = o.color;

                float F = max(0, freshnel(normalize(v.normal), V));
                F *= 0.2 - 0.1 * vert_colors.b;

                float4 tint = _TintR;
                tint = lerp(tint, _TintG, vert_colors.g);
                tint = lerp(tint, _TintB, vert_colors.b);

                float3 color_add = _ColorAddR;
                color_add = lerp(color_add, _ColorAddG, vert_colors.g);
                color_add = lerp(color_add, _ColorAddB, vert_colors.b);

                o.tint = tint.rgb * tint.a * 0.5;
                o.add  = color_add * 0.5 + F;

                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 masks = tex2D(_MainTex, i.uv);
                float3 C = masks.r * i.tint + i.add;
                C *= i.color.a;
				
				//C *= lerp(1, i.alpha, saturate(g_fAlphaReintegrationEnabled));

			
				/*
                if(g_bDebugAABB > 0.5 && g_fAlphaReintegrationEnabled > 0.25)
                {
                    C = lerp(float3(1,0,0), float3(0,1,0), A); 
                    A = 1;
                }*/
                return float4(C, 1);
            }
            ENDCG
        }
    }
}
