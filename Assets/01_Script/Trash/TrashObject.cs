using System.Collections;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// 쓰레기 오브젝트 컴포넌트.
/// PotionEffect → CleanUp() 호출 시 TrashResponseDatabase로 포션 반응 여부를 판정하고,
/// 포션 타입에 따라 다른 청소 연출을 실행합니다.
/// 
/// [포션별 동작]
/// - Heal  : HealableObject.Heal() 위임 → 시든 식물 fade out + 새 식물 fade in
/// - 그 외 : Fire 연출 → 불 파티클 스폰 → burnLifetime 후 페이드아웃 → Destroy
/// </summary>
public class TrashObject : MonoBehaviour
{
    [Header("쓰레기 설정")]
    public TrashType trashType;

    [Header("Fire 이펙트")]
    [Tooltip("불 파티클 프리팹 (ParticleSystem, Stop Action = Destroy 권장)")]
    [SerializeField] private GameObject burnEffectPrefab;
    [Tooltip("불이 켜지고 나서 오브젝트가 완전히 사라질 때까지 총 시간 (초)")]
    [SerializeField] private float burnLifetime    = 1.0f;
    [Tooltip("페이드아웃에 걸리는 시간. burnLifetime 안에 포함됩니다.")]
    [SerializeField] private float burnFadeDuration = 0.5f;

    // ── URP Lit 셰이더 프로퍼티 ID ───────────────────────────
    private static readonly int PropSurface   = Shader.PropertyToID("_Surface");
    private static readonly int PropBlend     = Shader.PropertyToID("_Blend");
    private static readonly int PropSrcBlend  = Shader.PropertyToID("_SrcBlend");
    private static readonly int PropDstBlend  = Shader.PropertyToID("_DstBlend");
    private static readonly int PropZWrite    = Shader.PropertyToID("_ZWrite");
    private static readonly int PropBaseColor = Shader.PropertyToID("_BaseColor");

    private OutlineVision _outlineControl;
    private bool _isCleaning = false;

    private void Awake()
    {
        _outlineControl = GetComponent<OutlineVision>();
    }

    // ── Public API ────────────────────────────────────────────

    /// <summary>
    /// PotionEffect에서 호출됩니다.
    /// 포션 타입에 따라 다른 청소 루틴을 실행합니다.
    /// </summary>
    public void CleanUp(PotionType potionType, TrashResponseDatabase database)
    {
        if (_isCleaning) return;

        if (!database.CanClean(trashType, potionType))
        {
            _outlineControl?.EnableOutline();
            return;
        }

        _isCleaning = true;
        DisableColliders();

        if (potionType == PotionType.Heal)
            StartCoroutine(HealRoutine());
        else
            StartCoroutine(FireRoutine());
    }

    // ── 청소 루틴 ─────────────────────────────────────────────

    /// <summary>
    /// Heal: HealableObject가 있으면 위임, 없으면 단순 페이드아웃.
    /// </summary>
    private IEnumerator HealRoutine()
    {
        var healable = GetComponent<HealableObject>();
        if (healable != null)
        {
            healable.Heal(); // HealableObject가 Destroy까지 담당
            yield break;
        }

        // HealableObject 미부착 시 fallback
        FadeOutAndDestroy(0.8f, null);
        yield break;
    }

    /// <summary>
    /// Fire: 불 파티클 스폰 → (burnLifetime - burnFadeDuration) 대기
    ///       → 페이드아웃 → 불 분리 → Destroy
    /// </summary>
    private IEnumerator FireRoutine()
    {
        // 불 파티클 스폰 (자식으로 붙여 오브젝트와 함께 이동)
        GameObject fireInstance = null;
        if (burnEffectPrefab != null)
        {
            fireInstance = Instantiate(burnEffectPrefab, transform.position, Quaternion.identity);
            fireInstance.transform.SetParent(transform);
        }

        // 불이 타다가 → 페이드 시작 시점까지 대기
        float waitBeforeFade = Mathf.Max(0f, burnLifetime - burnFadeDuration);
        yield return new WaitForSeconds(waitBeforeFade);

        FadeOutAndDestroy(burnFadeDuration, () => DetachAndStopFire(fireInstance));
    }

    // ── 공용 유틸 ─────────────────────────────────────────────

    /// <summary>
    /// 모든 하위 Renderer를 Transparent로 전환 후 알파를 0으로 트위닝.
    /// 완료 시 onBeforeDestroy 콜백 → Destroy.
    /// </summary>
    private void FadeOutAndDestroy(float duration, System.Action onBeforeDestroy)
    {
        var renderers = GetComponentsInChildren<Renderer>();

        // ParticleSystem Renderer 제외
        var validRenderers = new System.Collections.Generic.List<Renderer>();

        foreach (var r in renderers)
        {
            if (r.GetComponent<ParticleSystemRenderer>() != null)
                continue;

            validRenderers.Add(r);
        }

        EnableTransparency(validRenderers.ToArray());

        int total = 0;
        foreach (var r in validRenderers)
            total += r.materials.Length;

        if (total == 0)
        {
            onBeforeDestroy?.Invoke();
            Destroy(gameObject);
            return;
        }

        int done = 0;

        foreach (var r in validRenderers)
        {
            foreach (var mat in r.materials)
            {
                if (!mat.HasProperty(PropBaseColor))
                {
                    done++;

                    if (done >= total)
                    {
                        onBeforeDestroy?.Invoke();
                        Destroy(gameObject);
                    }

                    continue;
                }

                Color c = mat.GetColor(PropBaseColor);

                mat.DOColor(
                    new Color(c.r, c.g, c.b, 0f),
                    PropBaseColor,
                    duration
                )
                .SetEase(Ease.InQuad)
                .OnComplete(() =>
                {
                    done++;

                    if (done >= total)
                    {
                        onBeforeDestroy?.Invoke();
                        Destroy(gameObject);
                    }
                });
            }
        }
    }

    /// <summary>런타임에 URP Lit 머티리얼을 Transparent 모드로 전환합니다.</summary>
    private void EnableTransparency(Renderer[] renderers)
    {
        foreach (var r in renderers)
        {
            foreach (var mat in r.materials)
            {
                mat.SetFloat(PropSurface,  1f);   // Transparent
                mat.SetFloat(PropBlend,    0f);   // Alpha
                mat.SetFloat(PropSrcBlend, 5f);   // SrcAlpha
                mat.SetFloat(PropDstBlend, 10f);  // OneMinusSrcAlpha
                mat.SetFloat(PropZWrite,   0f);
                mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            }
        }
    }

    private void DisableColliders()
    {
        foreach (var col in GetComponentsInChildren<Collider>())
            col.enabled = false;
    }

    private void DetachAndStopFire(GameObject fireInstance)
    {
        if (fireInstance == null) return;
        fireInstance.transform.SetParent(null);
        var ps = fireInstance.GetComponent<ParticleSystem>();
        if (ps != null)
            ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        else
            Destroy(fireInstance, 2f);
    }
}
