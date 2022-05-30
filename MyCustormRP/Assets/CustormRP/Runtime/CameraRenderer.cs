
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// 处理单个相机的单个渲染
/// </summary>
public partial class CameraRenderer
{
    ScriptableRenderContext context;
    Lighting light = new Lighting();
    Camera camera;

    // 储存相机剔除后的所有视野内可见物体的数据信息
    CullingResults cullingResults;

    static ShaderTagId unlitShaderTagId = new ShaderTagId("SRPDefaultUnlit");

    const string bufferName = "Render Camera";

    CommandBuffer buffer = new CommandBuffer
    {
        name = bufferName
    };


    /// <summary>
    /// 绘制在相机视野内的所有物体
    /// </summary>
    public void Render(ScriptableRenderContext context, Camera camera, bool useDynamicBatching, bool useGPUInstancing)
    {
        this.context = context;
        this.camera = camera;

        // 在Game视图绘制的几何体也会绘制到Scene视图中
        PrepareForWindow();

        if (!Cull())
        {
            return;
        }
        // 设置缓冲区的名字
        PrepareBuffer();
        Step();
        // 绘制几何体之前设置灯光
        light.Setup(context);
        DrawVisibleGeometry(useDynamicBatching, useGPUInstancing);
        DrawUnsupportedShaders();
        DrawGizmos();
        Submit();
    }

    /// <summary>
    ///  用来设置相机的矩阵和属性
    /// </summary>
    void Step()
    {
        context.SetupCameraProperties(camera);
        // 获得当前相机的clearFlags
        var clearFlags = camera.clearFlags;
        // 为了保证下一帧绘制的图像正确，通常需要清除渲染目标，清除旧的数据
        buffer.ClearRenderTarget(clearFlags <= CameraClearFlags.Depth, clearFlags == CameraClearFlags.Color, clearFlags == CameraClearFlags.Color ? camera.backgroundColor.linear : Color.clear);
        // 开启采样
        buffer.BeginSample(SampleName);
        ExecuteBuffer();
    }

    /// <summary>
    /// 绘制可见物体
    /// </summary>
    void DrawVisibleGeometry(bool useDynamicBatching, bool useGPUInstancing)
    {
        // 设置绘制顺序和指定渲染相机
        var sortingSettings = new SortingSettings(camera)
        {
            criteria = SortingCriteria.CommonOpaque
        };

        // 设置渲染的shader以及排序模式
        DrawingSettings drawingSettings = new DrawingSettings(unlitShaderTagId, sortingSettings)
        {
            // 设置渲染时批处理的使用状态
            enableDynamicBatching = useDynamicBatching,
            enableInstancing = useGPUInstancing
        };

        // 只绘制RenderQueue为Opaque的不透明物体
        FilteringSettings filteringSettings = new FilteringSettings(RenderQueueRange.opaque);
        // 图像绘制， 绘制不透明物体
        context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);
        // 绘制天空盒
        context.DrawSkybox(camera);

        sortingSettings.criteria = SortingCriteria.CommonTransparent;
        drawingSettings.sortingSettings = sortingSettings;

        // 只绘制RenderQueue为Transparent的透明物体
        filteringSettings.renderQueueRange = RenderQueueRange.transparent;
        // 绘制透明物体
        context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);
    }

    /// <summary>
    /// 提交缓冲区渲染命令,
    /// </summary>
    void Submit()
    {
        // 结束采样
        buffer.EndSample(SampleName);
        ExecuteBuffer();
        // 当执行  context.Submit() 提交缓冲区渲染明然命令才进行这一帧的渲染
        context.Submit();
    }

    /// <summary>
    ///  执行命令缓冲区
    /// </summary>
    void ExecuteBuffer()
    {
        // 执行命令缓冲区，这个操作会从命令缓冲区复制命令但不会清除缓冲区，如果要重用 buffer,执行完该命令后调用clear()清除。
        context.ExecuteCommandBuffer(buffer);
        buffer.Clear();
    }

    /// <summary>
    /// 剔除, 相机渲染Render()的最开始调用剔除操作。
    /// </summary>
    bool Cull()
    {
        ScriptableCullingParameters p;
        // 得到需要进行剔除检查的所有物体
        if (camera.TryGetCullingParameters(out p))
        {
            // 正式剔除
            cullingResults = context.Cull(ref p);
            return true;
        }
        return false;
    }

}
