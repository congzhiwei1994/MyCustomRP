
#ifndef CUSTORM_UNLIT_PASS_INCLUDE
    #define CUSTORM_UNLIT_PASS_INCLUDE
    
    #include "ShaderLibrary/Common.hlsl"

    // 将材质属性定义在名字为 UnityPerMaterial 的常量缓冲区中
    CBUFFER_START(UnityPerMaterial)
        half4 _BaseColor;
    CBUFFER_END


    struct appdata
    {
        float4 vertex : POSITION;

    };

    struct v2f
    {
        float4 posCS : SV_POSITION;
    };

    v2f UnlitPassVertex (appdata v)
    {
        v2f o;
        float3 posWS = TransformObjectToWorld(v.vertex.xyz);
        o.posCS = TransformWorldToHClip(posWS);
        return o;
    }

    fixed4 UnlitPassFragment (v2f i) : SV_Target
    {
        return _BaseColor;
    }
#endif