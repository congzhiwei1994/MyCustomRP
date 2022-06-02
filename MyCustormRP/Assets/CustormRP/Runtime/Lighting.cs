using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// 处理灯光， 在相机绘制几何体之前进行调用
/// </summary>
public class Lighting
{
    ScriptableRenderContext context;

    // 定义最大可见平行光数量的字段
    private const int maxDirLightCount = 4;

    // 声明Shader属性ID标识，用于将灯光发送到GPU的对应属性中
    static int dirLightCountId = Shader.PropertyToID("_DirectionalLightCount");
    static int dirLightColorId = Shader.PropertyToID("_DirectionalLightColors");
    static int dirLightDirctionalId = Shader.PropertyToID("_DirectionalLightDirections");

    // 储存可见光的颜色和方向
    private static Vector4[] dirLightColors = new Vector4[maxDirLightCount];
    private static Vector4[] dirLightDirections = new Vector4[maxDirLightCount];

    const string bufferName = "Lighting";

    CommandBuffer buffer = new CommandBuffer
    {
        name = bufferName
    };

    // 储存相机剔除后的结果
    CullingResults cullingResults;

    /// <summary>
    /// 用来设置灯光
    /// </summary>
    /// <param name="context">上下文</param>
    /// <param name="cullingResults">相机剔除后的数据</param>
    public void Setup(ScriptableRenderContext context, CullingResults cullingResults)
    {
        this.cullingResults = cullingResults;
        buffer.BeginSample(bufferName);
        SetupLights();
        buffer.EndSample(bufferName);
        context.ExecuteCommandBuffer(buffer);
        buffer.Clear();
    }

    /// <summary>
    /// 将可见光的颜色和方向储存到数组
    /// </summary>
    /// <param name="index"> 光源索引 </param>
    /// <param name="visibleLight">可见光 </param>
    void SetupDirectionalLight(int index, VisibleLight visibleLight)
    {
        // 获取光源的最终颜色
        dirLightColors[index] = visibleLight.finalColor;
        // 获取光源的方向， visibleLight.localToWorldMatrix 此矩阵的第三列即为光源的前向向量， 要取反
        dirLightDirections[index] = -visibleLight.localToWorldMatrix.GetColumn(2);
    }

    // 设置和发送多个光源的数据
    void SetupLights()
    {
        // 得到所有的可见光数据
        NativeArray<VisibleLight> visibleLights = cullingResults.visibleLights;
    }
}