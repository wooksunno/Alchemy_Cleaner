using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class InventoryToggle : MonoBehaviour
{
    [Header("연동할 인벤토리 UI Canvas")]
    public GameObject inventoryCanvas;

    [Header("인풋 액션 설정")]
    public InputActionReference menuButtonAction;

    [Header("버튼 연동")]
    public GameObject actionButtonObject;

    [Header("UI 생성 위치 미세조정")]
    public float distanceFromPlayer = 1.2f;
    public float heightOffset = -0.2f;

    private Transform _cameraTransform;
    private bool _isUIActive = false;

    private void Awake()
    {
        if (Camera.main != null)
        {
            _cameraTransform = Camera.main.transform;
        }

        if (inventoryCanvas != null)
        {
            inventoryCanvas.SetActive(false);
        }

        // 씬 전환 시 잠겨버리는 M 키 인풋 액션을 강제로 깨웁니다.
        if (menuButtonAction != null && menuButtonAction.action != null)
        {
            menuButtonAction.action.Enable();

            // ✨ [진짜 치트키 소스: 끊어진 T키 링크선 강제 납땜]
            // 복사되면서 꼬인 시뮬레이터 인풋 자산의 권한을 이 씬의 가방으로 강제 귀속시킵니다.
            menuButtonAction.action.actionMap.Enable();
        }
    }

    private void OnEnable()
    {
        if (menuButtonAction != null)
            menuButtonAction.action.performed += OnMenuButtonPressed;
    }

    private void OnDisable()
    {
        if (menuButtonAction != null)
            menuButtonAction.action.performed -= OnMenuButtonPressed;
    }

    /// <summary>
    /// M 키를 눌러 가방이 열릴 때, 인풋 링크선이 깨지지 않도록 안전하게 글자를 교체합니다.
    /// </summary>
    private void OnMenuButtonPressed(InputAction.CallbackContext context)
    {
        if (inventoryCanvas == null || _cameraTransform == null) return;

        _isUIActive = !_isUIActive;
        inventoryCanvas.SetActive(_isUIActive);

        if (_isUIActive)
        {
            // ✨ [T키 오류 해결의 핵심] 가방이 열릴 때 안전하게 글자를 갱신합니다.
            UpdateButtonTextByScene();
            PositionInventoryInFront();
        }
    }

    /// <summary>
    /// 현재 씬의 번호를 판별하여 START 또는 HOME 글자를 안전하게 주입하는 메서드입니다.
    /// </summary>
    private void UpdateButtonTextByScene()
    {
        if (actionButtonObject == null) return;

        int currentBuildIndex = SceneManager.GetActiveScene().buildIndex;

        // 0번 씬(시작 화면)이면 START, 1번 씬(게임 맵)이면 HOME으로 설정
        string targetText = (currentBuildIndex == 0) ? "START" : "HOME";

        var tmpText = actionButtonObject.GetComponentInChildren<TextMeshProUGUI>();
        if (tmpText != null) tmpText.text = targetText;

        var normalText = actionButtonObject.GetComponentInChildren<UnityEngine.UI.Text>();
        if (normalText != null) normalText.text = targetText;
    }

    /// <summary>
    /// UI 생성 위치 조정
    /// </summary>
    private void PositionInventoryInFront()
    {
        Vector3 forward = _cameraTransform.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 targetPosition = _cameraTransform.position + (forward * distanceFromPlayer);
        targetPosition.y += heightOffset;

        Quaternion targetRotation = Quaternion.LookRotation(forward);

        inventoryCanvas.transform.position = targetPosition;
        inventoryCanvas.transform.rotation = targetRotation;
    }

    /// <summary>
    /// T 키(상호작용 클릭) 및 마우스 클릭 시 실행될 최종 통합 메서드입니다.
    /// </summary>
    public void ChangeToGameScene()
    {
        int currentBuildIndex = SceneManager.GetActiveScene().buildIndex;

        if (currentBuildIndex == 0)
        {
            Debug.Log("[Inventory] START 클릭: 게임 맵(인덱스 1)으로 이동합니다.");
            SceneManager.LoadScene(1);
        }
        else if (currentBuildIndex == 1)
        {
            Debug.Log("[Inventory] HOME 클릭: 시작 화면(인덱스 0)으로 복귀합니다.");
            SceneManager.LoadScene(0);
        }
    }
}