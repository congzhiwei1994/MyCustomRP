using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "Rendering/CreateRenderPipleLine")]
public class CustormRenderPineAsset : RenderPipelineAsset
{
    [SerializeField]
    public bool useDynamicBatching = false;
    [SerializeField]
    public bool useGPUInstancing = false;

    [SerializeField]
    public bool useSRPBatcher = false;


    protected override RenderPipeline CreatePipeline()
    {
        var custormRP = new CustormRenderPine(useDynamicBatching, useGPUInstancing, useSRPBatcher);
        return custormRP;
    }
}
