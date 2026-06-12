using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class InventorySlot : MonoBehaviour
{
    [Header("슬롯 상태")]
    public PotionRecipe potionRecipe;
    [SerializeField] private int itemCount = 0;
    public int ItemCount => itemCount;

    [Header("UI 컴포넌트 연결")]
    [SerializeField] private Image itemIconImage;
    [SerializeField] private TextMeshProUGUI countText;

    [Header("🧪 추출 핸들 설정")]
    [Tooltip("아이템이 들어있을 때 슬롯 위에 생성될 그랩 가능한 투명 핸들 프리팹")]
    [SerializeField] private GameObject extractHandlePrefab;
    [Tooltip("핸들이 스폰될 위치 오프셋 (로컬 좌표)")]
    [SerializeField] private Vector3 handleSpawnOffset = Vector3.zero;

    private GameObject _currentHandle;
    private XRInteractionManager _interactionManager;

    public static event System.Action OnItemExtracted;

    private void Start()
    {
        _interactionManager = FindFirstObjectByType<XRInteractionManager>();
        UpdateSlotUI();
    }

    public void SetSlotData(PotionRecipe recipe, int count)
    {
        potionRecipe = recipe;
        itemCount = Mathf.Clamp(count, 0, 1);
        UpdateSlotUI();
        RefreshExtractHandle();
    }

    public void ClearSlot()
    {
        potionRecipe = null;
        itemCount = 0;
        UpdateSlotUI();
        RefreshExtractHandle();
    }

    public void UpdateSlotUI()
    {
        if (itemCount > 0)
        {
            if (itemIconImage != null)
            {
                itemIconImage.sprite = potionRecipe?.resultIcon != null ? potionRecipe.resultIcon : null;
                itemIconImage.color = Color.yellow;
                itemIconImage.enabled = true;
            }
            if (countText != null) countText.text = "";
        }
        else
        {
            if (itemIconImage != null)
            {
                itemIconImage.sprite = null;
                itemIconImage.color = Color.white;
                itemIconImage.enabled = false;
            }
            if (countText != null) countText.text = "";
        }
    }

    /// <summary>
    /// 슬롯에 아이템이 있으면 ExtractHandle을 생성, 없으면 제거합니다.
    /// </summary>
    private void RefreshExtractHandle()
    {
        // 기존 핸들 정리
        if (_currentHandle != null)
        {
            Destroy(_currentHandle);
            _currentHandle = null;
        }

        if (itemCount <= 0 || extractHandlePrefab == null) return;

        // 슬롯 위치에 핸들 스폰
        Vector3 spawnPos = transform.position + transform.TransformVector(handleSpawnOffset);
        _currentHandle = Instantiate(extractHandlePrefab, spawnPos, transform.rotation);

        // 핸들의 ExtractHandle 컴포넌트에 이 슬롯을 연결
        var handle = _currentHandle.GetComponent<ExtractHandle>();
        if (handle != null)
        {
            handle.Initialize(this);
        }
    }

    /// <summary>
    /// ExtractHandle이 그랩되었을 때 호출됩니다.
    /// 실제 포션을 스폰하고 그랩을 이전한 뒤, 슬롯을 비웁니다.
    /// </summary>
    public void ExtractToInteractor(IXRSelectInteractor interactor)
    {
        if (itemCount <= 0 || potionRecipe == null) return;

        GameObject prefabToSpawn = potionRecipe.resultPrefab;
        if (prefabToSpawn == null)
        {
            Debug.LogWarning($"[InventorySlot] {potionRecipe.potionName}의 resultPrefab이 없습니다.");
            return;
        }

        // 인터랙터(컨트롤러) 위치에 실제 포션 스폰
        GameObject spawnedObj = Instantiate(prefabToSpawn, interactor.transform.position, interactor.transform.rotation);
        spawnedObj.transform.localScale = Vector3.one;

        var grabInteractable = spawnedObj.GetComponent<XRGrabInteractable>();
        if (grabInteractable != null && _interactionManager != null)
        {
            _interactionManager.SelectEnter(interactor, grabInteractable);
        }

        // 데이터 정리
        if (DataManager.Instance != null)
            DataManager.Instance.RemoveItem(potionRecipe);

        ClearSlot();

        if (InventoryManager.Instance != null)
            InventoryManager.Instance.SortAndRefreshInventory();

        OnItemExtracted?.Invoke();
    }
}