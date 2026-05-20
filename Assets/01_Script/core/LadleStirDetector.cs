using UnityEngine;

/// <summary>
/// 국자 끝(StirTip)에 부착하는 젓기 감지 스크립트
/// 
/// [물리 설정]
/// - 이 오브젝트: SphereCollider (Is Trigger ON, 반경 0.05~0.08 정도)
/// - 부모 Ladle: Rigidbody + XR Grab Interactable
/// 
/// [동작 원리]
/// 솥 트리거 안에 있는 동안 이동 속도가 임계값을 넘으면
/// 쿨다운마다 CauldronController.Stir()를 1회 호출합니다.
/// </summary>
public class LadleStirDetector : MonoBehaviour
{
    [Header("젓기 판정 설정")]
    [Tooltip("젓기로 인정되는 최소 이동 속도 (m/s)")]
    [SerializeField] private float stirSpeedThreshold = 0.4f;

    [Tooltip("1회 젓기 인정 후 다음 인정까지의 쿨다운 (초)")]
    [SerializeField] private float stirCooldown = 1f;

    // ── 내부 상태 ─────────────────────────────────────────────

    private CauldronController _targetCauldron = null; // 현재 안에 있는 솥
    private bool _isInsideCauldron = false;

    private Vector3 _prevPosition;
    private float _cooldownTimer = 0f;

    // ── Unity 루프 ────────────────────────────────────────────

    private void Update()
    {
        if (_cooldownTimer > 0f)
            _cooldownTimer -= Time.deltaTime;

        if (!_isInsideCauldron || _targetCauldron == null) return;

        // 이번 프레임 이동 속도 계산
        float speed = (transform.position - _prevPosition).magnitude / Time.deltaTime;

        if (speed >= stirSpeedThreshold && _cooldownTimer <= 0f)
        {
            _targetCauldron.Stir();
            _cooldownTimer = stirCooldown;

            //Debug.Log($"[Ladle] 젓기 감지! 속도: {speed:F2} m/s");
        }

        _prevPosition = transform.position;
    }

    // ── 트리거 감지 ───────────────────────────────────────────

    private void OnTriggerEnter(Collider other)
    {
        // 솥 트리거 안으로 진입
        var cauldron = other.GetComponent<CauldronController>();
        if (cauldron == null) return;

        _targetCauldron = cauldron;
        _isInsideCauldron = true;
        _prevPosition = transform.position;

        Debug.Log("[Ladle] 솥 진입 → 젓기 감지 시작");
    }

    private void OnTriggerExit(Collider other)
    {
        // 솥 트리거 밖으로 나옴
        if (other.GetComponent<CauldronController>() == null) return;

        _targetCauldron = null;
        _isInsideCauldron = false;

        Debug.Log("[Ladle] 솥 이탈 → 젓기 감지 중단");
    }
}