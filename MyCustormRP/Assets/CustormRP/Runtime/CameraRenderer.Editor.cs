
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using UnityEngine.Profiling;

/// <summary>
/// 编辑器模式下
/// </summary>
partial class CameraRenderer
{

    partial void DrawUnsupportedShaders();
    partial void DrawGizmos();
    partial void PrepareForWindow();
    partial void PrepareBuffer();
#if UNITY_EDITOR
    string SampleName { get; set; }

    // SRP不支持的Shader LightMode 标签类型
    static ShaderTagId[] legacyShaderTagIds =
    {
        new ShaderTagId("Always"),
        new ShaderTagId("ForWardBase"),
        new ShaderTagId("PrepassBase"),
        new ShaderTagId("Vertex"),
        new ShaderTagId("VertexLMRGBM"),
        new ShaderTagId("VertexLM"),
    };

    static Material errorMaterial;

    /// <summary>
    /// 绘制SRP不支持的Shader类型
    /// </summary>
    partial void DrawUnsupportedShaders()
    {
        if (errorMaterial == null)
        {
            errorMaterial = new Material(Shader.Find("Hidden/InternalErrorShader"));
        }
        var drawingSettings = new DrawingSettings(legacyShaderTagIds[0], new SortingSettings(camera))
        {
            overrideMaterial = errorMaterial
        };

        for (var i = 0; i < legacyShaderTagIds.Length; i++)
        {
            // 遍历数组逐个设置Shader的PassName
            drawingSettings.SetShaderPassName(i, legacyShaderTagIds[i]);

        }
        // 使用默认的设置即可，反正画出来的都是不支持的
        var filteringSettings = FilteringSettings.defaultValue;
        // 绘制不支持的ShaderTag类型的物体
        context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);
    }

    // 绘制Gizmos
    partial void DrawGizmos()
    {
        if (Handles.ShouldRenderGizmos())
        {
            context.DrawGizmos(camera, GizmoSubset.PreImageEffects);
            context.DrawGizmos(camera, GizmoSubset.PostImageEffects);
        }
    }

    // 在Game视图绘制的几何体也会绘制到Scene视图
    partial void PrepareForWindow()
    {
        if (camera.cameraType == CameraType.SceneView)
        {
            // 在Scene视图绘制UI
            ScriptableRenderContext.EmitWorldGeometryForSceneView(camera);
        }
    }

    partial void PrepareBuffer()
    {
        // 设置一下只有在编辑器模式下才分配内存
        Profiler.BeginSample("Editor Only");
        buffer.name = SampleName = camera.name;
        Profiler.EndSample();
    }
#else
// 非编辑器平台下为常量字符串
   const string SampleName = bufferName;
#endif
}
