
// 公共方法库
#ifndef CUSTORM_UNITY_COMMON_INCLUDE
    #define CUSTORM_UNITY_COMMON_INCLUDE
    
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
    #include "UnityInput.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
    
    #define UNITY_MATRIX_M unity_ObjectToWorld 
    #define UNITY_MATRIX_I_M unity_WorldToObject
    #define UNITY_MATRIX_V unity_MatrixV
    #define UNITY_MATRIX_VP unity_MatrixVP 
    #define UNITY_MATRIX_P glstate_matrix_projection

    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"

#endif