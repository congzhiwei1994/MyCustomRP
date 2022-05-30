Shader "CustormRP/Unlit Shader"
{
    Properties
    {
        // _MainTex ("Texture", 2D) = "white" {}
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("SrcBlend",float) = 1
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("DstBlend",float) = 1
        _BaseColor("Base Color",color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Blend[_SrcBlend][_DstBlend]
            CGPROGRAM
            #pragma vertex UnlitPassVertex 
            #pragma fragment UnlitPassFragment 

            #include "UnlitPass.hlsl"

            
            ENDCG
        }
    }
}
