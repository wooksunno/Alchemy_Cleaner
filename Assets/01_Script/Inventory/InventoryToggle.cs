using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// [VR 인벤토리 토글 매니저 - 플레이어 정면 생성 버전]
/// M 키를 누를 때마다 메인 카메라(VR 헤드셋)의 위치와 정면 방향을 계산하여
/// 항상 플레이어 눈앞 알맞은 거리에 가방 UI를 띄워줍니다.
/// </summary>
public class InventoryToggle : MonoBehaviour
{
    [Header("연동할 인벤토리 UI Canvas")]
    [SerializeField] private GameObject inventoryCanvas;

    [Header("인풋 액션 설정 (XRI 프리셋 활용)")]
    [SerializeField] private InputActionReference menuButtonAction;

    [Header("UI 생성 위치 미세조정")]
    [Tooltip("플레이어 눈앞으로부터 얼마나 떨어뜨려 배치할지 (미터 단위)")]
    [SerializeField] private float distanceFromPlayer = 1.2f;

    [Tooltip("플레이어 눈높이 기준으로 UI를 위아래로 얼마나 조절할지 (+는 위, -는 아래)")]
    [SerializeField] private float heightOffset = -0.2f;

    private Transform _mainCameraTransform;

    private void OnEnable()
    {
        if (menuButtonAction != null)
        {
            menuButtonAction.action.started += OnMenuButtonPressed;
        }
    }

    private void OnDisable()
    {
        if (menuButtonAction != null)
        {
            menuButtonAction.action.started -= OnMenuButtonPressed;
        }
    }

    private void Start()
    {
        // VR 헤드셋(메인 카메라)의 Transform을 찾아둡니다.
        if (Camera.main != null)
        {
            _mainCameraTransform = Camera.main.transform;
        }

        // 게임 시작할 때는 인벤토리를 꺼둡니다.
        if (inventoryCanvas != null)
        {
            inventoryCanvas.SetActive(false);
        }
    }

    private void OnMenuButtonPressed(InputAction.CallbackContext context)
    {
        if (inventoryCanvas == null || _mainCameraTransform == null) return;

        // 1. 현재 켜져있는지 꺼져있는지 상태 반전
        bool isActive = !inventoryCanvas.activeSelf;

        // 2. [🚨 핵심 로직] 가방을 '켤 때' 현재 플레이어 눈앞 위치를 계산해서 순간이동 시킴
        if (isActive)
        {
            RepositionCanvasFrontOfPlayer();
        }

        // 3. 최종 활성화/비활성화
        inventoryCanvas.SetActive(isActive);
        Debug.Log($"[Inventory] 가방 토글: {isActive} (플레이어 정면 배치 완료)");
    }

    /// <summary>
    /// 메인 카메라의 위치와 회전값을 기반으로 캔버스를 플레이어 정면에 정렬합니다.
    /// </summary>
    private void RepositionCanvasFrontOfPlayer()
    {
        // A. 플레이어 헤드셋의 평면(수평) 정면 방향을 계산 (UI가 수평으로만 정렬되도록 하여 멀미 방지)
        Vector3 cameraForward = _mainCameraTransform.forward;
        cameraForward.y = 0; // 고개를 위아래로 숙여도 UI가 땅에 박히거나 하늘로 치솟지 않게 수평 고정
        cameraForward.Normalize();

        // B. 최종 위치 계산: 현재 카메라 위치 + (정면 방향 * 거리) + (높이 보정)
        Vector3 targetPosition = _mainCameraTransform.position
                                 + (cameraForward * distanceFromPlayer)
                                 + (Vector3.up * heightOffset);

        // C. UI가 플레이어를 똑바로 바라보도록 회전값 계산
        Quaternion targetRotation = Quaternion.LookRotation(cameraForward);

        // D. 캔버스에 계산된 위치와 회전 적용
        inventoryCanvas.transform.position = targetPosition;
        inventoryCanvas.transform.rotation = targetRotation;
    }
}