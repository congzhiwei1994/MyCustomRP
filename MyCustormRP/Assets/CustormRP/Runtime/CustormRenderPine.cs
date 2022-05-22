
using UnityEngine;
using UnityEngine.Rendering;

public class CustormRenderPine : RenderPipeline
{
    CameraRenderer renderer = new CameraRenderer();

    /// <summary>
    /// SPR入口，每一帧都会调用此方法进行渲染
    /// </summary>
    /// ScriptableRenderContext: SRP用于最底层渲染的接口之一，还有一个是CommandBuffer
    /// Camera[]： 相机数组，存储了参与这一帧渲染的所有相机对象
    protected override void Render(ScriptableRenderContext context, Camera[] cameras)
    {
        // 遍历所有相机进行渲染
        foreach (var camera in cameras)
        {
            renderer.Render(context, camera);
        }
    }
}
