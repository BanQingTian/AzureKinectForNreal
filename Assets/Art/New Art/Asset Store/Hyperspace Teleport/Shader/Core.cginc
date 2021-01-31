#ifndef HT_CORE_INCLUDED
#define HT_CORE_INCLUDED

#include "UnityCG.cginc"

sampler2D _MainTex, _NoiseTex;
float _Clip, _Halo, _Metallic, _Glossiness;
float4 _HaloColor;

float HTClipFrag (float3 wldpos, float2 uv)
{
#ifdef HT_DIR_X
	float dt = wldpos.x - _Clip;
#endif
#ifdef HT_DIR_Y
	float dt = wldpos.y - _Clip;
#endif
#ifdef HT_DIR_Z
	float dt = wldpos.z - _Clip;
#endif

#ifdef HT_NOISE
	float3 ns3 = tex2D(_NoiseTex, uv).rgb;
	float ns = (ns3.x + ns3.y + ns3.z) / 3.0;
	dt += ns;
#endif

	float s = step(abs(dt), _Halo);
	float f = max(dt, 0.0);
	
#ifdef HT_BACKWARD
	clip(f - 0.01);
#endif
#ifdef HT_FORWARD
	clip(0.01 - f);
#endif
	return s;
}

#endif