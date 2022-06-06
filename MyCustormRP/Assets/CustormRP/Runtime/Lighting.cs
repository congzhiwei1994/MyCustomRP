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
    private Shadow shadow = new Shadow();

    /// <summary>
    /// 用来设置灯光
    /// </summary>
    /// <param name="context">上下文</param>
    /// <param name="cullingResults">相机剔除后的数据</param>
    public void Setup(ScriptableRenderContext context, CullingResults cullingResults,
        CustormShadowSettings shadowSettings)
    {
        this.cullingResults = cullingResults;
        buffer.BeginSample(bufferName);
        SetupLights();
        // 初始化并且传递阴影数据
        shadow.Setup(context, cullingResults, shadowSettings);
        // 渲染阴影
        shadow.Render();
        buffer.EndSample(bufferName);
        context.ExecuteCommandBuffer(buffer);
        buffer.Clear();
    }

    /// <summary>
    /// 此方法目的是将多个平行光数据传递到GPU
    /// </summary>
    void SetupLights()
    {
        int dirLightCount = 0;
        // 得到所有的可见光数据
        NativeArray<VisibleLight> visibleLights = cullingResults.visibleLights;
        //遍历所有方向光
        for (int i = 0; i < visibleLights.Length; i++)
        {
            var visibleLight = visibleLights[i];
            // 判断是不是平行光，如果是平行光才进行数据存储
            if (visibleLight.lightType == LightType.Directional)
            {
                // visibleLights 结构体很大，改为传递引用而不是传递值，这样不会生成副本
                SetupDirectionalLight(dirLightCount++, ref visibleLight);

                // 当超过灯光数量限制的时候就终止循环
                if (dirLightCount >= maxDirLightCount)
                {
                    break;
                }
            }
        }

        buffer.SetGlobalInt(dirLightCountId, dirLightCount);
        buffer.SetGlobalVectorArray(dirLightDirctionalId, dirLightDirections);
        buffer.SetGlobalVectorArray(dirLightColorId, dirLightColors);
    }

    /// <summary> 
    /// 此方法将可见的方向光颜色和方向储存到数组
    /// </summary>
    /// <param name="index"> 光源索引 </param>
    /// <param name="visibleLight"> 可见光 </param>
    void SetupDirectionalLight(int index, ref VisibleLight visibleLight)
    {
        // 存储光源的最终颜色
        dirLightColors[index] = visibleLight.finalColor;
        // 存储光源的方向， visibleLight.localToWorldMatrix 此矩阵的第三列即为光源的前向向量， 要取反
        dirLightDirections[index] = -visibleLight.localToWorldMatrix.GetColumn(2);
        shadow.ReserveDirectionalShadows(visibleLight.light, index);
    }

    public void CleanUp()
    {
        shadow.CleanUp();
    }
}