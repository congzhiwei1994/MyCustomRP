using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// 阴影设置
/// </summary>
[System.Serializable]
public class CustormShadowSettings 
{
    // 阴影的最大距离
    [Min(0.0f)] public float maxDistance = 100f;

    // 阴影贴图大小
    public enum TextureSize
    {
        _256 = 256,
        _512 = 512,
        _1024 = 1024,
        _2048 = 2048,
        _4096 = 4096
    }

    // 方向光的阴影配置
    [System.Serializable]
    public struct Directional
    {
        public TextureSize atlasSize;
    }

    // 默认尺寸为1024
    public Directional directional = new Directional
    {
        atlasSize = TextureSize._1024
    };
}