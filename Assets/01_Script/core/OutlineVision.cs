using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 렌더링 레이어 0번은 건들지말것
/// 레이어 1: 아웃라인
/// </summary>

public class OutlineVision : MonoBehaviour
{
    private List<MeshRenderer> renderers = new List<MeshRenderer>();

    public float VisionTime = 15;

    private void Awake()
    {
        // 하위의 모든 MeshRenderer 자동 수집
        renderers.AddRange(GetComponentsInChildren<MeshRenderer>());
    }

    private void Start()
    {
        // 아웃라인 다 해제
        DisableRenderingLayer(1);
    }

    private void EnableRenderingLayer(int layerIndex)
    {
        uint mask = 1u << layerIndex;

        // 삭제된 자식은 리스트에서 제거
        // 역순으로 도는 이유는 RemoveAt 중 인덱스 꼬임 방지
        for (int i = renderers.Count - 1; i >= 0; i--)
        {
            if (renderers[i] == null)
            {
                renderers.RemoveAt(i);
                continue;
            }

            renderers[i].renderingLayerMask |= mask;
        }
    }

    private void DisableRenderingLayer(int layerIndex)
    {
        uint mask = ~(1u << layerIndex);

        // 삭제된 자식은 리스트에서 제거
        // 역순으로 도는 이유는 RemoveAt 중 인덱스 꼬임 방지
        for (int i = renderers.Count - 1; i >= 0; i--)
        {
            if (renderers[i] == null)
            {
                renderers.RemoveAt(i);
                continue;
            }

            renderers[i].renderingLayerMask &= mask;
        }
    }

    IEnumerator EnableOutlineCoroutine()
    {
        EnableRenderingLayer(1);
        yield return new WaitForSeconds(VisionTime);
        DisableRenderingLayer(1);
    }

    public void EnableOutline()
    {
        StopAllCoroutines();
        StartCoroutine(EnableOutlineCoroutine());
    }

    public void DisableOutline()
    {
        StopAllCoroutines();
        DisableRenderingLayer(1);
    }
}