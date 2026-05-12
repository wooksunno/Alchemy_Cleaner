using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 렌더링 레이어 0번은 건들지말것
/// 레이어 1: 아웃라인
/// </summary>

public class RenderLayerGroup : MonoBehaviour
{
    private List<MeshRenderer> renderers = new List<MeshRenderer>();

    private void Awake()
    {
        // 하위의 모든 MeshRenderer 자동 수집
        renderers.AddRange(GetComponentsInChildren<MeshRenderer>());
    }

    private void Start()
    {
        DisableRenderingLayer(1);
    }

    // 특정 렌더링 레이어 ON
    public void EnableRenderingLayer(int layerIndex)
    {
        uint mask = 1u << layerIndex;

        foreach (var r in renderers)
        {
            r.renderingLayerMask |= mask;
        }
    }

    // 특정 렌더링 레이어 OFF
    public void DisableRenderingLayer(int layerIndex)
    {
        uint mask = ~(1u << layerIndex);

        foreach (var r in renderers)
        {
            r.renderingLayerMask &= mask;
        }
    }

    // 특정 레이어 토글
    public void ToggleRenderingLayer(int layerIndex)
    {
        uint mask = 1u << layerIndex;

        foreach (var r in renderers)
        {
            r.renderingLayerMask ^= mask;
        }
    }

    // 체크
    public bool HasLayer(MeshRenderer r, int layerIndex)
    {
        uint mask = 1u << layerIndex;
        return (r.renderingLayerMask & mask) != 0;
    }
}