
#ifndef CUSTORM_UNLIT_PASS_INCLUDE
    #define CUSTORM_UNLIT_PASS_INCLUDE
    
    #include "ShaderLibrary/Common.hlsl"

    // 将材质属性定义在名字为 UnityPerMaterial 的常量缓冲区中
    // CBUFFER_START(UnityPerMaterial)
        //     half4 _BaseColor;
    // CBUFFER_END

    UNITY_INSTANCING_BUFFER_START(UnityPerMaterial)

    UNITY_DEFINE_INSTANCED_PROP(half4,_BaseColor)

    UNITY_INSTANCING_BUFFER_END(UnityPerMaterial)


    struct appdata
    {
        float4 vertex : POSITION;

        UNITY_VERTEX_INPUT_INSTANCE_ID
        // UNITY_VERTEX_OUTPUT_STEREO
    };

    struct v2f
    {
        float4 posCS : SV_POSITION;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    v2f UnlitPassVertex (appdata v)
    {
        v2f o  = (v2f)0;
        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_TRANSFER_INSTANCE_ID(v, o);
        // UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
        
        float3 posWS = TransformObjectToWorld(v.vertex.xyz);
        o.posCS = TransformWorldToHClip(posWS);
        return o;
    }

    fixed4 UnlitPassFragment (v2f i) : SV_Target
    {
        UNITY_SETUP_INSTANCE_ID(i);
        // UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        half4 c = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial,_BaseColor);
        return c;
    }
#endif