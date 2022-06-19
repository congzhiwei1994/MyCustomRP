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
    private const int maxShadowDirectionalLightCount = 4;

    CullingResults cullingResults;
    private CustormShadowSettings shadowSettings;
    const string bufferName = "Shadows";

    CommandBuffer buffer = new CommandBuffer
    {
        name = bufferName
    };

    /// <summary>
    /// 阴影图集的Shader标识ID
    /// </summary>
    static int dirShadowAtlasId = Shader.PropertyToID("_DirectionalShadowAtlas");

    struct ShadowDirectionalLight
    {
        public int visibleLightIndex;
    }

    /// <summary>
    /// 存储可投影的可见光索引
    /// </summary>
    private ShadowDirectionalLight[] ShadowDirectionalLights =
        new ShadowDirectionalLight[maxShadowDirectionalLightCount];

    /// <summary>
    /// 存储可投射阴影的可见光的数量
    /// </summary>
    int shadowDirectionalLightCount;

    public void Setup(ScriptableRenderContext context, CullingResults cullingResults,
        CustormShadowSettings shadowSettings)
    {
        this.context = context;
        this.cullingResults = cullingResults;
        this.shadowSettings = shadowSettings;
        shadowDirectionalLightCount = 0;
    }

    /// <summary>
    /// 渲染阴影
    /// </summary>
    public void Render()
    {
        // 当可投影的平行光数量大于0的时候才会渲染平行光的阴影
        if (shadowDirectionalLightCount > 0)
        {
            RenderDirectionalShadow();
        }
    }
    
    /// <summary>
    /// 存储可投影可见光的阴影数据，目的是在阴影图集中为该光源的阴影贴图保留空间，并存储渲染它们所需要的信息
    /// </summary>
    /// <param name="light">可见光</param>
    /// <param name="visibleLightIndex">可见光索引</param>
    public void ReserveDirectionalShadows(Light light, int visibleLightIndex)
    {
        // 可投射阴影的光源数量小于或者等于可投影阴影的最大数量
        // 灯光必须开启阴影并且强度必须大于零
        // 判断是否在阴影最大投射距离内，有被当前光源影响且需要投影的物体存在，否则就没必要渲染阴影贴图了
        if (shadowDirectionalLightCount < maxShadowDirectionalLightCount && light.shadows != LightShadows.None &&
            light.shadowStrength > 0.0f && this.cullingResults.GetShadowCasterBounds(visibleLightIndex, out Bounds b))
        {
            ShadowDirectionalLights[shadowDirectionalLightCount++] = new ShadowDirectionalLight
            {
                visibleLightIndex = visibleLightIndex
            };
        }
    }
    

    /// <summary>
    /// 渲染平行光的阴影
    /// </summary>
    void RenderDirectionalShadow()
    {
        int atlasSize = (int)shadowSettings.directional.atlasSize;
        // 为阴影图集申请RT
        buffer.GetTemporaryRT(dirShadowAtlasId, atlasSize, atlasSize, 32, FilterMode.Bilinear,
            RenderTextureFormat.Shadowmap);
        // 指定渲染数据存储到渲染纹理而不是帧缓冲
        buffer.SetRenderTarget(dirShadowAtlasId, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
        buffer.ClearRenderTarget(true, false, Color.clear);

        buffer.BeginSample(bufferName);
        ExecuteBuffer();

        // 要分割的图块大小和数量
        int split = shadowDirectionalLightCount <= 1 ? 1 : 2;
        int tileSize = atlasSize / split;

        // 遍历所有方向光进行逐光源的阴影渲染
        for (int i = 0; i < shadowDirectionalLightCount; i++)
        {
            RenderDirectionalShadows(i, split, tileSize);
        }

        buffer.EndSample(bufferName);
        ExecuteBuffer();
    }


    /// <summary>
    /// 单个平行光光阴影
    /// </summary>
    /// <param name="intdex">投射阴影的灯光索引</param>
    /// <param name="tileSize">该光源的阴影贴图在阴影图集中所占图块的大小</param>
    public void RenderDirectionalShadows(int index, int split, int tileSize)
    {
        ShadowDirectionalLight light = ShadowDirectionalLights[index];
        // 创建ShadowDrawingSettings对象，用来创建阴影设置对象
        var shadowSetting = new ShadowDrawingSettings(cullingResults, light.visibleLightIndex);

        cullingResults.ComputeDirectionalShadowMatricesAndCullingPrimitives(light.visibleLightIndex, 0, 1, Vector3.zero,
            tileSize, 0.0f, out Matrix4x4 viewMatrix, out Matrix4x4 projMatrix, out ShadowSplitData splitData);
        // splitData 包含了如何剔除投影对象的信息
        shadowSetting.splitData = splitData;
        
        SetTileViewPort(index,split,tileSize);
        // 设置视图矩阵和投影矩阵
        buffer.SetViewProjectionMatrices(viewMatrix, projMatrix);
        ExecuteBuffer();
        // 渲染阴影投射
        context.DrawShadows(ref shadowSetting);
    }

    /// <summary>
    /// 调整视口渲染单个图块 
    /// </summary>
    /// <param name="index">图块索引</param>
    /// <param name="split">图块数量</param>
    /// <param name="tileSize">图块大小</param>
    void SetTileViewPort(int index, int split, int tileSize)
    {
        var offset = new Vector2(index % split, index / split);
        // 设置渲染视口，拆分成多个图块
        buffer.SetViewport(new Rect(tileSize * offset.x, tileSize * offset.y,tileSize,tileSize));
    }


    public void CleanUp()
    {
        buffer.ReleaseTemporaryRT(dirShadowAtlasId);
        ExecuteBuffer();
    }

    private void ExecuteBuffer()
    {
        context.ExecuteCommandBuffer(buffer);
        buffer.Clear();
    }
}