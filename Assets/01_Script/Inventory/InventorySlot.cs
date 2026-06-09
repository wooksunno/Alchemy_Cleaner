using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;

public class InventorySlot : MonoBehaviour
{
    [Header("슬롯 상태")]
    public IngredientData ingredientData;
    [SerializeField] private int itemCount = 0;
    public int ItemCount => itemCount;
    public GameObject savedWorldPrefab;

    [Header("UI 컴포넌트 연결")]
    [SerializeField] private Image itemIconImage;
    [SerializeField] private TextMeshProUGUI countText;

    private XRInteractionManager _interactionManager;

    private void Start()
    {
        _interactionManager = FindFirstObjectByType<XRInteractionManager>();
        UpdateSlotUI();
    }

    public void SetSlotData(IngredientData data, int count, GameObject prefab)
    {
        ingredientData = data;
        itemCount = Mathf.Clamp(count, 0, 1);
        savedWorldPrefab = prefab;
        UpdateSlotUI();
    }

    public void ClearSlot()
    {
        ingredientData = null;
        itemCount = 0;
        savedWorldPrefab = null;
        UpdateSlotUI();
    }

    public void UpdateSlotUI()
    {
        if (itemCount > 0)
        {
            if (itemIconImage != null)
            {
                itemIconImage.sprite = null;
                itemIconImage.color = Color.yellow; // 아이템이 있으면 무조건 노란 불 켜기
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
                itemIconImage.enabled = false; // 빈 슬롯은 끄기
            }
            if (countText != null) countText.text = "";
        }
    }

    // 가방에서 아이템을 바깥으로 다시 끄집어낼 때 호출되는 함수
    public void TryExtractIngredient(UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor interactor)
    {
        if (itemCount <= 0 || savedWorldPrefab == null) return;

        GameObject spawnedObj = Instantiate(savedWorldPrefab, interactor.transform.position, interactor.transform.rotation);
        spawnedObj.transform.localScale = Vector3.one;

        var grabInteractable = spawnedObj.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        if (grabInteractable != null && _interactionManager != null)
        {
            _interactionManager.SelectEnter(interactor, grabInteractable);

            // ✨ [에러 수정 완료] 새로 바뀐 SO 보관함 전용 제거 함수(RemoveItem) 호출!
            if (DataManager.Instance != null && ingredientData != null)
            {
                DataManager.Instance.RemoveItem(ingredientData);
            }

            ClearSlot();

            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.SortAndRefreshInventory();
            }
        }
    }

    public void TryExtractIngredientXRI(SelectEnterEventArgs args)
    {
        TryExtractIngredient(args.interactorObject);
    }
}