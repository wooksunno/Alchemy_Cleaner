using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class InventoryToggle : MonoBehaviour
{
    [Header("인벤토리 UI 설정")]
    public GameObject inventoryCanvas;
    [SerializeField] private float spawnDistance = 1.5f;
    [SerializeField] private float spawnHeight = 0.0f;

    [Header("🚀 씬 전환 버튼 연동")]
    public TextMeshProUGUI buttonText;

    [Header("🎮 VR 인풋 시스템 세팅")]
    public InputActionProperty menuButtonAction;

    private Transform _cameraTransform;
    private bool _isUIActive = false;

    private void Start()
    {
        if (Camera.main != null) _cameraTransform = Camera.main.transform;
        if (inventoryCanvas != null) inventoryCanvas.SetActive(false);
        UpdateButtonTextByScene();
    }

    private void OnEnable()
    {
        menuButtonAction.action?.Enable();
        InventorySlot.OnItemExtracted += CloseInventory;
    }

    private void OnDisable()
    {
        InventorySlot.OnItemExtracted -= CloseInventory;
    }

    private void Update()
    {
        if (menuButtonAction.action != null && menuButtonAction.action.WasPressedThisFrame())
        {
            ToggleInventory();
            return;
        }
        if (Input.GetKeyDown(KeyCode.M)) ToggleInventory();
    }

    public void ToggleInventory()
    {
        if (inventoryCanvas == null) return;
        if (_cameraTransform == null && Camera.main != null) _cameraTransform = Camera.main.transform;

        _isUIActive = !_isUIActive;
        inventoryCanvas.SetActive(_isUIActive);

        var canvasCollider = inventoryCanvas.GetComponent<BoxCollider>();
        if (canvasCollider != null) canvasCollider.enabled = _isUIActive;

        if (_isUIActive)
        {
            PositionInventoryInFront();
            RestoreInventoryFromSO();
        }
    }

    private void CloseInventory()
    {
        if (inventoryCanvas == null || !_isUIActive) return;
        _isUIActive = false;
        inventoryCanvas.SetActive(false);

        var canvasCollider = inventoryCanvas.GetComponent<BoxCollider>();
        if (canvasCollider != null) canvasCollider.enabled = false;
    }

    private void RestoreInventoryFromSO()
    {
        if (DataManager.Instance == null) return;

        var slots = FindObjectsOfType<InventorySlot>();
        System.Array.Sort(slots, (a, b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));

        int savedCount = DataManager.Instance.savedItems.Count;
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < savedCount)
                slots[i].SetSlotData(DataManager.Instance.savedItems[i], 1);
            else
                slots[i].ClearSlot();
        }

        if (InventoryManager.Instance != null)
            InventoryManager.Instance.SortAndRefreshInventory();
    }

    private void PositionInventoryInFront()
    {
        Vector3 forward = _cameraTransform.forward;
        forward.y = 0; forward.Normalize();
        inventoryCanvas.transform.position = _cameraTransform.position + forward * spawnDistance + Vector3.up * spawnHeight;
        inventoryCanvas.transform.rotation = Quaternion.LookRotation(forward);
    }

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