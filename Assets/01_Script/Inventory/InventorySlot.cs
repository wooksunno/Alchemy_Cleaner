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
                itemIconImage.color = Color.white; // ✨ 노란 틴트 제거, 아이콘 원본 색 그대로
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

    private void RefreshExtractHandle()
    {
        if (_currentHandle != null)
        {
            Destroy(_currentHandle);
            _currentHandle = null;
        }

        if (itemCount <= 0 || extractHandlePrefab == null) return;

        StartCoroutine(SpawnHandleNextFrame());
    }

    private System.Collections.IEnumerator SpawnHandleNextFrame()
    {
        yield return new WaitForEndOfFrame();

        if (itemCount <= 0 || extractHandlePrefab == null) yield break;

        Transform refTransform = itemIconImage != null ? itemIconImage.transform : transform;
        Vector3 spawnPos = refTransform.position + refTransform.TransformVector(handleSpawnOffset);

        _currentHandle = Instantiate(extractHandlePrefab, spawnPos, refTransform.rotation);

        var handle = _currentHandle.GetComponent<ExtractHandle>();
        if (handle != null)
        {
            handle.Initialize(this);
        }
    }

    public void ExtractToInteractor(IXRSelectInteractor interactor)
    {
        if (itemCount <= 0 || potionRecipe == null) return;

        GameObject prefabToSpawn = potionRecipe.resultPrefab;
        if (prefabToSpawn == null)
        {
            Debug.LogWarning($"[InventorySlot] {potionRecipe.potionName}의 resultPrefab이 없습니다.");
            return;
        }

        GameObject spawnedObj = Instantiate(prefabToSpawn, interactor.transform.position, interactor.transform.rotation);
        spawnedObj.transform.localScale = Vector3.one;

        var grabInteractable = spawnedObj.GetComponent<XRGrabInteractable>();
        if (grabInteractable != null && _interactionManager != null)
        {
            _interactionManager.SelectEnter(interactor, grabInteractable);
        }

        if (DataManager.Instance != null)
            DataManager.Instance.RemoveItem(potionRecipe);

        ClearSlot();

        if (InventoryManager.Instance != null)
            InventoryManager.Instance.SortAndRefreshInventory();

        OnItemExtracted?.Invoke();
    }
}