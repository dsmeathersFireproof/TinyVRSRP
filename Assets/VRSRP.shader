//
// VRSRP.shader
// Copyright (c) 2020 Fireproof Studios, All Rights Reserved
//

Shader "VRSRP"
{
    Properties
    {
    }    

    SubShader
    {
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"

            struct appdata
            {
                UNITY_VERTEX_INPUT_INSTANCE_ID
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);              
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            struct fragOut
            {
                float4 col : SV_TARGET;
            };

            fragOut frag (v2f i)
            {
                UNITY_SETUP_INSTANCE_ID(i);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
                
                half4 col = 1;
                fragOut output;
                output.col = col;
                return output;
            }
            ENDCG
        }
    }
}