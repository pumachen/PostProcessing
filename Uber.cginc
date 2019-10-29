#ifndef POST_PROCESS_UBER_INCLUDED
#define POST_PROCESS_UBER_INCLUDED

#include "UnityCG.cginc"

    sampler2D _MainTex;

#ifdef DEPTH_TEXTURE
    sampler2D _CameraDepthTexture;
#elif DEPTH_NORMAL_TEXTURE
    sampler2D _CameraDepthNormalTexture;
#elif MOTION_VECTOR
    sampler2D _CameraMotionVectorTexture;
#endif

    fixed4 frag_uber(v2f_img i) : SV_Target
    {
        fixed4 col = tex2D(_MainTex, i.uv);
        #ifdef DEPTH_TEXTURE
            float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
        #elif DEPTH_NORMAL_TEXTURE
            float depth;
            float3 normal;
            DecodeDepthNormal(SAMPLE_DEPTH_NORMAL_TEXTURE(_CameraDepthNormalTexture, i.uv), depth, normal);
        #endif
		return col;
    }
#endif //POST_PROCESS_UBER_INCLUDED