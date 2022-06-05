using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// 阴影
/// </summary>
public class Shadow
{
    ScriptableRenderContext context;

    /// <summary>
    /// 定义一个字段来限制渲染管线可以投影的平行光数量
    /// </summary>
    private const int maxShadowDirectionalLightCount = 1;

    // 储存相机剔除后的结果
    CullingResults cullingResults;
    private CustormShadowSettings shadowSettings;
    const string bufferName = "Shadows";

    CommandBuffer buffer = new CommandBuffer
    {
        name = bufferName
    };

    struct ShadowDirectionalLight
    {
        public int visibleLightIndex;
    }

    /// <summary>
    /// 存储可投影阴影的可见光源的索引
    /// </summary>
    private ShadowDirectionalLight[] shadowDirctionalights = new ShadowDirectionalLight[maxShadowDirectionalLightCount];

    /// <summary>
    /// 存储可投射阴影的可见光的数量
    /// </summary>
    private int shadowDirectionalLightCount;

    public void Setup(ScriptableRenderContext context, CullingResults cullingResults,
        CustormShadowSettings shadowSettings)
    {
        this.context = context;
        this.cullingResults = cullingResults;
        this.shadowSettings = shadowSettings;
        shadowDirectionalLightCount = 0;
    }

    /// <summary>
    /// 存储可投影可见光的阴影数据，目的是在阴影图集中为该光源的阴影贴图保留空间，并存储渲染它们所需要的信息
    /// </summary>
    public void ReserveDirectionalShadows(Light light, int visibleLightIndex)
    {
        // 可投射阴影的光源数量小于或者等于可投影阴影的最大数量
        // 灯光必须开启阴影并且强度必须大于零
        // 判断是否在阴影最大投射距离内，有被当前光源影响且需要投影的物体存在，否则就没必要渲染阴影贴图了
        if (shadowDirectionalLightCount < maxShadowDirectionalLightCount && light.shadows != LightShadows.None &&
            light.shadowStrength > 0.0f && this.cullingResults.GetShadowCasterBounds(visibleLightIndex, out Bounds b))
        {
            // shadowDirctionalights[shadowDirectionalLightCount++] = new ShadowDirectionalLight
            // {
            //     visibleLightIndex = visibleLightIndex
            // };
        }
    }

    private void ExecuteBuffer()
    {
        context.ExecuteCommandBuffer(buffer);
        buffer.Clear();
    }
}