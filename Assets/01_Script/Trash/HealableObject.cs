using System.Collections;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// Heal 포션 전용 컴포넌트.
/// 시든 식물 오브젝트에 부착합니다.
/// 
/// [인스펙터 설정]
///   healedPrefab   : 생기 있는 식물 프리팹 (같은 위치에 fade in 됨)
///   fadeDuration   : 시든 식물 fade out 시간
///   growDuration   : 새 식물 fade in 시간
/// 
/// [동작 흐름]
///   TrashObject.CleanUp(Heal) → HealableObject.Heal()
///   → 시든 오브젝트 fade out (알파 0)
///   → 같은 자리에 healedPrefab 스폰, 알파 0 → 1 fade in
///   → 시든 오브젝트 Destroy
/// </summary>
[RequireComponent(typeof(TrashObject))]
public class HealableObject : MonoBehaviour
{
    [Header("치유 설정")]
    [Tooltip("생기를 되찾은 식물 프리팹 (평소엔 알파 0으로 숨겨져 있어야 합니다)")]
    [SerializeField] private GameObject healedPrefab;

    [Tooltip("healedPrefab이 없을 때 대신 활성화할 씬 내 오브젝트 (선택)")]
    [SerializeField] private GameObject healedSceneObject;

    [SerializeField] private float fadeDuration = 0.8f;   // 시든 것 사라지는 시간
    [SerializeField] private float growDuration  = 1.2f;  // 새 것 나타나는 시간
    [SerializeField] private float crossFadeDelay = 0.3f; // fade out 시작 후 fade in 시작까지 딜레이

    // ── URP Lit 셰이더 프로퍼티 ──────────────────────────────
    private static readonly int PropSurface   = Shader.PropertyToID("_Surface");
    private static readonly int PropBlend     = Shader.PropertyToID("_Blend");
    private static readonly int PropSrcBlend  = Shader.PropertyToID("_SrcBlend");
    private static readonly int PropDstBlend  = Shader.PropertyToID("_DstBlend");
    private static readonly int PropZWrite    = Shader.PropertyToID("_ZWrite");
    private static readonly int PropBaseColor = Shader.PropertyToID("_BaseColor");

    // ── Public API ────────────────────────────────────────────

    /// <summary>TrashObject.HealRoutine()에서 호출됩니다.</summary>
    public void Heal()
    {
        StartCoroutine(HealSequence());
    }

    // ── 시퀀스 ────────────────────────────────────────────────

    private IEnumerator HealSequence()
    {
        // 1. 시든 오브젝트 Transparent 전환 → fade out
        var renderers = GetComponentsInChildren<Renderer>();
        EnableTransparency(renderers);

        bool fadeOutDone = false;
        FadeRenderers(renderers, 0f, fadeDuration, () => fadeOutDone = true);

        // 2. crossFadeDelay 후 새 식물 fade in 시작 (크로스페이드)
        yield return new WaitForSeconds(crossFadeDelay);

        SpawnAndFadeInHealed();

        // 3. fade out 완료 기다린 뒤 시든 오브젝트 제거
        yield return new WaitUntil(() => fadeOutDone);
        Destroy(gameObject);
    }

    private void SpawnAndFadeInHealed()
    {
        GameObject target = null;

        if (healedPrefab != null)
        {
            // 프리팹을 같은 위치/회전/스케일로 스폰
            target = Instantiate(healedPrefab, transform.position, transform.rotation);
            target.transform.localScale = transform.lossyScale;
        }
        else if (healedSceneObject != null)
        {
            // 씬에 이미 있던 오브젝트를 활성화
            target = healedSceneObject;
            target.SetActive(true);
        }

        if (target == null) return;

        // 알파 0으로 초기화 후 fade in
        var rends = target.GetComponentsInChildren<Renderer>();
        EnableTransparency(rends);
        SetAlpha(rends, 0f);
        FadeRenderers(rends, 1f, growDuration, null);
    }

    // ── 유틸 ──────────────────────────────────────────────────

    private void FadeRenderers(Renderer[] rends, float targetAlpha, float duration, System.Action onDone)
    {
        int total = 0;
        foreach (var r in rends) total += r.materials.Length;
        if (total == 0) { onDone?.Invoke(); return; }

        int done = 0;
        foreach (var r in rends)
        {
            foreach (var mat in r.materials)
            {
                Color c = mat.GetColor(PropBaseColor);
                mat.DOColor(new Color(c.r, c.g, c.b, targetAlpha), PropBaseColor, duration)
                   .SetEase(targetAlpha == 0f ? Ease.InQuad : Ease.OutQuad)
                   .OnComplete(() =>
                   {
                       done++;
                       if (done >= total) onDone?.Invoke();
                   });
            }
        }
    }

    private void SetAlpha(Renderer[] rends, float alpha)
    {
        foreach (var r in rends)
            foreach (var mat in r.materials)
            {
                Color c = mat.GetColor(PropBaseColor);
                mat.SetColor(PropBaseColor, new Color(c.r, c.g, c.b, alpha));
            }
    }

    private void EnableTransparency(Renderer[] rends)
    {
        foreach (var r in rends)
            foreach (var mat in r.materials)
            {
                mat.SetFloat(PropSurface,  1f);
                mat.SetFloat(PropBlend,    0f);
                mat.SetFloat(PropSrcBlend, 5f);
                mat.SetFloat(PropDstBlend, 10f);
                mat.SetFloat(PropZWrite,   0f);
                mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            }
    }
}
