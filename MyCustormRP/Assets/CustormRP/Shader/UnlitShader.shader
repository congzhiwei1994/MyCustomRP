Shader "CustormRP/Unlit Shader"
{
    Properties
    {
        // _MainTex ("Texture", 2D) = "white" {}
        _BaseColor("Base Color",color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma multi_compile_instancing

            
            #pragma vertex UnlitPassVertex 
            #pragma fragment UnlitPassFragment 
            
            #include "UnlitPass.hlsl"

            
            ENDCG
        }
    }
}
