
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// 处理灯光， 在相机绘制几何体之前进行调用
/// </summary>
public class Lighting
{
    ScriptableRenderContext context;
    static int dirLightColorId = Shader.PropertyToID("_DirctionalLightColor");
    static int dirLightDirctionalId = Shader.PropertyToID("_DirctionalLightDirctional");

    const string bufferName = "Lighting";

    CommandBuffer buffer = new CommandBuffer
    {
        name = bufferName
    };


    /// <summary>
    ///  用来灯光
    /// </summary>
    public void Setup(ScriptableRenderContext context)
    {

        buffer.BeginSample(bufferName);
        SetupDirectionalLight();
        buffer.EndSample(bufferName);
        context.ExecuteCommandBuffer(buffer);
        buffer.Clear();
    }

    void SetupDirectionalLight()
    {
        // 获取太阳光
        var light = RenderSettings.sun;
        // 设置光的颜色和强度
        buffer.SetGlobalVector(dirLightColorId, light.color.linear * light.intensity);
        // 设置光的方向
        buffer.SetGlobalVector(dirLightDirctionalId, -light.transform.forward);
    }



}
