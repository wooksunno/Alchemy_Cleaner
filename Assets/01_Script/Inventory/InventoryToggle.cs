using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// VR UI 인벤토리의 활성화 토글 및 플레이어 정면 정렬, 
/// 그리고 게임 플레이 씬으로의 전이를 총괄하는 UI 흐름 제어 클래스입니다.
/// </summary>
public class InventoryToggle : MonoBehaviour
{
    [Header("연동할 인벤토리 UI Canvas")]
    [SerializeField] private GameObject inventoryCanvas;

    [Header("인풋 액션 설정 (XRI 프리셋 활용)")]
    [SerializeField] private InputActionReference menuButtonAction;

    [Header("UI 생성 위치 미세조정")]
    [Tooltip("플레이어 시야로부터 배치할 거리 (미터 단위)")]
    [SerializeField] private float distanceFromPlayer = 1.2f;

    [Tooltip("플레이어 눈높이 기준 수직 오프셋")]
    [SerializeField] private float heightOffset = -0.2f;

    [Header("씬 전환 설정")]
    [Tooltip("이동할 대상 플레이 씬의 정식 명칭을 입력합니다.")]
    [SerializeField] private string targetSceneName = "Clean Map 1";

    private Transform _mainCameraTransform;
    private bool _isChanging = false;

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
        if (Camera.main != null)
        {
            _mainCameraTransform = Camera.main.transform;
        }

        if (inventoryCanvas != null)
        {
            inventoryCanvas.SetActive(false);
        }
    }

    private void OnMenuButtonPressed(InputAction.CallbackContext context)
    {
        if (inventoryCanvas == null || _mainCameraTransform == null) return;

        bool isActive = !inventoryCanvas.activeSelf;

        if (isActive)
        {
            RepositionCanvasFrontOfPlayer();
        }

        inventoryCanvas.SetActive(isActive);
    }

    /// <summary>
    /// 메인 카메라의 수평 벡터를 기반으로 캔버스를 플레이어 정면에 정렬합니다 (멀미 방지 알고리즘 반영).
    /// </summary>
    private void RepositionCanvasFrontOfPlayer()
    {
        Vector3 cameraForward = _mainCameraTransform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();

        Vector3 targetPosition = _mainCameraTransform.position
                                 + (cameraForward * distanceFromPlayer)
                                 + (Vector3.up * heightOffset);

        Quaternion targetRotation = Quaternion.LookRotation(cameraForward);

        inventoryCanvas.transform.position = targetPosition;
        inventoryCanvas.transform.rotation = targetRotation;
    }

    /// <summary>
    /// UI 인터페이스 버튼 입력을 받아 지정된 게임 플레이 씬을 로드합니다.
    /// 유니티 이벤트 시스템(On Click)에서 직접 호출할 수 있도록 인자가 없는 public 형태를 유지합니다.
    /// </summary>
    public void ChangeToGameScene()
    {
        if (_isChanging) return;
        _isChanging = true;

        Debug.Log($"[InventoryToggle] {targetSceneName} 씬 로드를 시작합니다.");
        SceneManager.LoadScene(targetSceneName);
    }
}