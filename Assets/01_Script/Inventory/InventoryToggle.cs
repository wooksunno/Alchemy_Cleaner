using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class InventoryToggle : MonoBehaviour
{
    [Header("인벤토리 UI 설정")]
    public GameObject inventoryCanvas;
    [SerializeField] private float spawnDistance = 1.5f;
    [SerializeField] private float spawnHeight = 0.0f;

    [Header("🚀 씬 전환 버튼 연동 (영문 고정)")]
    public TextMeshProUGUI buttonText;

    [Header("🎮 VR 인풋 시스템 세팅 (최우선)")]
    public InputActionProperty menuButtonAction;

    private Transform _cameraTransform;
    private bool _isUIActive = false;

    private void Start()
    {
        if (Camera.main != null) _cameraTransform = Camera.main.transform;
        if (inventoryCanvas != null) inventoryCanvas.SetActive(false);
        UpdateButtonTextByScene();
    }

    private void OnEnable() => menuButtonAction.action?.Enable();

    private void Update()
    {
        // 1순위: VR 컨트롤러 메뉴 버튼 입력 감지
        if (menuButtonAction.action != null && menuButtonAction.action.WasPressedThisFrame())
        {
            ToggleInventory();
            return;
        }

        // 2순위: PC 테스트용 키보드 M 키
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        if (inventoryCanvas == null) return;
        if (_cameraTransform == null && Camera.main != null) _cameraTransform = Camera.main.transform;

        _isUIActive = !_isUIActive;
        inventoryCanvas.SetActive(_isUIActive);

        // 가방 뒤 장벽 콜라이더 실시간 온오프 (스타트 버튼 터치 레이저 방해 방지)
        var canvasCollider = inventoryCanvas.GetComponent<BoxCollider>();
        if (canvasCollider != null) canvasCollider.enabled = _isUIActive;

        // ✨ 핵심 기믹: 1번 씬이든 2번 씬이든 가방이 열리기만 하면 실시간 동기화 복구!
        if (_isUIActive)
        {
            PositionInventoryInFront();
            RestoreInventoryFromSO();
        }
    }

    /// <summary>
    /// 이름 비교 연산 없이 보관함에 들어있는 SO 순서대로 슬롯에 꽂아 노란 불빛을 켭니다.
    /// </summary>
    private void RestoreInventoryFromSO()
    {
        if (DataManager.Instance == null) return;

        var slots = FindObjectsOfType<InventorySlot>();
        System.Array.Sort(slots, (a, b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));

        int savedCount = DataManager.Instance.savedItems.Count;

        for (int i = 0; i < slots.Length; i++)
        {
            if (i < savedCount)
            {
                IngredientData data = DataManager.Instance.savedItems[i];
                slots[i].SetSlotData(data, 1, null);
            }
            else
            {
                slots[i].ClearSlot(); // 남는 슬롯은 깨끗하게 밀어버리기
            }
        }

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.SortAndRefreshInventory();
        }
    }

    private void PositionInventoryInFront()
    {
        Vector3 cameraPosition = _cameraTransform.position;
        Vector3 cameraForward = _cameraTransform.forward;
        cameraForward.y = 0; cameraForward.Normalize();
        inventoryCanvas.transform.position = cameraPosition + cameraForward * spawnDistance + Vector3.up * spawnHeight;
        inventoryCanvas.transform.rotation = Quaternion.LookRotation(cameraForward);
    }

    // VR 레이저 트리거(Select) 및 마우스 클릭에 바인딩할 양방향 워프 함수
    public void HandleSceneTransition()
    {
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (currentScene == "New Scene") UnityEngine.SceneManagement.SceneManager.LoadScene("Clean Map 1");
        else if (currentScene == "Clean Map 1") UnityEngine.SceneManagement.SceneManager.LoadScene("New Scene");
    }

    private void UpdateButtonTextByScene()
    {
        if (buttonText == null) return;
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        buttonText.text = (currentScene == "New Scene") ? "Start" : "Home";
    }
}